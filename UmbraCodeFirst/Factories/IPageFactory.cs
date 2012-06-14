using System;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;

namespace UmbraCodeFirst.Factories
{
    public interface IPageFactory
    {
        #region IPage
        UmbracoPageBase GetPage(int? nodeId);
        UmbracoPageBase GetPage(int nodeId);
        [Obsolete("This class is obsolete; use class umbraco.NodeFactory.Node instead", false)]
        UmbracoPageBase GetPage(umbraco.presentation.nodeFactory.Node node);
        UmbracoPageBase GetPage(INode node);
        #endregion

        #region Generics
        T GetPage<T>(int? nodeId) where T : UmbracoPageBase;
        T GetPage<T>(int nodeId) where T : UmbracoPageBase;
        [Obsolete("This class is obsolete; use class umbraco.NodeFactory.Node instead", false)]
        T GetPage<T>(umbraco.presentation.nodeFactory.Node node) where T : UmbracoPageBase;
        T GetPage<T>(INode node) where T : UmbracoPageBase;
        #endregion

        #region UnCached
        #region IPage
        UmbracoPageBase GetPageFromDatabase(int documentId);
        UmbracoPageBase GetPageFromDatabase(Document document);
        UmbracoPageBase GetPageFromDatabase(Document document, Guid version);
        #endregion

        #region Generics
        T GetPageFromDatabase<T>(int documentId) where T : UmbracoPageBase;
        T GetPageFromDatabase<T>(Document document) where T : UmbracoPageBase;
        T GetPageFromDatabase<T>(Document document, Guid version) where T : UmbracoPageBase;
        T GetPageFromDatabase<T>(int? id, Guid version) where T : UmbracoPageBase;
        #endregion
        #endregion

    }
}
