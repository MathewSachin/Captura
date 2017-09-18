$params = @{
    'PackageName' = 'Captura';
    'Url' = "https://github.com/MathewSachin/Captura/releases/download/$tag/Captura-Setup.exe";
    'SilentArgs' = '/VERYSILENT /NOREBOOT /SP';
    'Checksum' = $checksum;
    'ChecksumType' = 'sha256';
};

Install-ChocolateyPackage @params