using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlScriptPackager.Core.Configuration;
using System.Configuration;
using System.Web.Configuration;

namespace SqlScriptPackager.Core
{
    public static class ScriptProviderManager
    {
        private const string PROVIDER_SECTION_NAME = "scriptproviders";

        private static readonly ScriptProviderCollection _providers = new ScriptProviderCollection();

        public static ScriptProviderCollection Providers
        {
            get { return _providers; }
        }

        static ScriptProviderManager()
        {
            var config = ConfigurationManager.GetSection(PROVIDER_SECTION_NAME) as ScriptProviderConfiguration;

            if (config == null)
                throw new ConfigurationErrorsException("Error reading '" + PROVIDER_SECTION_NAME + "' script providers configuration.");

            ProvidersHelper.InstantiateProviders(config.Providers, _providers, typeof(ScriptProvider));
            _providers.SetReadOnly();
        }
    }
}
