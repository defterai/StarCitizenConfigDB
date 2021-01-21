namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public sealed class DeleteFileTransaction : Transaction
    {
        private const string BackupExtension = ".bak";
        public string FilePath { get; }

        public static DeleteFileTransaction Create(string filePath) =>
            new DeleteFileTransaction(filePath);

        public DeleteFileTransaction(string filePath)
        {
            FilePath = filePath;
        }

        protected override bool OnApply() => FileUtils.MoveFile(FilePath, FilePath + BackupExtension);

        protected override void OnRevert() => FileUtils.MoveFile(FilePath + BackupExtension, FilePath);

        protected override void OnCommit()
        {
            if (Applied)
            {
                FileUtils.DeleteFile(FilePath + BackupExtension);
            }
        }
    }
}
