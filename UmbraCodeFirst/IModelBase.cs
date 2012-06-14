using System;
using System.Collections.Generic;
using umbraco.interfaces;

namespace UmbraCodeFirst
{
    public interface IModelBase
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
        IList<IModelBase> GetChildren();
        IList<IModelBase> GetDescendants();
        IList<T> GetChildrenOfType<T>() where T : IModelBase;
        IList<T> GetDescendantsOfType<T>() where T : IModelBase;
    }
}
