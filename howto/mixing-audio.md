---
layout: page
title: Mixing Microphone Input and Speaker Output
highlight: true
reading_time: true
---

Mixing audio from Microphone input and Speaker output (obtained using WASAPI Loopback) was a commonly asked feature.

Before this feature was implemented, Captura used NAudio.
We moved to ManagedBass (a .Net wrapper for un4seen BASS audio library) for audio handling which made it much easier to implement this function.
We implemented this feature using the BassMix addon along with built-in support for recording and WASAPI Loopback.

Here's a [link](https://github.com/MathewSachin/Captura/blob/master/src/Captura.Core/Models/MixedAudioProvider.cs) to the implementation used in Captura.
If you experience broken link (may happen if repository structure changes in future), let me know in the comments.

First, install the ManagedBass and ManagedBass.Mix NuGet packages:

```powershell
Install-Package ManagedBass
Install-Package ManagedBass.Mix
```

Download, `bass.dll` and `bassmix.dll` from [un4seen.com](https://un4seen.com/bass.html) and place in project output directory.

Add these using statements:

```csharp
using ManagedBass;
using ManagedBass.Mix;
```

Initialize BASS and enable WASAPI Loopback recording:

```csharp
// Initialize Default Playback Device.
Bass.Init();

// Enable Loopback Recording.
Bass.Configure(Configuration.LoopbackRecording, true);
```

Create a mixer stream:

```csharp
var mixer = BassMix.CreateMixerStream(44100, 2, BassFlags.Default);
```

Enumerate devices with `Bass.RecordGetDeviceInfo`.
Loopback devices have the `Loopback` property set to true.

In the following snippet, `RecordDeviceIndex` and `LoopbackDeviceIndex` are the indices of the recording and loopback devices respectively.

```csharp
// Initialize Recording device.
Bass.RecordInit(RecordDeviceIndex);

// Set as Current Recording Device.
// RecordInit may handle this automatically.
// But, we do this for cases where the device was already initialized.
Bass.CurrentRecordingDevice = RecordDeviceIndex;

var info = Bass.RecordingInfo;

// Create recording stream.
var record = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | BassFlags.RecordPause, null);

// Add to Mixer.
BassMix.MixerAddChannel(mixer, record, BassFlags.MixerDownMix);
```

When we do WASAPI Loopback recording, we need to play silence on the device we are recording from.
If we don't do so, any moments of silence are not recorded.

So, we need to find the playback device to play silence on.
The loopback device and the corresponding device use the same driver.
So, we can compare the `Driver` properties to find the playback device.

```csharp
int FindPlaybackDevice(int LoopbackDevice)
{
    var driver = Bass.RecordGetDeviceInfo(LoopbackDevice).Driver;

    // Enumerate Playback devices
    for (int i = 0; Bass.GetDeviceInfo(i, out var info); ++i)
        // Compare drivers
        if (info.Driver == driver)
            return i;

    return 0;
}
```

Create the silence stream:

```csharp
var playback = FindPlaybackDevice(LoopbackDeviceIndex);

// Initialize the device to play silence on.
Bass.Init(playback);
Bass.CurrentDevice = playback;

// ManagedBass provides a default implementation of Stream Procedure producing silence.
var silence = Bass.CreateStream(44100, 2, BassFlags.Float, ManagedBass.Extensions.SilenceStreamProcedure);
```

Create the loopback stream:

```csharp
// Initialize the loopback device.
Bass.RecordInit(LoopbackDeviceIndex);
Bass.CurrentRecordingDevice = LoopbackDeviceIndex;

var info = Bass.RecordingInfo;

// Loopback recording requires that the Default frequency and number of channels be used.
var loopback = Bass.RecordStart(info.Frequency, info.Channels, BassFlags.Float | Bass.RecordPause, null);

// Add to mixer.
BassMix.MixerAddChannel(mixer, loopback, BassMix.MixerDownMix);
```

Mute the mixer just to ensure that playing it may not produce any sound:

```csharp
Bass.ChannelSetAttribute(mixer, ChannelAttribute.Volume, 0);
```

Receiving the mixed recorded data:
We set a DSP Procedure on the mixer.
In the Procedure, we copy data from unmanaged memory to a `byte` array.

```csharp
Bass.ChannelSetDSP(Procedure);
```

where `Procedure` is defined as:

```csharp
byte[] buffer;

void Procedure(int Handle, int Channel, IntPtr Buffer, int Length, IntPtr User)
{
    // Increase the buffer size if necessary.
    if (_buffer == null || _buffer.Length < Length)
        _buffer = new byte[Length];

    // Copy data into buffer.
    Marshal.Copy(Buffer, _buffer, 0, Length);
    
    // USE THE DATA. e.g. You can write it to a file.
}
```

Now, to Start Recording, call the `Bass.ChannelPlay` method:

```csharp
// Start playing silence.
Bass.ChannelPlay(silence);

// Start Recording and Loopback.
Bass.ChannelPlay(recording);
Bass.ChannelPlay(loopback);

// Play the mixer.
Bass.ChannelPlay(mixer);
```

And to Pause, call the `Bass.ChannelPause`, but we do it in reverse order:

```csharp
Bass.ChannelPause(mixer);

Bass.ChannelPause(recording);
Bass.ChannelPause(loopback);

Bass.ChannelPause(silence);
```

Recording can be Resumed in the same way it was started.

Finally, to stop recording:

```csharp
Bass.StreamFree(mixer);

Bass.StreamFree(recording);
Bass.StreamFree(loopback);

Bass.StreamFree(silence);
```

Freeing devices:

```csharp
// Free the Playback device.
Bass.CurrentDevice = playback;
Bass.Free();

// Free the Recording device.
Bass.CurrentRecordingDevice = RecordDeviceIndex;
Bass.RecordFree();

// Free the Loopback device.
Bass.CurrentRecordingDevice = LoopbackDeviceIndex;
Bass.RecordFree();
```