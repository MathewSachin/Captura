using System.Net;

namespace Captura
{
    public enum ProxyType
    {
        None,
        System,
        Manual
    }

    public class ProxySettings : PropertyStore
    {
        public IWebProxy GetWebProxy()
        {
            if (Type == ProxyType.None)
                return null;

            IWebProxy proxy;

            if (Type == ProxyType.Manual && !string.IsNullOrWhiteSpace(Host) && Port > 0)
            {
                proxy = new WebProxy(Host, Port);
            }
            else proxy = WebRequest.GetSystemWebProxy();

            if (Authenticate && !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
            {
                proxy.Credentials = new NetworkCredential(UserName, Password);
            }

            return proxy;
        }

        public ProxyType Type
        {
            get => Get(ProxyType.System);
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
