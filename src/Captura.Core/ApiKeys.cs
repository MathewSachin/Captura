using System;

namespace Captura
{
    /// <summary>
    /// Holds Api Keys.
    /// On Development builds, Api Keys are retrieved from User Environment variables.
    /// On Production builds, AppVeyor embeds Api Keys into the app.
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    class ApiKeys : IImagurApiKeys
    {
        static string Get(string Key) => Environment.GetEnvironmentVariable(Key, EnvironmentVariableTarget.User) ?? "";

        public string ImgurClientId => Get("imgur_client_id");
        
        public string ImgurSecret => Get("imgur_secret");

        public static string YouTubeClientId => Get("yt_client_id");

        public static string YouTubeClientSecret => Get("yt_client_secret");
    }
}
