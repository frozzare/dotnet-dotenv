
using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace TestHelper
{
    partial class MockFileProvider
    {
        internal class MockChangeToken : IChangeToken, IDisposable
        {
            private MockFileProvider _Provider;
            public MockChangeToken(MockFileProvider provider)
            {
                _Provider = provider;
            }

            bool IChangeToken.HasChanged => _Provider._FileChanged;

            bool IChangeToken.ActiveChangeCallbacks => false;

            void IDisposable.Dispose()
            {
            }

            IDisposable IChangeToken.RegisterChangeCallback(Action<object> callback, object state)
            {
                _Provider.AddChangeListener( () => callback(state));
                return this;
            }
        }
    }
}