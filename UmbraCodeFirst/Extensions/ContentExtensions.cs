using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;

namespace UmbraCodeFirst.Extensions
{
    /// <summary>
    /// Extension methods for umbraco.cms.businesslogic.Content
    /// </summary>
    internal static class ContentExtensions
    {
        /// <summary>
        /// Determines whether the specified media has property.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="propertyAlias">The property alias.</param>
        /// <returns>
        ///   <c>true</c> if the specified media has property; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasProperty(this Content content, string propertyAlias)
        {
            var property = content.getProperty(propertyAlias);
            return (property != null);
        }

        public static XElement GetPropertyAsXml(this Content content, string propertyAlias)
        {
            if (content == null || String.IsNullOrWhiteSpace(propertyAlias))
                return null;

            var property = content.GenericProperties.FirstOrDefault(prop => prop.PropertyType.Alias.Equals(propertyAlias));
            if (property == null || property.Value == null || String.IsNullOrWhiteSpace(property.Value.ToString()))
                return null;

            try
            {
                return XElement.Parse(property.Value.ToString());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get a value of type T from a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="propertyAlias">alias of property to get</param>
        /// <param name="defaultValue">the default value to return</param>
        /// <returns>default(T) or property cast to (T)</returns>
        public static T GetPropertyValue<T>(this Content content, string propertyAlias, T defaultValue = default(T))
        {
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));

            if (typeConverter.IsNullOrDefault())
            {
                return defaultValue;
            }
            if (typeof (T) == typeof (bool))
            {
                // Use the GetPropertyAsBoolean method, as this handles true also being stored as "1"
                return (T) typeConverter.ConvertFrom(content.GetPropertyValueAsBoolean(propertyAlias).ToString(CultureInfo.InvariantCulture));
            }
            try
            {
                return (T) typeConverter.ConvertFromString(content.GetPropertyValueAsString(propertyAlias));
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Get a string value for the supplied property alias
        /// </summary>
        /// <param name="content">an umbraco.cms.businesslogic.Content object</param>
        /// <param name="propertyAlias">alias of propety to get</param>
        /// <returns>empty string, or property value as string</returns>
        public static string GetPropertyValueAsString(this Content content, string propertyAlias)
        {
            var propertyValue = String.Empty;

            var property = content.getProperty(propertyAlias);
            if (property != null)
            {
                propertyValue = property.Value.ToString();
            }

            return propertyValue;
        }

        /// <summary>
        /// Get a boolean value for the supplied property alias (works with built in Yes/No dataype)
        /// </summary>
        /// <param name="content">an umbraco.cms.businesslogic.Content object</param>
        /// <param name="propertyAlias">alias of propety to get</param>
        /// <returns>true if can cast value, else false for all other circumstances</returns>
        public static bool GetPropertyValueAsBoolean(this Content content, string propertyAlias)
        {
            var propertyValue = false; // Default

            var property = content.getProperty(propertyAlias);
            if (property != null)
            {
                if (property.Value.ToString() == "1")
                {
                    propertyValue = true;
                }
                else
                {
                    Boolean.TryParse(property.Value.ToString(), out propertyValue);
                }
            }

            return propertyValue;
        }

        /// <summary>
        /// Get a DateTime value for the supplied property alias
        /// </summary>
        /// <param name="content">an umbraco.cms.businesslogic.Content object</param>
        /// <param name="propertyAlias">alias of propety to get</param>
        /// <returns>DateTime value or DateTime.MinValue for all other circumstances</returns>
        public static DateTime GetPropertyValueAsDateTime(this Content content, string propertyAlias)
        {
            var propertyValue = DateTime.MinValue; // Default

            var property = content.getProperty(propertyAlias);
            if (property != null)
            {
                DateTime.TryParse(property.Value.ToString(), out propertyValue);
            }

            return propertyValue;
        }

        /// <summary>
        /// Get an int value for the supplied property alias
        /// </summary>
        /// <param name="content">an umbraco.cms.businesslogic.Content object</param>
        /// <param name="propertyAlias">alias of propety to get</param>
        /// <returns>int value of property or int.MinValue for all other circumstances</returns>
        public static int GetPropertyValueAsInt(this Content content, string propertyAlias)
        {
            var propertyValue = Int32.MinValue; // Default

            var property = content.getProperty(propertyAlias);
            if (property != null)
            {
                Int32.TryParse(property.Value.ToString(), out propertyValue);
            }

            return propertyValue;
        }

        /// <summary>
        /// Sets a property on this content
        /// </summary>
        /// <param name="content">an umbraco.cms.businesslogic.Content object</param>
        /// <param name="propertyAlias">alias of property to set</param>
        /// <param name="value">value to set</param>
        /// <returns>the same document object on which this is an extension method</returns>
        public static Content SetProperty(this Content content, string propertyAlias, object value)
        {
            var property = content.getProperty(propertyAlias);

            if (property != null)
            {
                // switch based on datatype of property being set - if setting a built in ddl or radion button list, then string supplied is checked against prevalues
                switch (property.PropertyType.DataTypeDefinition.DataType.Id.ToString())
                {
                    case "a74ea9c9-8e18-4d2a-8cf6-73c6206c5da6": // DropDownList
                    case "a52c7c1c-c330-476e-8605-d63d3b84b6a6": // RadioButtonList

                        var preValues = PreValues.GetPreValues(property.PropertyType.DataTypeDefinition.Id);
                        PreValue preValue = null;

                        // switch based on the supplied value type
                        switch (Type.GetTypeCode(value.GetType()))
                        {
                            case TypeCode.String:
                                // attempt to get prevalue from the label
                                preValue = preValues.Values.Cast<PreValue>().FirstOrDefault(x => x.Value == (string)value);
                                break;

                            case TypeCode.Int16:
                            case TypeCode.Int32:
                                // attempt to get prevalue from the id
                                preValue = preValues.Values.Cast<PreValue>().FirstOrDefault(x => x.Id == (int)value);
                                break;
                        }

                        if (preValue != null)
                        {
                            // check db field type being saved to and store prevalue id as an int or a string - note can never save a prevalue id to a date field ! 
                            switch (((DefaultData)property.PropertyType.DataTypeDefinition.DataType.Data).DatabaseType)
                            {
                                case DBTypes.Ntext:
                                case DBTypes.Nvarchar:
                                    property.Value = preValue.Id.ToString(CultureInfo.InvariantCulture);
                                    break;

                                case DBTypes.Integer:
                                    property.Value = preValue.Id;
                                    break;
                            }
                        }

                        break;

                    case "23e93522-3200-44e2-9f29-e61a6fcbb79a": // Date (NOTE: currently assumes database type is set to Date)

                        switch (Type.GetTypeCode(value.GetType()))
                        {
                            case TypeCode.DateTime:
                                property.Value = ((DateTime)value).Date;
                                break;
                            case TypeCode.String:
                                DateTime valueDateTime;
                                if (DateTime.TryParse((string)value, out valueDateTime))
                                {
                                    property.Value = valueDateTime.Date;
                                }
                                break;
                        }

                        break;

                    default:
                        property.Value = value; // This saves the property value
                        break;
                }
            }

            content.Save(); // This doesn't save the property, but sets the node to indicate requires publish

            return content;
        }
    }
}
