using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UmbraCodeFirst.Factories;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.NodeFactory;

namespace UmbraCodeFirst
{
    internal class DocumentNode : Document, INode
    {
        private readonly Hashtable _aliasToNames = new Hashtable();
        private readonly Document _underlying;

        #region Constructors
        public DocumentNode(Guid id, bool noSetup) : base(id, noSetup) { }

        public DocumentNode(int id, bool noSetup) : base(id, noSetup) { }

        public DocumentNode(int id, Guid version) : base(id, version) { }

        public DocumentNode(int id) : base(true, id) { }

        public DocumentNode(Guid id) : base(id) { }

        public DocumentNode(bool optimizedMode, int id) : base(optimizedMode, id) { }

        private DocumentNode(Document document) : base(true, document.Id)
        {
            _underlying = document;
        }

        #endregion

        #region Implementation of INode
        public IProperty GetProperty(string alias)
        {
            var property = _underlying != null ? _underlying.getProperty(alias) : getProperty(alias);
            return property == null ? null : new DocumentProperty(property.Id);
        }

        public IProperty GetProperty(string alias, out bool propertyExists)
        {
            var property = GetProperty(alias);
            propertyExists = property != null;
            return property;
        }

        public List<IProperty> PropertiesAsList
        {
            get
            {
                return _underlying != null ? _underlying.GenericProperties.Select(property => new DocumentProperty(property.Id)).Cast<IProperty>().ToList() : GenericProperties.Select(property => new DocumentProperty(property.Id)).Cast<IProperty>().ToList();
            }
        }

        public new IList<DocumentNode> Children { get { return ChildrenAsList.Cast<DocumentNode>().ToList(); } }

        public DataTable ChildrenAsTable()
        {
            if (Children.Count > 0)
            {
                var dt = GenerateDataTable(Children[0]);

                var firstNodeTypeAlias = Children[0].NodeTypeAlias;

                foreach (var n in Children)
                {
                    if (n.NodeTypeAlias != firstNodeTypeAlias)
                        continue;

                    var dr = dt.NewRow();
                    PopulateRow(ref dr, n, GetPropertyHeaders(n));
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return new DataTable();
        }

        public DataTable ChildrenAsTable(string nodeTypeAliasFilter)
        {
            if (Children.Count > 0)
            {
                INode firstnode = null;
                var nodeFound = false;
                foreach (var n in Children.Cast<INode>().Where(n => n.NodeTypeAlias == nodeTypeAliasFilter))
                {
                    firstnode = n;
                    nodeFound = true;
                    break;
                }

                if (nodeFound)
                {
                    var dt = GenerateDataTable(firstnode);

                    foreach (var n in Children)
                    {
                        if (n.NodeTypeAlias != nodeTypeAliasFilter)
                            continue;

                        var dr = dt.NewRow();
                        PopulateRow(ref dr, n, GetPropertyHeaders(n));
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
                return new DataTable();
            }
            return new DataTable();
        }

        private List<INode> _childrenAsList;
        public List<INode> ChildrenAsList
        {
            get
            {
                if (_childrenAsList != null)
                    return _childrenAsList;

                if (_underlying != null)
                    return (_childrenAsList = _underlying.Children.Select(child => new DocumentNode(child.Id)).Cast<INode>().ToList());                       

                return (_childrenAsList = Children.Select(child => new DocumentNode(child.Id)).Cast<INode>().ToList());
            }
        }

        public new INode Parent
        {
            get { return _underlying != null ? new DocumentNode(_underlying.ParentId) : new DocumentNode(ParentId); }
        }

        public string Url
        {
            get { return NiceUrl; }
        }

        public string UrlName
        {
            get { return GetProperty(Constants.PageUrlNamePropertyName).Value; }
        }

        public string NiceUrl
        {
            get
            {
                return _underlying != null
                           ? umbraco.library.NiceUrl(_underlying.Id)
                           : umbraco.library.NiceUrl(Id);
            }
        }

        public int template
        {
            get { return _underlying != null ? _underlying.Template : Template; }
        }

        public int SortOrder
        {
            get { return _underlying != null ? _underlying.sortOrder : sortOrder; }
        }

        public string Name
        {
            get { return _underlying != null ? _underlying.Text : Text; }
        }

        public string NodeTypeAlias
        {
            get { return _underlying != null ? _underlying.ContentType.Alias : ContentType.Alias; }
        }

        public string WriterName
        {
            get { return _underlying != null ? _underlying.Writer.Name : Writer.Name; }
        }

        public string CreatorName
        {
            get { return _underlying != null ? _underlying.Creator.Name : Creator.Name; }
        }

        public int WriterID
        {
            get { return _underlying != null ? _underlying.Writer.Id : Writer.Id; }
        }

        public int CreatorID
        {
            get { return _underlying != null ? _underlying.Creator.Id : Creator.Id; }
        }

        public DateTime CreateDate
        {
            get
            {
                return _underlying != null ? _underlying.CreateDateTime : CreateDateTime;
            }
        }
        #endregion

        #region DataTable Helpers
        private DataTable GenerateDataTable(INode schemaNode)
        {
            var nodeAsDataTable = new DataTable(schemaNode.NodeTypeAlias);
            string[] defaultColumns = {
                                          "Id", 
                                          "NodeName", 
                                          "NodeTypeAlias", 
                                          "CreateDate", 
                                          "UpdateDate", 
                                          "CreatorName",
                                          "WriterName", 
                                          "Url"
                                      };
            foreach (var dc in defaultColumns.Select(s => new DataColumn(s)))
            {
                nodeAsDataTable.Columns.Add(dc);
            }

            // add properties
            var propertyHeaders = GetPropertyHeaders(schemaNode);
            var ide = propertyHeaders.GetEnumerator();
            while (ide.MoveNext())
            {
                var dc = new DataColumn(ide.Value.ToString());
                nodeAsDataTable.Columns.Add(dc);
            }

            return nodeAsDataTable;
        }

        private Hashtable GetPropertyHeaders(INode schemaNode)
        {
            if (_aliasToNames.ContainsKey(schemaNode.NodeTypeAlias))
                return (Hashtable)_aliasToNames[schemaNode.NodeTypeAlias];

            var ct = umbraco.cms.businesslogic.ContentType.GetByAlias(schemaNode.NodeTypeAlias);
            var def = new Hashtable();
            foreach (var pt in ct.PropertyTypes)
                def.Add(pt.Alias, pt.Name);

            HttpContextFactory.Current.Application.Lock();
            _aliasToNames.Add(schemaNode.NodeTypeAlias, def);
            HttpContextFactory.Current.Application.UnLock();

            return def;
        }

        private static void PopulateRow(ref DataRow dr, DocumentNode n, IDictionary aliasesToNames)
        {
            dr["Id"] = n.Id;
            dr["NodeName"] = n.Name;
            dr["NodeTypeAlias"] = n.NodeTypeAlias;
            dr["CreateDate"] = n.CreateDate;
            dr["UpdateDate"] = n.UpdateDate;
            dr["CreatorName"] = n.CreatorName;
            dr["WriterName"] = n.WriterName;
            dr["Url"] = umbraco.library.NiceUrl(n.Id);

            foreach (var p in from Property p in n.PropertiesAsList where p.Value != null select p)
            {
                dr[aliasesToNames[p.Alias].ToString()] = p.Value;
            }
        }
        #endregion

        public static DocumentNode FromDocument(Document document)
        {
            if (document == null)
                return null;

            try
            {
                return new DocumentNode(document);
            }
            catch
            {
                return null;
            }
        }

    }
}