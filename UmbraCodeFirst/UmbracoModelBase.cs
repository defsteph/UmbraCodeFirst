using System;
using System.Collections.Generic;
using System.Linq;
using UmbraCodeFirst.Extensions;
using UmbraCodeFirst.Factories;
using umbraco.interfaces;

namespace UmbraCodeFirst
{
    public class UmbracoModelBase : IModelBase
    {
        private readonly INode _node;

        #region Constructors
        protected UmbracoModelBase() { }
        public UmbracoModelBase(INode node)
        {
            _node = node;
        }
        #endregion

        #region Properties
        public INode Node { get { return _node; } }
        public DateTime CreateDate { get { return _node.CreateDate; } }
        public int CreatorID { get { return _node.CreatorID; } }
        public string CreatorName { get { return _node.CreatorName; } }
        public int Id { get { return _node.Id; } }
        public int Level { get { return _node.Level; } }
        public virtual string NodeName { get { return _node.Name; } }
        public int ParentNodeId { get { return _node.Parent != null ? _node.Parent.Id : -1; } }
        public int SortOrder { get { return _node.SortOrder; } }
        public int TemplateId { get { return _node.template; } }
        public DateTime UpdateDate { get { return _node.UpdateDate; } }
        public Guid Version { get { return _node.Version; } }
        public int WriterID { get { return _node.WriterID; } }
        public string WriterName { get { return _node.WriterName; } }
        public string Path { get { return _node.Path; } }
        #endregion

        public void SetPropertyValue(string alias, object value)
        {
            bool propertyExists;
            var property = _node.GetProperty(alias, out propertyExists);
            if (propertyExists)
            {
                _node.SetProperty(alias, value);
            }
        }

        public T GetPropertyValue<T>(string alias)
        {
            return _node.GetProperty<T>(alias);
        }

        public IList<IModelBase> GetChildren()
        {
            return _node.ChildrenAsList.Select(child => ModelFactory.Instance.GetModel<UmbracoModelBase>(child)).Cast<IModelBase>().ToList();
        }

        public IList<IModelBase> GetDescendants()
        {
            return _node.GetDescendantNodes().Select(descendant => ModelFactory.Instance.GetModel<UmbracoModelBase>(descendant)).Cast<IModelBase>().ToList();
        }

        public IList<T> GetChildrenOfType<T>() where T : IModelBase
        {
            return GetChildren().OfType<T>().ToList();
        }

        public IList<T> GetDescendantsOfType<T>() where T : IModelBase
        {
            return GetDescendants().OfType<T>().ToList();
        }

        public override string ToString()
        {
            return String.Format("ID: {0}, Name: {1}, Type: {2}", Id, NodeName, Node.NodeTypeAlias);
        }
    }
}