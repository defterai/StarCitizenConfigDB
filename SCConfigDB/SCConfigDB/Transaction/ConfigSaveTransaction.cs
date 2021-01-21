namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public sealed class ConfigSaveTransaction : Transaction
    {
        private const string TemporaryExtension = ".tmp";
        private const string BackupExtension = ".bak";
        private readonly object _node;
        private readonly string _filePath;

        public static ConfigSaveTransaction Create(object node, string filePath) =>
            new ConfigSaveTransaction(node, filePath);

        public ConfigSaveTransaction(object node, string filePath)
        {
            _node = node;
            _filePath = filePath;
        }

        protected override bool OnApply()
        {
            if (!FileUtils.MoveFile(_filePath, _filePath + BackupExtension))
            {
                return false;
            }
            if (!SaveToFile(_node, _filePath + TemporaryExtension))
            {
                FileUtils.MoveFile(_filePath + BackupExtension, _filePath);
                FileUtils.DeleteFile(_filePath + TemporaryExtension);
                return false;
            }
            return true;
        }

        protected override void OnRevert() => FileUtils.MoveFile(_filePath + BackupExtension, _filePath);

        protected override void OnCommit()
        {
            if (Applied)
            {
                FileUtils.DeleteFile(_filePath + BackupExtension);
            }
        }

        private static bool SaveToFile(object node, string path)
        {
            try
            {
                ConfigDatabase.SaveToFile(node, path);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
