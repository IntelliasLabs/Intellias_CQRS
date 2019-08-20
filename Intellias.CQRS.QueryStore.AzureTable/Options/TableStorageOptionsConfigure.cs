using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Intellias.CQRS.QueryStore.AzureTable.Options
{
    public class TableStorageOptionsConfigure<TSource> : IOptionsChangeTokenSource<TableStorageOptions>, IConfigureOptions<TableStorageOptions>
    {
        private readonly IOptionsMonitor<TSource> optionsSource;
        private readonly Action<TSource, TableStorageOptions> configure;

        public TableStorageOptionsConfigure(IOptionsMonitor<TSource> optionsSource, Action<TSource, TableStorageOptions> configure)
        {
            this.optionsSource = optionsSource;
            this.configure = configure;
        }

        public string Name => Microsoft.Extensions.Options.Options.DefaultName;

        public IChangeToken GetChangeToken()
        {
            var changeToken = new ConfigurationReloadToken();
            this.optionsSource.OnChange(_ => changeToken.OnReload());
            return changeToken;
        }

        public void Configure(TableStorageOptions options)
        {
            this.configure(this.optionsSource.CurrentValue, options);
        }
    }
}