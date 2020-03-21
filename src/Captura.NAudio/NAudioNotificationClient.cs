using System;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace Captura.Audio
{
    class NAudioNotificationClient : IMMNotificationClient
    {
        void InvokeDevicesUpdated() => DevicesUpdated?.Invoke();

        public void OnDeviceStateChanged(string DeviceId, DeviceState NewState)
        {
            InvokeDevicesUpdated();
        }

        public void OnDeviceAdded(string DeviceId)
        {
            InvokeDevicesUpdated();
        }

        public void OnDeviceRemoved(string DeviceId)
        {
            InvokeDevicesUpdated();
        }

        public void OnDefaultDeviceChanged(DataFlow Flow, Role Role, string DefaultDeviceId)
        {
            InvokeDevicesUpdated();
        }

        public void OnPropertyValueChanged(string DeviceId, PropertyKey Key)
        {
        }

        public event Action DevicesUpdated;
    }
}