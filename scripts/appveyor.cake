#l "constants.cake"
#l "choco.cake"

class Artifact
{
    public Artifact(string Name, string Path)
    {
        this.Name = Name;
        this.Path = Path;
    }

    public string Name { get; }
    public string Path { get; }
}

readonly var artifacts = new []
{
    new Artifact("Portable", PortablePath),
    new Artifact("Setup", SetupPath),
    new Artifact("Chocolatey", ChocoPkgPath)
};

void UploadArtifacts()
{
    foreach (var artifact in artifacts)
    {
        if (FileExists(artifact.Path))
        {
            AppVeyor.UploadArtifact(artifact.Path, new AppVeyorUploadArtifactsSettings
            {
                DeploymentName = artifact.Name
            });
        }
    }
}