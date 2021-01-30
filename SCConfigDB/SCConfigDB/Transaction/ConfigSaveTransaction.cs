using System;
using System.IO;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public sealed class ConfigSaveTransaction : Transaction
    {
        private const string TemporaryExtension = ".tmp";
        private const string BackupExtension = ".bak";
        public object Node { get; }
        public string FilePath { get; }

        public static ConfigSaveTransaction Create(object node, string filePath) =>
            new ConfigSaveTransaction(node, filePath);

        public ConfigSaveTransaction(object node, string filePath)
        {
            Node = node;
            FilePath = filePath;
        }

        protected override bool OnApply()
        {
            if (!FileUtils.MoveFileIfExist(FilePath, FilePath + BackupExtension))
            {
                LastApplyException = FileUtils.LastException;
                return false;
            }
            if (!SaveToFile(Node, FilePath + TemporaryExtension))
            {
                FileUtils.MoveFileIfExist(FilePath + BackupExtension, FilePath);
                FileUtils.DeleteFileIfExist(FilePath + TemporaryExtension);
                return false;
            }
            if (!FileUtils.MoveFile(FilePath + TemporaryExtension, FilePath))
            {
                LastApplyException = FileUtils.LastException;
                return false;
            }
            return true;
        }

        protected override void OnRevert()
        {
#if DEBUG
            FileUtils.LastException = null;
#endif
            FileUtils.MoveFileIfExist(FilePath + BackupExtension, FilePath);
#if DEBUG
            LastRevertException = FileUtils.LastException;
#endif
        }

        protected override void OnCommit()
        {
            if (Applied)
            {
#if DEBUG
                FileUtils.LastException = null;
#endif
                FileUtils.DeleteFileIfExist(FilePath + BackupExtension);
#if DEBUG
                LastCommitException = FileUtils.LastException;
#endif
            }
        }

        private bool SaveToFile(object node, string path)
        {
            try
            {
                ConfigDatabase.SaveToFile(node, path);
            }
            catch (Exception e)
            {
                LastApplyException = e;
                return false;
            }
            return true;
        }
    }
}
