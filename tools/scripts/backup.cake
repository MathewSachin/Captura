static List<Backup> Backups { get; } = new List<Backup>();

class Backup
{
    public Backup(string OriginalPath, string BackupPath)
    {
        this.OriginalPath = OriginalPath;
        this.BackupPath = BackupPath;
    }

    public string OriginalPath { get; }

    public string BackupPath { get; }
}

void CreateBackup(string OriginalPath, string BackupPath)
{
    var backup = new Backup(OriginalPath, BackupPath);
    CopyFile(OriginalPath, BackupPath);
    Backups.Add(backup);
}

void RestoreBackups()
{
    Backups.ForEach(M =>
    {
        DeleteFile(M.OriginalPath);
        MoveFile(M.BackupPath, M.OriginalPath);
    });
}