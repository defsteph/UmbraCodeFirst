using System;
using System.Collections.Generic;
using System.Configuration;
using UmbraCodeFirst.Configuration.PageFactory;

namespace UmbraCodeFirst.Configuration
{
    public class UmbraCodeFirstConfiguration
    {
        private static readonly UmbraCodeFirstSection Config = ConfigurationManager.GetSection("umbraCodeFirst") as UmbraCodeFirstSection;

        public static IDictionary<string, Type> ConfiguredPageTypeMappings
        {
            get
            {
                var mappings = new Dictionary<string, Type>();

                if (Config == null || Config.PageFactory == null || !Config.PageFactory.Enabled)
                    return mappings;

                foreach (PageTypeMapElement mapping in Config.PageFactory.Mappings)
                {
                    Type type;
                    if (TryLoadType(mapping.Type, out type))
                    {
                        mappings.Add(mapping.NodeTypeAlias, type);
                    }
                    else
                    {
                        throw new TypeLoadException(
                            String.Format(
                                "The configured PageTypeMapping could not be loaded. Please check your configuration for NodeTypeAlias \"{0}\" with specified Type \"{1}\"",
                                mapping.NodeTypeAlias, mapping.Type));
                    }
                }
                return mappings;
            }
        }

        private static bool TryLoadType(string typeName, out Type type)
        {
            try
            {
                type = Type.GetType(typeName);
                return true;
            }
            catch
            {
                type = null;
                return false;
            }
        }

        public static bool EnableSynchronization
        {
            get
            {
                return Config == null || Config.Synchronization.Enabled;
            }
        }
    }
}