using System;
using Microsoft.Extensions.Options;

namespace Intellias.CQRS.Tests.Fakes
{
    public class OptionsMonitorFake<TOptions> : IOptionsMonitor<TOptions>
    {
        private readonly TOptions options;

        public OptionsMonitorFake(TOptions options)
        {
            this.options = options;
        }

        public TOptions CurrentValue => options;

        public TOptions Get(string name) => options;

        public IDisposable OnChange(Action<TOptions, string> listener) => new DisposableStub();

        private class DisposableStub : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}