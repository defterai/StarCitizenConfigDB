using System;
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
            string? parentDirPath = FileUtils.GetParentDirPath(DirectoryPath);
            if (parentDirPath == null)
            {
                LastApplyException = new ArgumentException("Invalid delete directory path: " + DirectoryPath);
                return false;
            }
            string backupDirPath = Path.Combine(parentDirPath, Path.GetRandomFileName());
            if (!FileUtils.MoveDirecory(DirectoryPath, backupDirPath))
            {
                LastApplyException = FileUtils.LastException;
                return false;
            }
            _backupDirPath = backupDirPath;
            return true;
        }

        protected override void OnRevert()
        {
            if (_backupDirPath != null)
            {
#if DEBUG
                FileUtils.LastException = null;
#endif
                FileUtils.MoveDirecory(_backupDirPath, DirectoryPath);
#if DEBUG
                LastRevertException = FileUtils.LastException;
#endif
                _backupDirPath = null;
            }
        }

        protected override void OnCommit()
        {
            if (_backupDirPath != null)
            {
#if DEBUG
                FileUtils.LastException = null;
#endif
                FileUtils.DeleteDirectory(_backupDirPath, Recursive);
#if DEBUG
                LastCommitException = FileUtils.LastException;
#endif
                _backupDirPath = null;
            }
        }
    }
}
