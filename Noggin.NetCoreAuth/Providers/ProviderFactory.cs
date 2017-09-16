﻿using Noggin.NetCoreAuth.Config;
using Noggin.NetCoreAuth.Exceptions;
using Noggin.NetCoreAuth.Providers.Twitter;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Extensions.Options;

namespace Noggin.NetCoreAuth.Providers
{
    public class ProviderFactory : IProviderFactory
    {
        private readonly IList<ProviderConfig> _providerConfigs;
        private readonly IDictionary<string, Provider> _providers;
        private readonly string _defaultRedirectTemplate;
        private readonly string _defaultCallbackTemplate;

        public ProviderFactory(IOptions<AuthConfigSection> config)
        {
            _providerConfigs = config.Value.Providers;
            _providers = new Dictionary<string, Provider>();

            _defaultRedirectTemplate = CreateDefaultTemplate(config.Value.DefaultRedirectTemplate, "auth/redirect/{provider}");
            _defaultCallbackTemplate = CreateDefaultTemplate(config.Value.DefaultCallbackTemplate, "auth/callbackback/{provider}");

            Providers = new List<Provider>();
            foreach(var provider in config.Value.Providers)
            {
                Providers.Add(Get(provider.Name));
            }

            // Thought: Lazy Dictionary, or simplyfy getting providers as all pre initted
        }

        public Provider Get(string name)
        {
            switch(name.ToLower())
            {
                case "twitter":
                    return Get(name, (x) => new TwitterProvider(x, _defaultRedirectTemplate, _defaultCallbackTemplate));
                default:
                    throw new NogginNetCoreConfigException($"No provider called {name} found");
            }
        }

        private Provider Get(string name, Func<ProviderConfig, Provider> createProvider)
        {
            if (_providers.ContainsKey(name))
            {
                return _providers[name];
            }

            var config = _providerConfigs.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
            var provider = createProvider(config);
            _providers[name] = provider;
            return provider;
        }

        public IList<Provider> Providers { get; }

        private static string CreateDefaultTemplate(string template1, string template2)
        {
            if (string.IsNullOrEmpty(template1)) return template2;

            if (template1.Contains("{provider}")) return template1;

            throw new NogginNetCoreAuthException("Default Url Templates must contain '{provider}' for the provider name.");
        }

        
    }

    public interface IProviderFactory
    {
        Provider Get(string name);
        IList<Provider> Providers { get; }
    }
}
