using System;
using System.Management;
using System.Text;
using Captura.Audio;
using Captura.ViewModels;
using Captura.Webcam;

namespace Captura.Models
{
    public static class SystemInfo
    {
        public static string GetInfo()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{nameof(Captura)} v{AboutViewModel.Version}");
            sb.AppendLine(OsInfo());
            sb.AppendLine($"{(Environment.Is64BitOperatingSystem ? 64 : 32)}-bit OS");
            sb.AppendLine($"{(Environment.Is64BitProcess ? 64 : 32)}-bit Process");
            sb.AppendLine($"{Environment.ProcessorCount} processor(s)");

            sb.AppendLine();
            sb.Append(CpuInfo());
            sb.Append(RamInfo());
            sb.Append(GpuInfo());

            var platformServices = ServiceProvider.Get<IPlatformServices>();

            try
            {
                sb.AppendLine($"Desktop: {platformServices.DesktopRectangle}");

                foreach (var screen in platformServices.EnumerateScreens())
                {
                    sb.AppendLine($"Screen: {screen.DeviceName}: {screen.Rectangle}");
                }
            }
            catch
            {
                sb.AppendLine("Unable to get screen dimensions");
            }

            var audioSource = ServiceProvider.Get<IAudioSource>();

            try
            {
                foreach (var mic in audioSource.Microphones)
                {
                    sb.AppendLine($"Mic: {mic.Name}");
                }

                foreach (var speaker in audioSource.Speakers)
                {
                    sb.AppendLine($"Speaker: {speaker.Name}");
                }
            }
            catch
            {
                sb.AppendLine("Unable to get audio devices");
            }

            try
            {
                var webcamSource = ServiceProvider.Get<IWebCamProvider>();

                foreach (var webcam in webcamSource.GetSources())
                {
                    sb.AppendLine($"Webcam: {webcam.Name}");
                }
            }
            catch
            {
                sb.AppendLine("Unable to get webcams");
            }

            sb.AppendLine();

            return sb.ToString();
        }

        static string DeviceInformation(string ClassName, string[] Properties)
        {
            var sb = new StringBuilder();

            foreach (var instance in new ManagementClass(ClassName).GetInstances())
            {
                foreach (var property in Properties)
                {
                    try
                    {
                        sb.AppendLine($"{property}: {instance.Properties[property].Value}");
                    }
                    catch
                    {
                        //Add codes to manage more information
                    }
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        static string CpuInfo()
        {
            return DeviceInformation("Win32_Processor", new[]
            {
                "Name",
                "NumberOfCores",
                "NumberOfLogicalProcessors"
            });
        }

        static string GpuInfo()
        {
            return DeviceInformation("Win32_VideoController", new[]
            {
                "Name",
                "AdapterRAM"
            });
        }

        static string OsInfo()
        {
            var devInfo = DeviceInformation("Win32_OperatingSystem", new[]
            {
                "Name"
            });

            return $"OS: {devInfo.Split(':')[1].Trim()}";
        }

        static string RamInfo()
        {
            return DeviceInformation("Win32_PhysicalMemory", new[]
            {
                "Name",
                "Capacity"
            });
        }
    }
}