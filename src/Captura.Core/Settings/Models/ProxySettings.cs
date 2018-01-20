using System.Net;

namespace Captura
{
    public class ProxySettings : NotifyPropertyChanged
    {
        bool _enabled, _auth;

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
            get => _enabled;
            set
            {
                _enabled = value;
                
                OnPropertyChanged();
            }
        }

        public string Host { get; set; }
        
        public int Port { get; set; }

        public bool Authenticate
        {
            get => _auth;
            set
            {
                _auth = value;
                
                OnPropertyChanged();
            }
        }
        
        public string UserName { get; set; }
        
        public string Password { get; set; }
    }
}
