using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace TestHelper
{
    partial class MockFileProvider
    {
		internal class MockDirectoryContents : IDirectoryContents
		{
            private bool _IndicateExistence;
			public MockDirectoryContents(bool doesExist)
			{
                _IndicateExistence = doesExist;
			}

			bool IDirectoryContents.Exists => _IndicateExistence;

			IEnumerator<IFileInfo> IEnumerable<IFileInfo>.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}
		}
        
    }
}