using System;
using System.IO;
using System.Text;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace TestHelper
{
    partial class MockFileProvider
    {
        internal class MockFileInfo : IFileInfo
		{
            private MockFileProvider _Provider;

			public MockFileInfo(MockFileProvider provider)
			{
                _Provider = provider;
			}

			bool IFileInfo.Exists => _Provider._FileExists;

			long IFileInfo.Length => _Provider._Content.Length;

			string IFileInfo.PhysicalPath => throw new NotImplementedException();

			string IFileInfo.Name => MockFileProvider.FILENAME;

			DateTimeOffset IFileInfo.LastModified => _Provider._LastChanged;

			bool IFileInfo.IsDirectory => false;

			Stream IFileInfo.CreateReadStream()
			{
                return MakeStreamFromContent();
			}

            private MemoryStream MakeStreamFromContent()
            {
                byte[] contentBytes = Encoding.ASCII.GetBytes( _Provider._Content );
                return new MemoryStream( contentBytes );
            }
		}
        
    }
}