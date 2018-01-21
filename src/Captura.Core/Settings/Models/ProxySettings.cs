using System.Net;

namespace Captura
{
    public class ProxySettings : PropertyStore
    {
        public WebProxy GetWebProxy()
        {
            if (!Enabled)
                return null;

            var proxy = new WebProxy(Host, Port);

            if (Authenticate)
            {
                proxy.Credentials = new NetworkCredential(UserName, Password);
            }

            return proxy;
        }

        public bool Enabled
        {
            get => Get<bool>();
            set => Set(value);
        }

        public string Host
        {
            get => Get("");
            set => Set(value);
        }

        public int Port
        {
            get => Get<int>();
            set => Set(value);
        }

        public bool Authenticate
        {
            get => Get<bool>();
            set => Set(value);
        }

        public string UserName
        {
            get => Get("");
            set => Set(value);
        }

        public string Password
        {
            get => Get("");
            set => Set(value);
        }
    }
}
