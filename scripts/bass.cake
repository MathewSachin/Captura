#l "constants.cake"

// Returns path to bass.dll
string RestoreBass()
{
    var bass = tempFolder + File("bass/bass.dll");

    if (!FileExists(bass))
    {
        Information("Downloading BASS...");

        var bassZipPath =  tempFolder + File("bass.zip");
        const string bassUrl = "http://www.un4seen.com/files/bass24.zip";

        DownloadFile(bassUrl, bassZipPath);

        Information("Extracting BASS ...");

        Unzip(bassZipPath, tempFolder + Directory("bass"));
    }

    return bass;
}

// Returns path to bassmix.dll
string RestoreBassMix()
{
    var bassmix = tempFolder + File("bassmix/bassmix.dll");

    if (!FileExists(bassmix))
    {
        Information("Downloading BASSmix...");

        var bassMixZipPath =  tempFolder + File("bassmix.zip");
        const string bassMixUrl = "http://www.un4seen.com/files/bassmix24.zip";

        DownloadFile(bassMixUrl, bassMixZipPath);

        Information("Extracting BASSmix...");

        Unzip(bassMixZipPath, tempFolder + Directory("bassmix"));
    }

    return bassmix;
}