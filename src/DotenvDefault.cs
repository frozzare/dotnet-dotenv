using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Frozzare.Dotenv
{
    public class DotenvDefault
    {
        private static DotenvDefault _Instance = new DotenvDefault();
        private DotenvDefault() {
            InitFileProviderFinder();
        }

        public static DotenvDefault Instance
        {
            get { return _Instance; }
        } 

        private Func<IFileProvider> _FileProviderFinder;

        public IFileProvider GetFileProvider() {
            return _FileProviderFinder();
        }

        public void SetDefaultFileProvider(IFileProvider value) {
            _FileProviderFinder = () => value;
        }

        private void InitFileProviderFinder() {
            _FileProviderFinder = () => new PhysicalFileProvider(Directory.GetCurrentDirectory());
        }
    }
}