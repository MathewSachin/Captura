# Running this script on Pull Requests causes failure due to usage of secure environment variable
if (-not $env:APPVEYOR_PULL_REQUEST_NUMBER)
{
    Set-Content src/Captura.Core/ApiKeys.cs $env:api_keys
}