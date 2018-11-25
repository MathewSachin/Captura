#l "constants.cake"
#l "backup.cake"
using System.Text.RegularExpressions;

IEnumerable<string> GetVariables(string ApiKeysContent)
{
    var match = Regex.Match(ApiKeysContent, "Get\\(\"(.*)\"\\)");

    while (match.Success)
    {
        yield return match.Groups[1].ToString();

        match = match.NextMatch();
    }
}

void EmbedApiKeys()
{
    var apiKeysPath = sourceFolder + File("Captura.Core/ApiKeys.cs");

    Information("Embedding Api Keys from Environment Variables ...");

    CreateBackup(apiKeysPath, tempFolder + File("ApiKeys.cs"));

    var content = FileRead(apiKeysPath);

    foreach (var variable in GetVariables(content))
    {
        if (HasEnvironmentVariable(variable))
        {
            content = content.Replace($"Get(\"{variable}\")", $"\"{EnvironmentVariable(variable)}\"");
        }
    }

    FileWrite(apiKeysPath, content);
}