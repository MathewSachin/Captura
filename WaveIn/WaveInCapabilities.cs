using System;
using System.Runtime.InteropServices;

namespace NWaveIn
{
    [Flags]
    public enum SupportedWaveFormat
    {
        /// <summary>
        /// 11.025 kHz, Mono,   8-bit
        /// </summary>
        WAVE_FORMAT_1M08 = 0x00000001,
        /// <summary>
        /// 11.025 kHz, Stereo, 8-bit
        /// </summary>
        WAVE_FORMAT_1S08 = 0x00000002,
        /// <summary>
        /// 11.025 kHz, Mono,   16-bit
        /// </summary>
        WAVE_FORMAT_1M16 = 0x00000004,
        /// <summary>
        /// 11.025 kHz, Stereo, 16-bit
        /// </summary>
        WAVE_FORMAT_1S16 = 0x00000008,
        /// <summary>
        /// 22.05  kHz, Mono,   8-bit
        /// </summary>
        WAVE_FORMAT_2M08 = 0x00000010,
        /// <summary>
        /// 22.05  kHz, Stereo, 8-bit 
        /// </summary>
        WAVE_FORMAT_2S08 = 0x00000020,
        /// <summary>
        /// 22.05  kHz, Mono,   16-bit
        /// </summary>
        WAVE_FORMAT_2M16 = 0x00000040,
        /// <summary>
        /// 22.05  kHz, Stereo, 16-bit
        /// </summary>
        WAVE_FORMAT_2S16 = 0x00000080,
        /// <summary>
        /// 44.1   kHz, Mono,   8-bit 
        /// </summary>
        WAVE_FORMAT_4M08 = 0x00000100,
        /// <summary>
        /// 44.1   kHz, Stereo, 8-bit 
        /// </summary>
        WAVE_FORMAT_4S08 = 0x00000200,
        /// <summary>
        /// 44.1   kHz, Mono,   16-bit
        /// </summary>
        WAVE_FORMAT_4M16 = 0x00000400,
        /// <summary>
        ///  44.1   kHz, Stereo, 16-bit
        /// </summary>
        WAVE_FORMAT_4S16 = 0x00000800,

        /// <summary>
        /// 44.1   kHz, Mono,   8-bit 
        /// </summary>
        WAVE_FORMAT_44M08 = 0x00000100,
        /// <summary>
        /// 44.1   kHz, Stereo, 8-bit 
        /// </summary>
        WAVE_FORMAT_44S08 = 0x00000200,
        /// <summary>
        /// 44.1   kHz, Mono,   16-bit
        /// </summary>
        WAVE_FORMAT_44M16 = 0x00000400,
        /// <summary>
        /// 44.1   kHz, Stereo, 16-bit
        /// </summary>
        WAVE_FORMAT_44S16 = 0x00000800,
        /// <summary>
        /// 48     kHz, Mono,   8-bit 
        /// </summary>
        WAVE_FORMAT_48M08 = 0x00001000,
        /// <summary>
        ///  48     kHz, Stereo, 8-bit
        /// </summary>
        WAVE_FORMAT_48S08 = 0x00002000,
        /// <summary>
        /// 48     kHz, Mono,   16-bit
        /// </summary>
        WAVE_FORMAT_48M16 = 0x00004000,
        /// <summary>
        /// 48     kHz, Stereo, 16-bit
        /// </summary>
        WAVE_FORMAT_48S16 = 0x00008000,
        /// <summary>
        /// 96     kHz, Mono,   8-bit 
        /// </summary>
        WAVE_FORMAT_96M08 = 0x00010000,
        /// <summary>
        /// 96     kHz, Stereo, 8-bit
        /// </summary>
        WAVE_FORMAT_96S08 = 0x00020000,
        /// <summary>
        /// 96     kHz, Mono,   16-bit
        /// </summary>
        WAVE_FORMAT_96M16 = 0x00040000,
        /// <summary>
        /// 96     kHz, Stereo, 16-bit
        /// </summary>
        WAVE_FORMAT_96S16 = 0x00080000,

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WaveInCapabilities
    {
        /// <summary>
        /// wMid
        /// </summary>
        private short manufacturerId;
        /// <summary>
        /// wPid
        /// </summary>
        private short productId;
        /// <summary>
        /// vDriverVersion
        /// </summary>
        private int driverVersion;
        /// <summary>
        /// Product Name (szPname)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLength)]
        private string productName;
        /// <summary>
        /// Supported formats (bit flags) dwFormats 
        /// </summary>
        private SupportedWaveFormat supportedFormats;
        /// <summary>
        /// Supported channels (1 for mono 2 for stereo) (wChannels)
        /// Seems to be set to -1 on a lot of devices
        /// </summary>
        private short channels;
        /// <summary>
        /// wReserved1
        /// </summary>
        private short reserved;

        // extra WAVEINCAPS2 members
        private Guid manufacturerGuid;
        private Guid productGuid;
        private Guid nameGuid;

        private const int MaxProductNameLength = 32;

        /// <summary>
        /// Number of channels supported
        /// </summary>
        public int Channels
        {
            get
            {
                return channels;
            }
        }

        /// <summary>
        /// The product name
        /// </summary>
        public string ProductName
        {
            get
            {
                return productName;
            }
        }

        /// <summary>
        /// The device name Guid (if provided)
        /// </summary>
        public Guid NameGuid { get { return nameGuid; } }
        /// <summary>
        /// The product name Guid (if provided)
        /// </summary>
        public Guid ProductGuid { get { return productGuid; } }
        /// <summary>
        /// The manufacturer guid (if provided)
        /// </summary>
        public Guid ManufacturerGuid { get { return manufacturerGuid; } }

        /// <summary>
        /// Checks to see if a given SupportedWaveFormat is supported
        /// </summary>
        /// <param name="waveFormat">The SupportedWaveFormat</param>
        /// <returns>true if supported</returns>
        public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
        {
            return (supportedFormats & waveFormat) == waveFormat;
        }

    }
}
