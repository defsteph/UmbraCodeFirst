using System;
using System.Collections.Generic;
using UmbraCodeFirst.Configuration;
using UmbraCodeFirst.Exceptions;
using umbraco.BusinessLogic.Utils;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.NodeFactory;

namespace UmbraCodeFirst.Factories
{
#pragma warning disable 612,618
    public class PageFactory : IPageFactory
    {
        private readonly IDictionary<string, Type> _pageTypeMap = new Dictionary<string, Type>();

        #region Singleton

        private PageFactory()
        {
            LoadInstalledPageTypesByReflection();
            LoadInstalledPageTypesFromConfiguration();
        }

        private void LoadInstalledPageTypesByReflection()
        {
            var inherentPageTypes = TypeFinder.FindClassesOfType<IPageBase>(true, true);
            foreach (var inherentPageType in inherentPageTypes)
            {
                var typeName = inherentPageType.Name;
                if (!_pageTypeMap.ContainsKey(typeName))
                    _pageTypeMap.Add(typeName, inherentPageType);
            }
        }

        private void LoadInstalledPageTypesFromConfiguration()
        {
            var configuredPageTypeMappings = UmbraCodeFirstConfiguration.ConfiguredPageTypeMappings;
            foreach (var configuredPageTypeMapping in configuredPageTypeMappings)
            {
                var nodeTypeAlias = configuredPageTypeMapping.Key;
                var type = configuredPageTypeMapping.Value;

                // Overrides automatical type mappings
                if (_pageTypeMap.ContainsKey(nodeTypeAlias))
                {
                    _pageTypeMap[nodeTypeAlias] = type;
                }
                else
                {
                    _pageTypeMap.Add(nodeTypeAlias, type);
                }
            }
        }

        private static IPageFactory _instance;

        /// <summary>
        /// Singleton instance of the page factory
        /// </summary>
        public static IPageFactory Instance
        {
            get { return _instance ?? (_instance = new PageFactory()); }
        }

        #endregion

        #region IPageBase
        public UmbracoPageBase GetPage(int? nodeId)
        {
            if (nodeId.HasValue)
                return GetPage(nodeId.Value);

            throw new PageNotFoundException();
        }

        public UmbracoPageBase GetPage(int nodeId)
        {
            if (nodeId > 0)
                return GetPage(new Node(nodeId));

            throw new PageNotFoundException();
        }

        public UmbracoPageBase GetPage(umbraco.presentation.nodeFactory.Node node)
        {
            return GetPage(node.Id);
        }

        public UmbracoPageBase GetPage(INode node)
        {
            if (node == null)
                throw new PageNotFoundException();

            try
            {
                var type = GetTypeFromNodeTypeAlias(node.NodeTypeAlias);

                if (type != null)
                {
                    var typedObject = Activator.CreateInstance(type, node);
                    var umbracoPageBase = typedObject as UmbracoPageBase;
                    if (umbracoPageBase != null)
                        return umbracoPageBase;
                }
                return new UmbracoPageBase(node);
            }
            catch
            {
                return new UmbracoPageBase(node);
            }
        }
        #endregion

        #region Generics
        public T GetPage<T>(int? nodeId) where T : UmbracoPageBase
        {
            if (nodeId.HasValue)
                return GetPage<T>(nodeId.Value);

            throw new PageNotFoundException();
        }

        public T GetPage<T>(int nodeId) where T : UmbracoPageBase
        {
            if (nodeId > 0)
                return GetPage<T>(new Node(nodeId));

            throw new PageNotFoundException();
        }

        public T GetPage<T>(umbraco.presentation.nodeFactory.Node node) where T : UmbracoPageBase
        {
            return GetPage<T>(node.Id);
        }

        public T GetPage<T>(INode node) where T : UmbracoPageBase
        {

            var type = GetTypeFromNodeTypeAlias(node.NodeTypeAlias);

            if (type != null)
                return (T)Activator.CreateInstance(type, node);

            return (T)GetPage(node);
        }

        private Type GetTypeFromNodeTypeAlias(string nodeTypeAlias)
        {
            return _pageTypeMap.ContainsKey(nodeTypeAlias) ? _pageTypeMap[nodeTypeAlias] : null;
        }

        #endregion

        #region UnCached

        public UmbracoPageBase GetPageFromDatabase(int documentId)
        {
            return GetPage(new DocumentNode(documentId));
        }

        public UmbracoPageBase GetPageFromDatabase(Document document)
        {
            return GetPage(new DocumentNode(document.Id));
        }

        public UmbracoPageBase GetPageFromDatabase(Document document, Guid version)
        {
            return GetPage(new DocumentNode(document.Id, version));
        }

        public T GetPageFromDatabase<T>(int documentId) where T : UmbracoPageBase
        {
            return GetPage<T>(new DocumentNode(documentId));
        }

        public T GetPageFromDatabase<T>(Document document) where T : UmbracoPageBase
        {
            return GetPage<T>(new DocumentNode(document.Id));
        }

        public T GetPageFromDatabase<T>(Document document, Guid version) where T : UmbracoPageBase
        {
            return GetPage<T>(new DocumentNode(document.Id, version));
        }

        public T GetPageFromDatabase<T>(int? id, Guid version) where T : UmbracoPageBase
        {
            return !id.HasValue ? default(T) : GetPage<T>(new DocumentNode(id.Value, version));
        }

        #endregion

    }
}
#pragma warning restore 612,618