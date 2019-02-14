using System;
using System.Drawing;
using System.Runtime.InteropServices;
using DirectShowLib;

// ReSharper disable SuspiciousTypeConversion.Global

namespace Captura.Webcam
{
    /// <summary>
    /// Gets the video output of a webcam or other video device.
    /// </summary>
    class CaptureWebcam : ISampleGrabberCB, IDisposable
    {
        #region Fields
        /// <summary> 
        ///  The video capture device filter. Read-only. To use a different 
        ///  device, dispose of the current Capture instance and create a new 
        ///  instance with the desired device. 
        /// </summary>
        readonly Filter _videoDevice;

        /// <summary>
        ///  The control that will host the preview window. 
        /// </summary>
        readonly IntPtr _previewWindow;

        /// <summary>
        /// The Width and Height of the video feed.
        /// </summary>
        public Size Size => _videoInfoHeader != null ? new Size(_videoInfoHeader.BmiHeader.Width, _videoInfoHeader.BmiHeader.Height) : Size.Empty;

        /// <summary>
        /// When graphState==Rendered, have we rendered the preview stream?
        /// </summary>
        bool _isPreviewRendered;

        /// <summary>
        /// Do we need the preview stream rendered (VideoDevice and PreviewWindow != null)
        /// </summary>
        bool _wantPreviewRendered;

        /// <summary>
        /// State of the internal filter graph.
        /// </summary>
        GraphState _actualGraphState;

        /// <summary>
        /// DShow Filter: Graph builder.
        /// </summary>
        IGraphBuilder _graphBuilder;

        /// <summary>
        /// DShow Filter: building graphs for capturing video.
        /// </summary>
        ICaptureGraphBuilder2 _captureGraphBuilder;

        /// <summary>
        /// DShow Filter: selected video device.
        /// </summary>
        IBaseFilter _videoDeviceFilter;

        /// <summary>
        /// DShow Filter: Start/Stop the filter graph -> copy of graphBuilder.
        /// </summary>
        IMediaControl _mediaControl;

        /// <summary>
        /// DShow Filter: Control preview window -> copy of graphBuilder.
        /// </summary>
        IVideoWindow _videoWindow;

        /// <summary>
        /// DShow Filter: selected video compressor.
        /// </summary>
        IBaseFilter _videoCompressorFilter;

        /// <summary>
        /// Grabber filter interface. 
        /// </summary>
        IBaseFilter _baseGrabFlt;

        byte[] _savedArray;

        ISampleGrabber _sampGrabber;
        VideoInfoHeader _videoInfoHeader;
        #endregion

        readonly DummyForm _form;

        public CaptureWebcam(Filter VideoDevice, Action OnClick, IntPtr PreviewWindow)
        {
            _videoDevice = VideoDevice ?? throw new ArgumentException("The videoDevice parameter must be set to a valid Filter.\n");

            _form = new DummyForm();
            _form.Show();

            _form.Click += (S, E) => OnClick?.Invoke();

            _previewWindow = PreviewWindow != IntPtr.Zero ? PreviewWindow : _form.Handle;

            CreateGraph();
        }

        #region Public Methods
        /// <summary>
        /// Starts the video preview from the video source.
        /// </summary>
        public void StartPreview()
        {
            DerenderGraph();

            _wantPreviewRendered = _previewWindow != IntPtr.Zero && _videoDevice != null;

            RenderGraph();
            StartPreviewIfNeeded();
        }

        /// <summary>
        /// Stops the video previewing.
        /// </summary>
        public void StopPreview()
        {
            DerenderGraph();

            _wantPreviewRendered = false;

            RenderGraph();
            StartPreviewIfNeeded();
        }

        /// <summary> Resize the preview when the PreviewWindow is resized </summary>
        public void OnPreviewWindowResize(int X, int Y, int Width, int Height)
        {
            // Position video window in client rect of owner window.
            _videoWindow?.SetWindowPosition(X, Y, Width, Height);
        }

        /// <summary>
        /// Gets the current frame from the buffer.
        /// </summary>
        /// <returns>The Bitmap of the frame.</returns>
        public IBitmapImage GetFrame(IBitmapLoader BitmapLoader)
        {
            if (_actualGraphState != GraphState.Rendered)
                return null;

            // Asks for the buffer size.
            var bufferSize = 0;
            _sampGrabber.GetCurrentBuffer(ref bufferSize, IntPtr.Zero);

            if (bufferSize <= 0)
            {
                return null;
            }

            if (_savedArray == null || _savedArray.Length < bufferSize)
                _savedArray = new byte[bufferSize + 64000];

            // Allocs the byte array.
            var handleObj = GCHandle.Alloc(_savedArray, GCHandleType.Pinned);

            // Gets the addres of the pinned object.
            var address = handleObj.AddrOfPinnedObject();

            try
            {
                // Puts the buffer inside the byte array.
                _sampGrabber.GetCurrentBuffer(ref bufferSize, address);

                // Image size.
                var width = _videoInfoHeader.BmiHeader.Width;
                var height = _videoInfoHeader.BmiHeader.Height;

                var stride = width * 4;
                address += height * stride;

                return BitmapLoader.CreateBitmapBgr32(new Size(width, height), address, -stride);
            }
            finally
            {
                handleObj.Free();
            }
        }

        /// <summary>
        /// Closes and cleans the video previewing.
        /// </summary>
        public void Dispose()
        {
            _wantPreviewRendered = false;

            try { DestroyGraph(); }
            catch { }

            _form.Dispose();

            _savedArray = null;
        }
        #endregion

        #region Private Methods
        /// <summary> 
        ///  Create a new filter graph and add filters (devices, compressors, misc),
        ///  but leave the filters unconnected. Call RenderGraph()
        ///  to connect the filters.
        /// </summary>
        void CreateGraph()
        {
            // Skip if already created
            if (_actualGraphState < GraphState.Created)
            {
                // Make a new filter graph
                _graphBuilder = (IGraphBuilder)new FilterGraph();

                // Get the Capture Graph Builder
                _captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

                // Link the CaptureGraphBuilder to the filter graph
                var hr = _captureGraphBuilder.SetFiltergraph(_graphBuilder);

                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                _sampGrabber = (ISampleGrabber)new SampleGrabber();

                _baseGrabFlt = (IBaseFilter)_sampGrabber;

                var media = new AMMediaType();

                // Get the video device and add it to the filter graph
                if (_videoDevice != null)
                {
                    _videoDeviceFilter = (IBaseFilter)Marshal.BindToMoniker(_videoDevice.MonikerString);

                    hr = _graphBuilder.AddFilter(_videoDeviceFilter, "Video Capture Device");

                    if (hr < 0)
                        Marshal.ThrowExceptionForHR(hr);

                    media.majorType = MediaType.Video;
                    media.subType = MediaSubType.RGB32;
                    media.formatType = FormatType.VideoInfo;
                    media.temporalCompression = true;

                    hr = _sampGrabber.SetMediaType(media);

                    if (hr < 0)
                        Marshal.ThrowExceptionForHR(hr);

                    hr = _graphBuilder.AddFilter(_baseGrabFlt, "Grabber");

                    if (hr < 0)
                        Marshal.ThrowExceptionForHR(hr);
                }

                // Retrieve the stream control interface for the video device
                // FindInterface will also add any required filters
                // (WDM devices in particular may need additional upstream filters to function).

                // Try looking for an interleaved media type
                var cat = PinCategory.Capture;
                var med = MediaType.Interleaved;
                var iid = typeof(IAMStreamConfig).GUID;

                hr = _captureGraphBuilder.FindInterface(cat, med, _videoDeviceFilter, iid, out _);

                if (hr != 0)
                {
                    // If not found, try looking for a video media type
                    med = MediaType.Video;
                    _captureGraphBuilder.FindInterface(cat, med, _videoDeviceFilter, iid, out _);
                }
                
                // Retreive the media control interface (for starting/stopping graph)
                _mediaControl = (IMediaControl)_graphBuilder;

                _videoInfoHeader = Marshal.PtrToStructure<VideoInfoHeader>(media.formatPtr);
                Marshal.FreeCoTaskMem(media.formatPtr);
                media.formatPtr = IntPtr.Zero;

                hr = _sampGrabber.SetBufferSamples(true);

                if (hr == 0)
                    hr = _sampGrabber.SetOneShot(false);

                if (hr == 0)
                    hr = _sampGrabber.SetCallback(null, 0);

                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);
            }

            // Update the state now that we are done
            _actualGraphState = GraphState.Created;
        }

        /// <summary>
        ///  Disconnect and remove all filters except the device
        ///  and compressor filters. This is the opposite of
        ///  renderGraph(). Soem properties such as FrameRate
        ///  can only be set when the device output pins are not
        ///  connected. 
        /// </summary>
        void DerenderGraph()
        {
            // Stop the graph if it is running (ignore errors)
            _mediaControl?.Stop();

            // Free the preview window (ignore errors)
            if (_videoWindow != null)
            {
                _videoWindow.put_Visible(OABool.False);
                _videoWindow.put_Owner(IntPtr.Zero);
                _videoWindow = null;
            }

            if ((int) _actualGraphState < (int) GraphState.Rendered)
                return;

            // Update the state
            _actualGraphState = GraphState.Created;
            _isPreviewRendered = false;

            // Disconnect all filters downstream of the 
            // video and audio devices. If we have a compressor
            // then disconnect it, but don't remove it
            if (_videoDeviceFilter != null)
                RemoveDownstream(_videoDeviceFilter);
        }

        /// <summary>
        ///  Removes all filters downstream from a filter from the graph.
        ///  This is called only by DerenderGraph() to remove everything
        ///  from the graph except the devices and compressors. The parameter
        ///  "removeFirstFilter" is used to keep a compressor (that should
        ///  be immediately downstream of the device) if one is begin used.
        /// </summary>
        void RemoveDownstream(IBaseFilter Filter)
        {
            // Get a pin enumerator off the filter
            var hr = Filter.EnumPins(out var pinEnum);

            if (pinEnum == null)
                return;

            pinEnum.Reset();

            if (hr != 0)
                return;

            // Loop through each pin
            var pins = new IPin[1];

            do
            {
                // Get the next pin
                hr = pinEnum.Next(1, pins, IntPtr.Zero);

                if (hr != 0 || pins[0] == null)
                    continue;

                // Get the pin it is connected to
                pins[0].ConnectedTo(out var pinTo);

                if (pinTo != null)
                {
                    // Is this an input pin?
                    hr = pinTo.QueryPinInfo(out var info);

                    if (hr == 0 && info.dir == PinDirection.Input)
                    {
                        // Recurse down this branch
                        RemoveDownstream(info.filter);

                        // Disconnect 
                        _graphBuilder.Disconnect(pinTo);
                        _graphBuilder.Disconnect(pins[0]);

                        // Remove this filter
                        // but don't remove the video or audio compressors
                        if (info.filter != _videoCompressorFilter)
                            _graphBuilder.RemoveFilter(info.filter);
                    }

                    Marshal.ReleaseComObject(info.filter);
                    Marshal.ReleaseComObject(pinTo);
                }

                Marshal.ReleaseComObject(pins[0]);
            }
            while (hr == 0);

            Marshal.ReleaseComObject(pinEnum);
        }

        /// <summary>
        ///  Connects the filters of a previously created graph 
        ///  (created by CreateGraph()). Once rendered the graph
        ///  is ready to be used. This method may also destroy
        ///  streams if we have streams we no longer want.
        /// </summary>
        void RenderGraph()
        {
            var didSomething = false;

            // Stop the graph
            _mediaControl?.Stop();

            // Create the graph if needed (group should already be created)
            CreateGraph();

            // Derender the graph if we have a capture or preview stream
            // that we no longer want. We can't derender the capture and 
            // preview streams seperately. 
            // Notice the second case will leave a capture stream intact
            // even if we no longer want it. This allows the user that is
            // not using the preview to Stop() and Start() without
            // rerendering the graph.
            if (!_wantPreviewRendered && _isPreviewRendered)
                DerenderGraph();

            // Render preview stream (only if necessary)
            if (_wantPreviewRendered && !_isPreviewRendered)
            {
                // Render preview (video -> renderer)
                var cat = PinCategory.Preview;
                var med = MediaType.Video;
                var hr = _captureGraphBuilder.RenderStream(cat, med, _videoDeviceFilter, _baseGrabFlt, null);
                if (hr < 0) Marshal.ThrowExceptionForHR(hr);

                // Get the IVideoWindow interface
                _videoWindow = (IVideoWindow)_graphBuilder;

                // Set the video window to be a child of the main window
                hr = _videoWindow.put_Owner(_previewWindow);

                _videoWindow.put_MessageDrain(_form.Handle);

                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                // Set video window style
                hr = _videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren | WindowStyle.ClipSiblings);

                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                // Make the video window visible, now that it is properly positioned
                hr = _videoWindow.put_Visible(OABool.True);

                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                _isPreviewRendered = true;
                didSomething = true;

                var media = new AMMediaType();
                hr = _sampGrabber.GetConnectedMediaType(media);

                if (hr < 0)
                    Marshal.ThrowExceptionForHR(hr);

                if (media.formatType != FormatType.VideoInfo || media.formatPtr == IntPtr.Zero)
                    throw new NotSupportedException("Unknown Grabber Media Format");

                _videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));

                Marshal.FreeCoTaskMem(media.formatPtr);
                media.formatPtr = IntPtr.Zero;
            }

            if (didSomething)
                _actualGraphState = GraphState.Rendered;
        }

        /// <summary>
        ///  Setup and start the preview window if the user has
        ///  requested it (by setting PreviewWindow).
        /// </summary>
        void StartPreviewIfNeeded()
        {
            // Render preview 
            if (_wantPreviewRendered && _isPreviewRendered)
            {
                // Run the graph (ignore errors)
                // We can run the entire graph becuase the capture
                // stream should not be rendered (and that is enforced
                // in the if statement above)
                _mediaControl.Run();
            }
        }

        /// <summary>
        ///  Completely tear down a filter graph and 
        ///  release all associated resources.
        /// </summary>
        void DestroyGraph()
        {
            // Derender the graph (This will stop the graph
            // and release preview window. It also destroys
            // half of the graph which is unnecessary but
            // harmless here.) (ignore errors)
            try { DerenderGraph(); }
            catch { }

            // Update the state after derender because it
            // depends on correct status. But we also want to
            // update the state as early as possible in case
            // of error.
            _actualGraphState = GraphState.Null;
            _isPreviewRendered = false;

            // Remove filters from the graph
            // This should be unnecessary but the Nvidia WDM
            // video driver cannot be used by this application 
            // again unless we remove it. Ideally, we should
            // simply enumerate all the filters in the graph
            // and remove them. (ignore errors)
            if (_graphBuilder != null)
            {
                if (_videoCompressorFilter != null)
                    _graphBuilder.RemoveFilter(_videoCompressorFilter);

                if (_videoDeviceFilter != null)
                    _graphBuilder.RemoveFilter(_videoDeviceFilter);

                // Cleanup
                Marshal.ReleaseComObject(_graphBuilder);
                _graphBuilder = null;
            }

            if (_captureGraphBuilder != null)
            {
                Marshal.ReleaseComObject(_captureGraphBuilder);

                _captureGraphBuilder = null;
            }

            if (_videoDeviceFilter != null)
            {
                Marshal.ReleaseComObject(_videoDeviceFilter);

                _videoDeviceFilter = null;
            }

            if (_videoCompressorFilter != null)
            {
                Marshal.ReleaseComObject(_videoCompressorFilter);

                _videoCompressorFilter = null;
            }

            // These are copies of graphBuilder
            _mediaControl = null;
            _videoWindow = null;

            // For unmanaged objects we haven't released explicitly
            GC.Collect();
        }
        #endregion

        #region SampleGrabber
        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample Sample) => 0;

        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr Buffer, int BufferLen) => 1;
        #endregion
    }
}
