using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BCS.Framework.Commons.Mapping
{
    /// <author>Yazeed Hamdan</author>
    /// <summary>
    /// Responsible of instantiating each provider and cahing it into a Dictionary
    /// </summary>
    internal static class ProviderFactory
    {
        //Static providers cache
        static IDictionary<string, IPropertyMappingProvider> providers = new Dictionary<string, IPropertyMappingProvider>();

        public static IPropertyMappingProvider CreatePropertyMappingProvider(Type providerType)             
        {
            IPropertyMappingProvider provider = null;
            
            //Check if the provider already exists in the cahce
            if (providers.ContainsKey(providerType.ToString()))
                provider = providers[providerType.ToString()] as IPropertyMappingProvider;
            else
            {
                //Instaniate a new provider and add it to the cache
                provider=  AppDomain.CurrentDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().ToString(), providerType.ToString()) as IPropertyMappingProvider;
                providers.Add(provider.GetType().ToString(), provider as IPropertyMappingProvider);
            }
            return provider;
        }
    }
}