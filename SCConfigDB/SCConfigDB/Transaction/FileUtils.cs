using System.IO;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public static class FileUtils
    {
        public static bool MoveFile(string sourceFileName, string destinationFileName)
        {
            try
            {
                if (File.Exists(destinationFileName))
                    File.Delete(destinationFileName);
                File.Move(sourceFileName, destinationFileName);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
