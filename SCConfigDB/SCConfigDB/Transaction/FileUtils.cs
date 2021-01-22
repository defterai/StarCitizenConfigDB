using System;
using System.IO;
using System.Threading;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public static class FileUtils
    {
        private static readonly ThreadLocal<Exception?> _lastThreadException = new ThreadLocal<Exception?>();
        public static Exception? LastException
        {
            get => _lastThreadException.Value;
            set => _lastThreadException.Value = value;
        }

        public static string? GetParentDirPath(string path) =>
            Path.GetDirectoryName(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));

        public static bool MoveFile(string sourceFileName, string destinationFileName)
        {
            try
            {
                if (File.Exists(destinationFileName))
                    File.Delete(destinationFileName);
                File.Move(sourceFileName, destinationFileName);
            }
            catch (Exception e)
            {
                _lastThreadException.Value = e;
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
            catch (Exception e)
            {
                _lastThreadException.Value = e;
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
            catch (Exception e)
            {
                _lastThreadException.Value = e;
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
            catch (Exception e)
            {
                _lastThreadException.Value = e;
                return false;
            }
            return true;
        }
    }
}
