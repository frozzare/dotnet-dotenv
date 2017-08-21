using System;
using System.IO;
using System.Text;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace TestHelper
{

    public class TemporaryTestFile : IDisposable
    {
        const string FILENAME = "./.env";

        private string _FilenameUsed;
        private string _Content;

        public TemporaryTestFile(string body)
        {
            _FilenameUsed = MakeFileName();
            _Content = body;
            EnsureNoFile();
            CreateNewFile(body);
            WaitForFilesystemToCatchUpToMakeTestsMoreReliable();
        }

        public void Dispose()
        {
            EnsureNoFile();
        }

        public string Path { get { return _FilenameUsed; } }

        private void WaitForFilesystemToCatchUpToMakeTestsMoreReliable()
        {
            int ExcessiveWaitingLimit = 100;
            while (!File.Exists(_FilenameUsed))
            {
                Thread.Sleep(1);
                ExcessiveWaitingLimit--;
                if (ExcessiveWaitingLimit == 0)
                {
                    throw new Exception("System took too long to finish writing file - must be some other timing problem.");
                }
            }
        }

        private string MakeFileName()
        {
            DateTime now = DateTime.Now;
            return FILENAME + $"-{now.Minute}-{now.Second}-{now.Millisecond}";
        }

        private void CreateNewFile(string body)
        {
            using (StreamWriter writer = File.AppendText(_FilenameUsed))
            {
                writer.Write(body);
                writer.Flush();
            }
        }

        private void EnsureNoFile()
        {
            if (File.Exists(_FilenameUsed))
            {
                System.Console.WriteLine($"Removing old {_FilenameUsed} file");
                File.Delete(_FilenameUsed);
            }
        }
    }

}

