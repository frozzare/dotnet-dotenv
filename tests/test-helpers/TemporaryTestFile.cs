using System;
using System.IO;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace TestHelper {

    public class TemporaryTestFile : IDisposable {
	const string FILENAME = "./.env";

	private string _FilenameUsed;

	public TemporaryTestFile(string body) {
		_FilenameUsed = MakeFileName();
		EnsureNoFile();
		CreateNewFile(body);
	}

	public void Dispose() {
		EnsureNoFile();			
	}

	public string Path { get { return _FilenameUsed; }}

	private string MakeFileName() {
		DateTime now = DateTime.Now;
		return FILENAME + $"-{now.Millisecond}";
	}

	private void CreateNewFile(string body) {
		using(StreamWriter writer = File.AppendText(_FilenameUsed)) {
			writer.Write(body);
			writer.Flush();
		}
	}
        
	private void EnsureNoFile() {
		if (File.Exists(_FilenameUsed)) {
			System.Console.WriteLine( $"Removing old {_FilenameUsed} file" );
			File.Delete(_FilenameUsed);
		}
	}
    }
}
