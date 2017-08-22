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
        const int WAIT_TIME = 10;

        private string _FilenameUsed;
        private string _Content;

        public TemporaryTestFile(string body)
        {
            _FilenameUsed = MakeFileName();
            _Content = body;
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
                Thread.Sleep(WAIT_TIME);
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
            // Open temporary for write and overwrite if file exists.
            // Don't append as that may result in an unknown configuration
            // state.
            using (StreamWriter writer = File.CreateText(_FilenameUsed) )
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

