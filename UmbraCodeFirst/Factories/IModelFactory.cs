using System;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;

namespace UmbraCodeFirst.Factories
{
    public interface IModelFactory
    {
        UmbracoModelBase GetModel(int? nodeId);
        UmbracoModelBase GetModel(int nodeId);
        [Obsolete("This class is obsolete; use class umbraco.NodeFactory.Node instead", false)]
        UmbracoModelBase GetModel(umbraco.presentation.nodeFactory.Node node);
        UmbracoModelBase GetModel(INode node);
        T GetModel<T>(int? nodeId) where T : UmbracoModelBase;
        T GetModel<T>(int nodeId) where T : UmbracoModelBase;
        [Obsolete("This class is obsolete; use class umbraco.NodeFactory.Node instead", false)]
        T GetModel<T>(umbraco.presentation.nodeFactory.Node node) where T : UmbracoModelBase;
        T GetModel<T>(INode node) where T : UmbracoModelBase;
        UmbracoModelBase GetModelFromDatabase(int documentId);
        UmbracoModelBase GetModelFromDatabase(Document document);
        UmbracoModelBase GetModelFromDatabase(Document document, Guid version);
        T GetModelFromDatabase<T>(int documentId) where T : UmbracoModelBase;
        T GetModelFromDatabase<T>(Document document) where T : UmbracoModelBase;
        T GetModelFromDatabase<T>(Document document, Guid version) where T : UmbracoModelBase;
        T GetModelFromDatabase<T>(int? id, Guid version) where T : UmbracoModelBase;
    }
}
