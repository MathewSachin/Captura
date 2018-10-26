#l "constants.cake"
#l "backup.cake"

void EmbedApiKeys()
{
    var apiKeysPath = sourceFolder + File("Captura.Core/ApiKeys.cs");

    var variables = new [] { "imgur_client_id" };

    Information("Embedding Api Keys from Environment Variables ...");

    CreateBackup(apiKeysPath, tempFolder + File("ApiKeys.cs"));

    var content = FileRead(apiKeysPath);

    foreach (var variable in variables)
    {
        if (HasEnvironmentVariable(variable))
        {
            content = content.Replace($"Get(\"{variable}\")", $"\"{EnvironmentVariable(variable)}\"");
        }
    }

    FileWrite(apiKeysPath, content);
}