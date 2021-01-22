using System;

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
            if (!FileUtils.MoveFile(FilePath, FilePath + BackupExtension))
            {
                LastApplyException = FileUtils.LastException;
                return false;
            }
            if (!SaveToFile(Node, FilePath + TemporaryExtension))
            {
                FileUtils.MoveFile(FilePath + BackupExtension, FilePath);
                FileUtils.DeleteFile(FilePath + TemporaryExtension);
                return false;
            }
            return true;
        }

        protected override void OnRevert() => FileUtils.MoveFile(FilePath + BackupExtension, FilePath);

        protected override void OnCommit()
        {
            if (Applied)
            {
                FileUtils.DeleteFile(FilePath + BackupExtension);
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
