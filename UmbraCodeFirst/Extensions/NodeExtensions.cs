using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml;
using umbraco;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.NodeFactory;

namespace UmbraCodeFirst.Extensions
{
    /// <summary>
    /// uQuery extensions for the Node object.
    /// </summary>
    internal static class NodeExtensions
    {
        /// <summary>
        /// Functionally similar to the XPath axis 'ancestor'
        /// Get the Ancestor Nodes from current to root, (useful for breadcrumbs)
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <returns>Node as IEnumerable</returns>
        public static IEnumerable<INode> GetAncestorNodes(this INode node)
        {
            var ancestor = node.Parent;
            while (ancestor != null)
            {
                yield return ancestor;

                ancestor = ancestor.Parent;
            }
        }

        /// <summary>
        /// Functionally similar to the XPath axis 'ancestor-or-self'
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <returns>Node as IEnumerable</returns>
        public static IEnumerable<INode> GetAncestorOrSelfNodes(this INode node)
        {
            yield return node;
            foreach (var ancestor in node.GetAncestorNodes())
            {
                yield return ancestor;
            }
        }

        /// <summary>
        /// Functionally similar to the XPath axis 'preceding-sibling'
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <returns>Node as IEumerable</returns>
        public static IEnumerable<INode> GetPrecedingSiblingNodes(this INode node)
        {
            if (node.Parent != null)
            {
                foreach (var precedingSiblingNode in node.Parent.ChildrenAsList.Where(childNode => childNode.SortOrder < node.SortOrder))
                {
                    yield return precedingSiblingNode;
                }
            }
        }

        /// <summary>
        /// Functionally similar to the XPath axis 'following-sibling'
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <returns>Node as IEumerable</returns>
        public static IEnumerable<INode> GetFollowingSiblingNodes(this INode node)
        {
            if (node.Parent != null)
            {
                foreach (var followingSiblingNode in node.Parent.ChildrenAsList.Where(childNode => childNode.SortOrder > node.SortOrder))
                {
                    yield return followingSiblingNode;
                }
            }
        }

        /// <summary>
        /// Gets all sibling Nodes
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <returns>Node as IEumerable</returns>
        public static IEnumerable<INode> GetSiblingNodes(this INode node)
        {
            if (node.Parent != null)
            {
                foreach (var siblingNode in node.Parent.ChildrenAsList.Where(childNode => childNode.Id != node.Id))
                {
                    yield return siblingNode;
                }
            }
        }

        /// <summary>
        /// Functionally similar to the XPath axis 'descendant-or-self'
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <returns>Node as IEnumerable</returns>
        public static IEnumerable<INode> GetDescendantOrSelfNodes(this INode node)
        {
            yield return node;
            foreach (var descendant in node.GetDescendantNodes())
            {
                yield return descendant;
            }
        }

        /// <summary>
        /// Functionally similar to the XPath axis 'descendant'
        /// Make the All Descendants LINQ queryable
        /// taken from: http://our.umbraco.org/wiki/how-tos/useful-helper-extension-methods-%28linq-null-safe-access%29
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <returns>Node as IEnumerable</returns>
        public static IEnumerable<INode> GetDescendantNodes(this INode node)
        {
            foreach (var child in node.ChildrenAsList)
            {
                yield return child;

                foreach (var descendant in child.GetDescendantNodes())
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Drills down into the descendant nodes returning those where Func is true, when Func is false further descendants are not checked
        /// taken from: http://ucomponents.codeplex.com/discussions/246406
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="func">The func</param>
        /// <returns>Node as IEnumerable</returns>
        public static IEnumerable<INode> GetDescendantNodes(this INode node, Func<INode, bool> func)
        {
            foreach (var child in node.ChildrenAsList.Where(func))
            {
                yield return child;

                foreach (var descendant in child.GetDescendantNodes(func))
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Extension method on Node to retun a matching child node by name
        /// </summary>
        /// <param name="parentNode">an umbraco.NodeFactory.Node object</param>
        /// <param name="nodeName">name of node to search for</param>
        /// <returns>null or Node</returns>
        public static INode GetChildNodeByName(this INode parentNode, string nodeName)
        {
            return parentNode.ChildrenAsList.FirstOrDefault(child => child.Name == nodeName);
        }

        public static T GetPropertyValue<T>(this INode node, string propertyAlias, T defaultValue)
        {
            if (node == null || String.IsNullOrWhiteSpace(propertyAlias))
                return defaultValue;

            var property = node.GetProperty(propertyAlias);
            if (property == null || property.Value == null || String.IsNullOrWhiteSpace(property.Value))
                return defaultValue;

            if (!(property.Value is T) && (typeof(T) == typeof(bool)))
                return (T)Convert.ChangeType(property.Value.Equals("1") ? "true" : "false", typeof(T));

            if (!(property.Value is T) && (typeof(T) == typeof(string)))
                return (T)Convert.ChangeType(property.Value, typeof(T));

            return (T)Convert.ChangeType(property.Value, typeof(T));
        }

        public static T GetPropertyValueRecursive<T>(this INode node, string propertyAlias, T defaultValue)
        {
            if (node == null || String.IsNullOrWhiteSpace(propertyAlias))
                return defaultValue;

            var property = node.GetProperty(propertyAlias);
            if (property == null || property.Value == null || String.IsNullOrWhiteSpace(property.Value))
                return node.Parent == null ? defaultValue : node.Parent.GetPropertyValueRecursive(propertyAlias, defaultValue);

            if (!(property.Value is T) && (typeof(T) == typeof(bool)))
                return (T)Convert.ChangeType(property.Value.Equals("1") ? "true" : "false", typeof(T));

            if (!(property.Value is T) && (typeof(T) == typeof(string)))
                return (T)Convert.ChangeType(property.Value, typeof(T));

            return (T)Convert.ChangeType(property.Value, typeof(T));
        }

        /// <summary>
        /// Get a value of type T from a property
        /// </summary>
        /// <typeparam name="T">type T to cast to</typeparam>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="propertyAlias">alias of property to get</param>
        /// <returns>default(T) or property cast to (T)</returns>
        public static T GetProperty<T>(this INode node, string propertyAlias)
        {
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            if (typeConverter.IsNullOrDefault())
            {
                return default(T);
            }
            if (typeof (T) == typeof (bool))
            {
                // Use the GetPropertyAsBoolean method, as this handles true also being stored as "1"
                return (T) typeConverter.ConvertFrom(node.GetPropertyAsBoolean(propertyAlias).ToString(CultureInfo.InvariantCulture));
            }

            try
            {
                return (T) typeConverter.ConvertFromString(node.GetPropertyAsString(propertyAlias));
            }
            catch
            {
                return default(T);
            }
        }

        /// <summary>
        /// Get a string value for the supplied property alias
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="propertyAlias">alias of propety to get</param>
        /// <returns>empty string, or property value as string</returns>
        public static string GetPropertyAsString(this INode node, string propertyAlias)
        {
            var propertyValue = String.Empty;

            var property = node.GetProperty(propertyAlias);
            if (property != null)
            {
                propertyValue = property.Value;
            }

            return propertyValue;
        }

        /// <summary>
        /// Get a boolean value for the supplied property alias (works with built in Yes/No dataype)
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="propertyAlias">alias of propety to get</param>
        /// <returns>true if can cast value, else false for all other circumstances</returns>
        public static bool GetPropertyAsBoolean(this INode node, string propertyAlias)
        {
            var propertyValue = false; // Default

            var property = node.GetProperty(propertyAlias);
            if (property != null)
            {
                // Umbraco yes / no datatype stores a string value of '1' or '0'
                if (property.Value == "1")
                {
                    propertyValue = true;
                }
                else
                {
                    Boolean.TryParse(property.Value, out propertyValue);
                }
            }

            return propertyValue;
        }

        /// <summary>
        /// Get a DateTime value for the supplied property alias
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="propertyAlias">alias of propety to get</param>
        /// <returns>DateTime value or DateTime.MinValue for all other circumstances</returns>
        public static DateTime GetPropertyAsDateTime(this INode node, string propertyAlias)
        {
            var propertyValue = DateTime.MinValue; // Default

            var property = node.GetProperty(propertyAlias);
            if (property != null)
            {
                DateTime.TryParse(property.Value, out propertyValue);
            }

            return propertyValue;
        }

        /// <summary>
        /// Get an int value for the supplied property alias
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="propertyAlias">alias of propety to get</param>
        /// <returns>int value of property or int.MinValue for all other circumstances</returns>
        public static int GetPropertyAsInt(this INode node, string propertyAlias)
        {
            var propertyValue = Int32.MinValue; // Default

            var property = node.GetProperty(propertyAlias);
            if (property != null)
            {
                Int32.TryParse(property.Value, out propertyValue);
            }

            return propertyValue;
        }

        /// <summary>
        /// Extension method on Node obj to get it's depth
        /// taken from: http://our.umbraco.org/wiki/how-tos/useful-helper-extension-methods-%28linq-null-safe-access%29
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <returns>int for depth, starts at 1</returns>
        public static int GetDepth(this INode node)
        {
            return node.Path.Split(',').Count();
        }

        /// <summary>
        /// Returns the url for a given crop name using the built in Image Cropper datatype
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="propertyAlias">property alias</param>
        /// <param name="cropName">name of crop to get url for</param>
        /// <returns>emtpy string or url</returns>
        public static string GetImageCropperUrl(this INode node, string propertyAlias, string cropName)
        {
            string cropUrl = String.Empty;

            /*
            * Example xml : 
            * 
            * <crops date="28/11/2010 16:08:13">
            *   <crop name="Big" x="0" y="0" x2="1024" y2="768" url="/media/135/image_Big.jpg" />
            *   <crop name="Small" x="181" y="0" x2="608" y2="320" url="/media/135/image_Small.jpg" />
            * </crops>
            * 
            */

            if (!String.IsNullOrEmpty(node.GetPropertyAsString(propertyAlias)))
            {
                var xml = node.GetProperty<string>(propertyAlias);
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(xml);
                var cropNode = xmlDocument.SelectSingleNode("descendant::crops/crop[@name='" + cropName + "']");

                if (cropNode != null)
                {
                    if (cropNode.Attributes != null)
                        cropUrl = cropNode.Attributes.GetNamedItem("url").InnerText;
                }
            }

            return cropUrl;
        }

        /// <summary>
        /// Sets a property value on this node
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="propertyAlias">alias of property to set</param>
        /// <param name="value">value to set</param>
        /// <returns>the same node object on which this is an extension method</returns>
        public static INode SetProperty(this INode node, string propertyAlias, object value)
        {
            var document = new Document(node.Id);

            document.SetProperty(propertyAlias, value);

            return node;
        }

        /// <summary>
        /// Republishes this node
        /// </summary>
        /// <param name="node">an umbraco.NodeFactory.Node object</param>
        /// <param name="useAdminUser">if true then publishes under the context of User(0), if false uses current user</param>
        /// <returns>the same node object on which this is an extension method</returns>
        public static INode Publish(this INode node, bool useAdminUser)
        {
            var document = new Document(node.Id);
            document.Publish(useAdminUser);

            return node;
        }

        public static IEnumerable<INode> GetBreadCrumb(this INode node, bool includeStartPage = true)
        {
            var allNodes = node.Path.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (allNodes.Count == 0)
                return null;
            if (!includeStartPage)
                allNodes.RemoveAt(0);
            return allNodes.Select(n => new Node(Int32.Parse(n)));
        }

        /// <summary>
        /// Extension method on Node collection to return key value pairs of: node.Id / node.Name
        /// </summary>
        /// <param name="nodes">generic list of node objects</param>
        /// <returns>a collection of nodeIDs and their names</returns>
        public static Dictionary<int, string> ToNameIds(this IList<INode> nodes)
        {
            return nodes.ToDictionary(node => node.Id, node => node.Name);
        }

        /// <summary>
        /// Gets the XML for the Node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>Returns an <c>XmlNode</c> for the selected Node</returns>
        public static XmlNode ToXml(this INode node)
        {
            var hasXmlNode = (IHasXmlNode)library.GetXmlNodeById(node.Id.ToString(CultureInfo.InvariantCulture)).Current;
            return hasXmlNode != null ? hasXmlNode.GetNode() : null;
        }

        /// <summary>
        /// <para>Returns the top most page in the tree.</para>
        /// <para>Similar to Model.AnscestorOrSelf(1)</para>
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <returns>Returns the node at the top level.</returns>
        public static INode GetStartPage(this INode currentNode)
        {
            try
            {
                var csv = currentNode.Path.Split(',').Select(Int32.Parse).ToArray();
                return new Node(csv[1]);
            }
            catch
            {
                return null;
            }
        }
    }
}