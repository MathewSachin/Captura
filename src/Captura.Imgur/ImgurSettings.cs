using System;

namespace Captura.Imgur
{
    public class ImgurSettings : PropertyStore
    {
        public bool Anonymous
        {
            get => Get(true);
            set => Set(value);
        }

        public string AccessToken
        {
            get => Get("");
            set => Set(value);
        }

        public string RefreshToken
        {
            get => Get("");
            set => Set(value);
        }

        public DateTime ExpiresAt
        {
            get => Get(DateTime.MinValue);
            set => Set(value);
        }

        /// <summary>
        /// Checks if the Token expires in the next 10 seconds.
        /// </summary>
        public bool IsExpired()
        {
            return DateTime.UtcNow + TimeSpan.FromSeconds(10) > ExpiresAt;
        }
    }
}