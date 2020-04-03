using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using Captura.SharpAvi;
using Captura.Video;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VideoWritersViewModel : NotifyPropertyChanged
    {
        public IReadOnlyList<IVideoWriterProvider> VideoWriterProviders { get; }
        readonly ObservableCollection<IVideoWriterItem> _videoWriters = new ObservableCollection<IVideoWriterItem>();
        public ReadOnlyObservableCollection<IVideoWriterItem> AvailableVideoWriters { get; }

        public IEnumerable<IVideoConverter> AvailablePostWriters { get; }

        public VideoWritersViewModel(IEnumerable<IVideoWriterProvider> WriterProviders,
            IEnumerable<IVideoConverter> PostWriters,
            SharpAviWriterProvider SharpAviWriterProvider)
        {
            VideoWriterProviders = WriterProviders.ToList();

            AvailableVideoWriters = new ReadOnlyObservableCollection<IVideoWriterItem>(_videoWriters);

            AvailablePostWriters = PostWriters;
            SelectedPostWriter = PostWriters.FirstOrDefault();

            if (VideoWriterProviders.Count > 0)
                SelectedVideoWriterKind = VideoWriterProviders[0];

            AvailableStepWriters = new IVideoWriterItem[]
            {
                new StepsVideoWriterItem(SharpAviWriterProvider.First()),
                new ImageFolderWriterItem()
            };

            SelectedStepsWriter = AvailableStepWriters[0];
        }

        public void RefreshCodecs()
        {
            // Remember selected codec
            var lastVideoCodecName = SelectedVideoWriter?.ToString();

            _videoWriters.Clear();

            foreach (var writerItem in SelectedVideoWriterKind)
            {
                _videoWriters.Add(writerItem);
            }

            // Set First
            if (_videoWriters.Count > 0)
                SelectedVideoWriter = _videoWriters[0];

            var matchingVideoCodec = AvailableVideoWriters.FirstOrDefault(M => M.ToString() == lastVideoCodecName);

            if (matchingVideoCodec != null)
            {
                SelectedVideoWriter = matchingVideoCodec;
            }
        }

        IVideoWriterProvider _writerKind;

        public IVideoWriterProvider SelectedVideoWriterKind
        {
            get => _writerKind;
            set
            {
                if (_writerKind == value)
                    return;

                if (value != null)
                    _writerKind = value;

                OnPropertyChanged();

                RefreshCodecs();
            }
        }

        IVideoWriterItem _writer;

        public IVideoWriterItem SelectedVideoWriter
        {
            get => _writer;
            set => Set(ref _writer, value ?? AvailableVideoWriters.FirstOrDefault());
        }

        IVideoConverter _postWriter;

        public IVideoConverter SelectedPostWriter
        {
            get => _postWriter;
            set => Set(ref _postWriter, value ?? AvailablePostWriters.FirstOrDefault());
        }

        public IReadOnlyList<IVideoWriterItem> AvailableStepWriters { get; }

        IVideoWriterItem _stepsWriter;

        public IVideoWriterItem SelectedStepsWriter
        {
            get => _stepsWriter;
            set => Set(ref _stepsWriter, value ?? AvailableStepWriters[0]);
        }

        public IEnumerable<RecorderMode> AvailableRecorderModes { get; } = Enum
            .GetValues(typeof(RecorderMode))
            .Cast<RecorderMode>();
    }
}