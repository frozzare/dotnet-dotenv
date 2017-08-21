using System;
using System.IO;
using System.Text;
using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System.Collections;
using System.Collections.Generic;

namespace TestHelper
{

	public partial class MockFileProvider : IDisposable, IFileProvider
	{
		const string FILENAME = "./.env.NeverToResolve";
        const long LONG_TIME = 1000000;

        private string _Content;
        private bool _FileExists;
        private bool _FileChanged;
        private MockChangeToken _ChangeToken;
        private DateTimeOffset _LastChanged;
        private Action _ChangeCallback;
        private Action _UsageAction;

		public MockFileProvider(string body)
		{
            _UsageAction = () => {};
            _Content = body;
            _FileExists = true;
            _FileChanged = false;
            _LastChanged = DateTimeOffset.FromFileTime( LONG_TIME );
            _ChangeToken = new MockChangeToken(this);
		}

		public void Dispose()
		{
		}

        public void SetUsageAction(Action action)
        {
            _UsageAction = action;
        }

		public string Path { get { ObserveUsage(); return FILENAME; } }

        public MockFileProvider PretendFileExists(bool state)
        {
            _FileExists = state;
            return this;
        }

        public MockFileProvider PretendFileChanged(bool state)
        {
            if (state) {
                _LastChanged = DateTimeOffset.Now;
                _FileChanged = true;
                NotifyChanges();
            }
            return this;
        }

		IFileInfo IFileProvider.GetFileInfo(string subpath)
		{
		    return new MockFileInfo(this);
		}

		IDirectoryContents IFileProvider.GetDirectoryContents(string subpath)
		{
			throw new NotImplementedException();
		}

		IChangeToken IFileProvider.Watch(string filter)
		{
			return new MockChangeToken(this);
		}

        private void AddChangeListener(Action listener) {
            _ChangeCallback += listener;
        }

        private void NotifyChanges() {
            _ChangeCallback();
        }

        private void ObserveUsage()
        {
            _UsageAction();
        }
	}

}