namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public sealed class DeleteFileTransaction : Transaction
    {
        private const string BackupExtension = ".bak";
        private readonly string _filePath;

        public static DeleteFileTransaction Create(string filePath) =>
            new DeleteFileTransaction(filePath);

        public DeleteFileTransaction(string filePath)
        {
            _filePath = filePath;
        }

        protected override bool OnApply() => FileUtils.MoveFile(_filePath, _filePath + BackupExtension);

        protected override void OnRevert() => FileUtils.MoveFile(_filePath + BackupExtension, _filePath);

        protected override void OnCommit()
        {
            if (Applied)
            {
                FileUtils.DeleteFile(_filePath + BackupExtension);
            }
        }
    }
}
