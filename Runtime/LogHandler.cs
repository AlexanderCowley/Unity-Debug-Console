using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace RuntimeDebugger
{
    public class LogHandler
    {
        string path;
        string _documentName;
        string _fileExtension;
        string _fileHeader;

        List<string> LogHistory = new List<string>();

        public LogHandler(string directoryName, string documentName, string fileExtension, string header)
        {
            this._documentName = documentName;
            this._fileExtension = fileExtension;
            this._fileHeader = header;

            path = Application.streamingAssetsPath + $"/{directoryName}/" + documentName + fileExtension;
            Directory.CreateDirectory(Application.streamingAssetsPath + $"/{directoryName}/" );
            GetFile();
        }

        public LogHandler(string documentName, string fileExtension)
        {
            this._documentName = documentName;
            this._fileExtension = fileExtension;
            path = Application.streamingAssetsPath + documentName + fileExtension;
            Directory.CreateDirectory(Application.streamingAssetsPath);
            File.WriteAllText(path, $"{_fileHeader}\n\n");
            GetFile();
        }

        void GetFile()
        {
            if (!File.Exists(path))
                File.WriteAllText(path, $"{_fileHeader}\n\n");
        }

        public void ReadTxtFile()
        {
            File.ReadAllText(path);
        }

        public void WriteToLog(string messageToAppend)
        {
            LogHistory.Add(messageToAppend);
            DateTime timeBuildStarted = DateTime.Now;
            File.AppendAllText(path, messageToAppend + " - " + timeBuildStarted + "\n\n");
        }
    }
}
