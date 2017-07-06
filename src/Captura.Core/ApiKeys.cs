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
        public static string ImgurClientId { get; } = Environment.GetEnvironmentVariable("imgur_client_id", EnvironmentVariableTarget.User) ?? "";
    }
}
