using System;
using System.Collections.Generic;
using umbraco.interfaces;

namespace UmbraCodeFirst
{
    public interface IPageBase
    {
        INode Node { get; }
        DateTime CreateDate { get; }
        int CreatorID { get; }
        string CreatorName { get; }
        int Id { get; }
        int Level { get; }
        string NodeName { get; }
        int ParentNodeId { get; }
        int SortOrder { get; }
        int TemplateId { get; }
        DateTime UpdateDate { get; }
        Guid Version { get; }
        int WriterID { get; }
        string WriterName { get; }
        string Path { get; }

        T GetPropertyValue<T>(string alias);
        IList<IPageBase> GetChildren();
        IList<IPageBase> GetDescendants();
        IList<T> GetChildrenOfType<T>() where T : IPageBase;
        IList<T> GetDescendantsOfType<T>() where T : IPageBase;
    }
}
