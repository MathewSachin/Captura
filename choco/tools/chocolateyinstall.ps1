$response = Invoke-WebRequest https://api.github.com/repos/MathewSachin/Captura/releases/latest

$release = ConvertFrom-Json $response

$tag = $release.tag_name

$params = @{
    'PackageName' = 'Captura';
    'Url' = "https://github.com/MathewSachin/Captura/releases/download/$tag/Setup.exe";
    'SilentArgs' = '--silent'
};

Install-ChocolateyPackage @params