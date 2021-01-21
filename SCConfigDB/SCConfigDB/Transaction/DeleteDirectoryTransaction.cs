using System.IO;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public sealed class DeleteDirectoryTransaction : Transaction
    {
        private string? _backupDirPath;
        public string DirectoryPath { get; }
        public bool Recursive { get; }

        public static DeleteDirectoryTransaction Create(string path, bool recursive) =>
            new DeleteDirectoryTransaction(path, recursive);

        public DeleteDirectoryTransaction(string path, bool recursive)
        {
            DirectoryPath = path;
            Recursive = recursive;
        }

        protected override bool OnApply()
        {
            string parentDirPath = Path.GetDirectoryName(DirectoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (parentDirPath != null)
            {
                string backupDirPath = Path.Combine(parentDirPath, Path.GetRandomFileName());
                if (FileUtils.MoveDirecory(DirectoryPath, backupDirPath))
                {
                    _backupDirPath = backupDirPath;
                    return true;
                }
            }
            return false;
        }

        protected override void OnRevert()
        {
            if (_backupDirPath != null)
            {
                FileUtils.MoveDirecory(_backupDirPath, DirectoryPath);
                _backupDirPath = null;
            }
        }

        protected override void OnCommit()
        {
            if (_backupDirPath != null)
            {
                FileUtils.DeleteDirectory(_backupDirPath, Recursive);
                _backupDirPath = null;
            }
        }
    }
}
