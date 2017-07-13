using System.Configuration;
using System.Net;

namespace Captura
{
    public partial class Settings
    {
        public WebProxy GetWebProxy()
        {
            if (!UseProxy)
                return null;

            var proxy = new WebProxy(ProxyHost, ProxyPort);

            if (UseProxyAuth)
            {
                proxy.Credentials = new NetworkCredential(ProxyUserName, ProxyPassword);
            }

            return proxy;
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool UseProxy
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ProxyHost
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int ProxyPort
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool UseProxyAuth
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ProxyUserName
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("")]
        public string ProxyPassword
        {
            get => Get<string>();
            set => Set(value);
        }
    }
}
