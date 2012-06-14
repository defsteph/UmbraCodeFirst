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
    public class ModelFactory : IModelFactory
    {
        private readonly IDictionary<string, Type> _pageTypeMap = new Dictionary<string, Type>();

        #region Singleton

        private ModelFactory()
        {
            LoadInstalledPageTypesByReflection();
            LoadInstalledPageTypesFromConfiguration();
        }

        private void LoadInstalledPageTypesByReflection()
        {
            var inherentPageTypes = TypeFinder.FindClassesOfType<IModelBase>(true, true);
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

        private static IModelFactory _instance;

        /// <summary>
        /// Singleton instance of the page factory
        /// </summary>
        public static IModelFactory Instance
        {
            get { return _instance ?? (_instance = new ModelFactory()); }
        }

        #endregion

        public UmbracoModelBase GetModel(int? nodeId)
        {
            if (nodeId.HasValue)
                return GetModel(nodeId.Value);

            throw new ModelNotFoundException();
        }

        public UmbracoModelBase GetModel(int nodeId)
        {
            if (nodeId > 0)
                return GetModel(new Node(nodeId));

            throw new ModelNotFoundException();
        }

        public UmbracoModelBase GetModel(umbraco.presentation.nodeFactory.Node node)
        {
            return GetModel(node.Id);
        }

        public UmbracoModelBase GetModel(INode node)
        {
            if (node == null)
                throw new ModelNotFoundException();

            try
            {
                var type = GetTypeFromNodeTypeAlias(node.NodeTypeAlias);

                if (type != null)
                {
                    var typedObject = Activator.CreateInstance(type, node);
                    var umbracoPageBase = typedObject as UmbracoModelBase;
                    if (umbracoPageBase != null)
                        return umbracoPageBase;
                }
                return new UmbracoModelBase(node);
            }
            catch
            {
                return new UmbracoModelBase(node);
            }
        }
        public T GetModel<T>(int? nodeId) where T : UmbracoModelBase
        {
            if (nodeId.HasValue)
                return GetModel<T>(nodeId.Value);

            throw new ModelNotFoundException();
        }

        public T GetModel<T>(int nodeId) where T : UmbracoModelBase
        {
            if (nodeId > 0)
                return GetModel<T>(new Node(nodeId));

            throw new ModelNotFoundException();
        }

        public T GetModel<T>(umbraco.presentation.nodeFactory.Node node) where T : UmbracoModelBase
        {
            return GetModel<T>(node.Id);
        }

        public T GetModel<T>(INode node) where T : UmbracoModelBase
        {
            var type = GetTypeFromNodeTypeAlias(node.NodeTypeAlias);

            if (type != null)
                return (T)Activator.CreateInstance(type, node);

            return (T)GetModel(node);
        }

        private Type GetTypeFromNodeTypeAlias(string nodeTypeAlias)
        {
            return _pageTypeMap.ContainsKey(nodeTypeAlias) ? _pageTypeMap[nodeTypeAlias] : null;
        }

        public UmbracoModelBase GetModelFromDatabase(int documentId)
        {
            return GetModel(new DocumentNode(documentId));
        }

        public UmbracoModelBase GetModelFromDatabase(Document document)
        {
            return GetModel(new DocumentNode(document.Id));
        }

        public UmbracoModelBase GetModelFromDatabase(Document document, Guid version)
        {
            return GetModel(new DocumentNode(document.Id, version));
        }

        public T GetModelFromDatabase<T>(int documentId) where T : UmbracoModelBase
        {
            return GetModel<T>(new DocumentNode(documentId));
        }

        public T GetModelFromDatabase<T>(Document document) where T : UmbracoModelBase
        {
            return GetModel<T>(new DocumentNode(document.Id));
        }

        public T GetModelFromDatabase<T>(Document document, Guid version) where T : UmbracoModelBase
        {
            return GetModel<T>(new DocumentNode(document.Id, version));
        }

        public T GetModelFromDatabase<T>(int? id, Guid version) where T : UmbracoModelBase
        {
            return !id.HasValue ? default(T) : GetModel<T>(new DocumentNode(id.Value, version));
        }
    }
}
#pragma warning restore 612,618