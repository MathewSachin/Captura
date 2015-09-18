using System;

namespace NWaveIn
{
    /// <summary>
    /// Windows multimedia error codes from mmsystem.h.
    /// </summary>
    public enum MmResult
    {
        /// <summary>no error, MMSYSERR_NOERROR</summary>
        NoError = 0,
        /// <summary>unspecified error, MMSYSERR_ERROR</summary>
        UnspecifiedError = 1,
        /// <summary>device ID out of range, MMSYSERR_BADDEVICEID</summary>
        BadDeviceId = 2,
        /// <summary>driver failed enable, MMSYSERR_NOTENABLED</summary>
        NotEnabled = 3,
        /// <summary>device already allocated, MMSYSERR_ALLOCATED</summary>
        AlreadyAllocated = 4,
        /// <summary>device handle is invalid, MMSYSERR_INVALHANDLE</summary>
        InvalidHandle = 5,
        /// <summary>no device driver present, MMSYSERR_NODRIVER</summary>
        NoDriver = 6,
        /// <summary>memory allocation error, MMSYSERR_NOMEM</summary>
        MemoryAllocationError = 7,
        /// <summary>function isn't supported, MMSYSERR_NOTSUPPORTED</summary>
        NotSupported = 8,
        /// <summary>error value out of range, MMSYSERR_BADERRNUM</summary>
        BadErrorNumber = 9,
        /// <summary>invalid flag passed, MMSYSERR_INVALFLAG</summary>
        InvalidFlag = 10,
        /// <summary>invalid parameter passed, MMSYSERR_INVALPARAM</summary>
        InvalidParameter = 11,
        /// <summary>handle being used simultaneously on another thread (eg callback),MMSYSERR_HANDLEBUSY</summary>
        HandleBusy = 12,
        /// <summary>specified alias not found, MMSYSERR_INVALIDALIAS</summary>
        InvalidAlias = 13,
        /// <summary>bad registry database, MMSYSERR_BADDB</summary>
        BadRegistryDatabase = 14,
        /// <summary>registry key not found, MMSYSERR_KEYNOTFOUND</summary>
        RegistryKeyNotFound = 15,
        /// <summary>registry read error, MMSYSERR_READERROR</summary>
        RegistryReadError = 16,
        /// <summary>registry write error, MMSYSERR_WRITEERROR</summary>
        RegistryWriteError = 17,
        /// <summary>registry delete error, MMSYSERR_DELETEERROR</summary>
        RegistryDeleteError = 18,
        /// <summary>registry value not found, MMSYSERR_VALNOTFOUND</summary>
        RegistryValueNotFound = 19,
        /// <summary>driver does not call DriverCallback, MMSYSERR_NODRIVERCB</summary>
        NoDriverCallback = 20,
        /// <summary>more data to be returned, MMSYSERR_MOREDATA</summary>
        MoreData = 21,

        /// <summary>unsupported wave format, WAVERR_BADFORMAT</summary>
        WaveBadFormat = 32,
        /// <summary>still something playing, WAVERR_STILLPLAYING</summary>
        WaveStillPlaying = 33,
        /// <summary>header not prepared, WAVERR_UNPREPARED</summary>
        WaveHeaderUnprepared = 34,
        /// <summary>device is synchronous, WAVERR_SYNC</summary>
        WaveSync = 35,

        // ACM error codes, found in msacm.h

        /// <summary>Conversion not possible (ACMERR_NOTPOSSIBLE)</summary>
        AcmNotPossible = 512,
        /// <summary>Busy (ACMERR_BUSY)</summary>
        AcmBusy = 513,
        /// <summary>Header Unprepared (ACMERR_UNPREPARED)</summary>
        AcmHeaderUnprepared = 514,
        /// <summary>Cancelled (ACMERR_CANCELED)</summary>
        AcmCancelled = 515,

        // Mixer error codes, found in mmresult.h

        /// <summary>invalid line (MIXERR_INVALLINE)</summary>
        MixerInvalidLine = 1024,
        /// <summary>invalid control (MIXERR_INVALCONTROL)</summary>
        MixerInvalidControl = 1025,
        /// <summary>invalid value (MIXERR_INVALVALUE)</summary>
        MixerInvalidValue = 1026,
    }

}
