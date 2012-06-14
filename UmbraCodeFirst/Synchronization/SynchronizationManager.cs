
using System;

namespace UmbraCodeFirst.Synchronization
{
    public sealed class SynchronizationManager
    {
        #region Singleton

        private SynchronizationManager()
        {

        }

        private static SynchronizationManager _instance;

        /// <summary>
        /// Singleton instance of the SynchronizationManager
        /// </summary>
        public static SynchronizationManager Instance
        {
            get { return _instance ?? (_instance = new SynchronizationManager()); }
        }

        #endregion

        public void Synchronize()
        {
            if (SynchronizationDisabled)
                return;
            
            SynchronizeTemplates();
            SynchronizeDataTypeDefinitions();
            SynchronizeDocumentTypes();
            SynchronizeMacroPropertyTypes();
        }

        private static bool SynchronizationDisabled
        {
            get
            {
                var umbracoIsInstalled = !String.IsNullOrWhiteSpace(umbraco.GlobalSettings.ConfigurationStatus);
                var synchronizationEnabled = Configuration.UmbraCodeFirstConfiguration.EnableSynchronization;

                return !umbracoIsInstalled || !synchronizationEnabled;
            }
        }

        private static void SynchronizeMacroPropertyTypes()
        {
            MacroPropertyTypeSynchronizer.Instance.Synchronize();
        }

        private static void SynchronizeTemplates()
        {
            TemplateSynchronizer.Instance.Synchronize();
        }

        private static void SynchronizeDataTypeDefinitions()
        {
            DataTypeDefinitionSynchronizer.Instance.Synchronize();
        }

        private static void SynchronizeDocumentTypes()
        {
            DocumentTypeSynchronizer.Instance.Synchronize();
        }
        
    }
}