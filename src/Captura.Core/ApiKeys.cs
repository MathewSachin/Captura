using System;

namespace Captura
{
    /// <summary>
    /// Holds Api Keys.
    /// On Development builds, Api Keys are retrieved from User Environment variables.
    /// On Production builds, AppVeyor embeds Api Keys into the app.
    /// </summary>
    static class ApiKeys
    {
        static string Get(string Key) => Environment.GetEnvironmentVariable(Key, EnvironmentVariableTarget.User) ?? "";

        public static string ImgurClientId { get; } = Get("imgur_client_id");
    }
}
