using System;
using System.IO;

namespace LocationTrackingAPI.Models
{
    public class LogHelper
    {
        string _strFielPath = string.Empty;

        public LogHelper(string strFielPath)
        {
            _strFielPath = strFielPath;
        }
        public void WriteFileToLocal_old(string strError, string strAlias = "Error-")
        {
            try
            {
                string strDirectoryPath = _strFielPath == "" ? AppDomain.CurrentDomain.BaseDirectory + "Logs/" :
                        _strFielPath + "Logs/";
                if (!Directory.Exists(strDirectoryPath))
                    Directory.CreateDirectory(strDirectoryPath);

                string strFileName = strAlias + DateTime.Now.ToString("yyyyMMddHHmmsstt") + ".txt";
                string strFilePath = strDirectoryPath + strFileName;
                File.WriteAllText(strFilePath.Trim(), strError);

            }
            catch (Exception ex)
            {
                //WriteFileToLocal(ex.ToString());
            }
        }

        public void WriteFileToLocal(string strError, string strAlias = "Error-", bool isInternal = false)
        {
            try
            {
                string strDirectoryPath = string.IsNullOrWhiteSpace(_strFielPath)
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs")
                    : Path.Combine(_strFielPath, "Logs");

                if (!Directory.Exists(strDirectoryPath))
                    Directory.CreateDirectory(strDirectoryPath);

                // Auto-delete logs older than 7 days
                DeleteOldLogs(strDirectoryPath, 7);

                string fileName = $"{strAlias}{DateTime.Now:yyyyMMdd_HHmmss_fff}.txt";
                string filePath = Path.Combine(strDirectoryPath, fileName);

                File.WriteAllText(filePath, strError);
            }
            catch (Exception ex)
            {
                if (!isInternal)
                {
                    // Avoid infinite recursion
                    WriteFileToLocal("Internal logging failure: " + ex.Message, "Fatal-", true);
                }
            }
        }

        public void DeleteOldLogs(string logDirectory, int daysToKeep = 7)
        {
            try
            {
                if (!Directory.Exists(logDirectory))
                    return;

                var files = Directory.GetFiles(logDirectory, "*.txt");

                foreach (var file in files)
                {
                    var creation = File.GetCreationTime(file);

                    if (creation < DateTime.Now.AddDays(-daysToKeep))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Avoid breaking the main app if delete fails
            }
        }

    }
}