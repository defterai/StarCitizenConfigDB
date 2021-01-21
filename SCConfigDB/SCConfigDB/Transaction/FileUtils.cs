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

        public static bool MoveDirecory(string sourceDirName, string destDirName)
        {
            try
            {
                Directory.Move(sourceDirName, destDirName);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static bool DeleteDirectory(string path, bool recursive)
        {
            try
            {
                Directory.Delete(path, recursive);
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
