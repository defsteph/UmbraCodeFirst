using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UmbraCodeFirst.Attributes;
using UmbraCodeFirst.Exceptions;
using umbraco.BusinessLogic.Utils;
using umbraco.BusinessLogic.console;
using umbraco.cms.businesslogic;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.propertytype;
using umbraco.cms.businesslogic.web;

namespace UmbraCodeFirst.Synchronization
{
    internal class DocumentTypeSynchronizer : ISynchronizer
    {
        private readonly IList<DocumentType> _installedDocumentTypes;
        private readonly IList<Type> _typesWithDocumentTypeAttribute;

        private readonly IDictionary<int, Type> _documentTypeIdTypeMappings;
        private readonly IDictionary<Type, int> _typeDocumentTypeIdMappings;
 
        #region Singleton

        private DocumentTypeSynchronizer()
        {
            _installedDocumentTypes = DocumentType.GetAllAsList();
            _typesWithDocumentTypeAttribute = TypeFinder.FindClassesMarkedWithAttribute(typeof(DocumentTypeAttribute)).ToList();
            _documentTypeIdTypeMappings = new Dictionary<int, Type>();
            _typeDocumentTypeIdMappings = new Dictionary<Type, int>();
            LoadInstalledDocumentTypes();
        }

        private static DocumentTypeSynchronizer _instance;

        public static DocumentTypeSynchronizer Instance
        {
            get { return _instance ?? (_instance = new DocumentTypeSynchronizer()); }
        }

        #endregion

        private void LoadInstalledDocumentTypes()
        {
            var installedTypes = _typesWithDocumentTypeAttribute.Where(type => _installedDocumentTypes.Any(documentType => documentType.Alias == type.Name));
            InsertInstalledTypesInMappingsDictionaries(installedTypes);
        }

        private void InsertInstalledTypesInMappingsDictionaries(IEnumerable<Type> installedTypes)
        {
            foreach (var installedType in installedTypes)
            {
                var type = installedType;
                var documentType = _installedDocumentTypes.FirstOrDefault(dt => dt.Alias == type.Name);
                if (documentType == null)
                    continue;

                InsertDocumentTypeIdAndTypeInMappingsDictionaries(documentType.Id, type);
            }
        }

        private void InsertDocumentTypeIdAndTypeInMappingsDictionaries(int id, Type type)
        {
            InsertInDocumentTypeIdToTypeMapping(id, type);
            InsertInTypeToDocumentTypeIdMapping(id, type);
        }

        private void InsertInDocumentTypeIdToTypeMapping(int id, Type type)
        {
            if (!_documentTypeIdTypeMappings.ContainsKey(id))
                _documentTypeIdTypeMappings.Add(id, type);
        }

        private void InsertInTypeToDocumentTypeIdMapping(int id, Type type)
        {
            if (!_typeDocumentTypeIdMappings.ContainsKey(type))
                _typeDocumentTypeIdMappings.Add(type, id);
        }

        public void Synchronize()
        {
            EnsureDocumentTypes();

            SynchronizeAllDocumentTypes();
        }

        public int ExecutionOrder
        {
            get { return 2; }
        }

        private void EnsureDocumentTypes()
        {
            var uninstalledTypes = _typesWithDocumentTypeAttribute.Where(type => _installedDocumentTypes.All(documentType => documentType.Alias != type.Name));

            foreach (var type in uninstalledTypes)
            {
                EnsureDocumentType(type);
            }
        }

        private int EnsureDocumentType(Type type)
        {
            if (type == typeof(UmbracoModelBase))
                return 0;

            ThrowExceptionIfNotInheritsFromUmbracoPageBase(type);

            if (_typeDocumentTypeIdMappings.ContainsKey(type))
                return _typeDocumentTypeIdMappings[type];

            // This won't work if we inherit from a type, that in turn inherits from another type that has the attribute specified.
            var documentTypeAttribute = GetDocumentTypeAttributeFromType(type);
            if (documentTypeAttribute == null)
                return 0;

            var alias = type.Name;
            var masterDocumentTypeId = EnsureDocumentType(type.BaseType);

            var umbracoDocumentType = DocumentType.MakeNew(Constants.DefaultBackendUser, alias);

            SetDocumentTypeValues(umbracoDocumentType, masterDocumentTypeId, documentTypeAttribute);

            InsertDocumentTypeIdAndTypeInMappingsDictionaries(umbracoDocumentType.Id, type);

            _installedDocumentTypes.Add(umbracoDocumentType);

            return umbracoDocumentType.Id;
        }

        private static void SetDocumentTypeValues(DocumentType documentType, int masterDocumentTypeId, DocumentTypeAttribute attribute)
        {
            SetDocumentTypeNameIfDifferent(documentType, attribute.Name);
            SetDocumentTypeDescriptionIfDifferent(documentType, attribute.Description);
            SetDocumentTypeIconIfDifferent(documentType, attribute.IconUrl);
            SetDocumentTypeThumbnailIfDifferent(documentType, attribute.ThumbnailUrl);
            SetDocumentTypeMasterIfDifferent(documentType, masterDocumentTypeId);
        }

        private static void SetDocumentTypeNameIfDifferent(IconI documentType, string name)
        {
            if (documentType.Text == name)
                return;

            documentType.Text = name;
        }

        private static void SetDocumentTypeDescriptionIfDifferent(ContentType documentType, string description)
        {
            if (documentType.Description == description)
                return;

            documentType.Description = description ?? String.Empty;
        }

        private static void SetDocumentTypeIconIfDifferent(ContentType documentType, string iconUrl)
        {
            if (documentType.IconUrl == iconUrl)
                return;

            documentType.IconUrl = iconUrl ?? String.Empty;
        }

        private static void SetDocumentTypeThumbnailIfDifferent(ContentType documentType, string thumbnailUrl)
        {
            if (documentType.Thumbnail == thumbnailUrl)
                return;

            documentType.Thumbnail = thumbnailUrl ?? String.Empty;
        }

        private static void SetDocumentTypeMasterIfDifferent(ContentType documentType, int masterDocumentTypeId)
        {
            if (documentType.MasterContentType == masterDocumentTypeId)
                return;
                documentType.MasterContentType = masterDocumentTypeId;
        }

        private static void ThrowExceptionIfNotInheritsFromUmbracoPageBase(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (!type.IsSubclassOf(typeof(UmbracoModelBase)))
                throw new DocumentTypeException();
        }

        private static DocumentTypeAttribute GetDocumentTypeAttributeFromType(Type type)
        {
            return (DocumentTypeAttribute)type.GetCustomAttributes(true).SingleOrDefault(attribute => attribute is DocumentTypeAttribute);
        }

        private void SynchronizeAllDocumentTypes()
        {
            foreach (var documentTypeIdTypeMapping in _documentTypeIdTypeMappings)
            {
                var type = documentTypeIdTypeMapping.Value;
                var documentType = _installedDocumentTypes.FirstOrDefault(t => t.Id == documentTypeIdTypeMapping.Key);
                if (documentType == null)
                    continue;

                var attribute = GetDocumentTypeAttributeFromType(type);
                if (attribute == null)
                    continue;

                SetDocumentTypeNameIfDifferent(documentType, attribute.Name);
                SetAllowedChildren(documentType, attribute.AllowedChildDocumentTypes);
                SetDocumentTypeIconIfDifferent(documentType, attribute.IconUrl);
                SetDocumentTypeThumbnailIfDifferent(documentType, attribute.ThumbnailUrl);
                SetDefaultAndAllowedTemplates(documentType, attribute.AllowedTemplates, attribute.DefaultTemplate);
                SynchronizePropertiesForType(documentType, type);

                documentType.Save();
            }
        }

        private void SetAllowedChildren(ContentType documentType, Type[] allowedChildDocumentTypes)
        {
            if (documentType == null || allowedChildDocumentTypes == null || !allowedChildDocumentTypes.Any())
                return;

            documentType.AllowedChildContentTypeIDs = allowedChildDocumentTypes
                .Where(allowedChildDocumentType => _typeDocumentTypeIdMappings.ContainsKey(allowedChildDocumentType))
                .Select(allowedChildDocumentType => _typeDocumentTypeIdMappings[allowedChildDocumentType]).ToArray();
        }

        private void SetDefaultAndAllowedTemplates(DocumentType documentType, Type[] allowedTemplates, Type defaultTemplate)
        {
            if (documentType == null)
                return;

            if (defaultTemplate != null && CheckIfAllowedTemplatesContainsDefaultTemplate(allowedTemplates, defaultTemplate))
                throw new DefaultTemplateException(defaultTemplate, _documentTypeIdTypeMappings[documentType.Id]);

            documentType.allowedTemplates = TemplateSynchronizer.Instance.GetTemplatesForTypes(allowedTemplates);
            documentType.DefaultTemplate = TemplateSynchronizer.Instance.GetTemplateIdForType(defaultTemplate);
        }

        private static bool CheckIfAllowedTemplatesContainsDefaultTemplate(Type[] allowedTemplates, Type defaultTemplate)
        {
            return (allowedTemplates.Any() && !allowedTemplates.Contains(defaultTemplate));
        }

        private static void SynchronizePropertiesForType(ContentType documentType, Type type)
        {
            if (documentType == null || type == null)
                return;

            var synchronizableProperties = GetPropertiesWithAttributes(type);
            foreach (var property in synchronizableProperties)
            {
                SynchronizeProperty(documentType, property);
            }
        }

        private static IEnumerable<KeyValuePair<PropertyInfo, DocumentTypePropertyAttribute>> GetPropertiesWithAttributes(IReflect type)
        {
            var privateOrPublicInstanceProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var propertiesWithPropertyAttributes = privateOrPublicInstanceProperties.Where(propertyInfo => HasAttribute(propertyInfo, typeof(DocumentTypePropertyAttribute)));

            var propertyAttributeMapping = new Dictionary<PropertyInfo, DocumentTypePropertyAttribute>();

            foreach (var property in propertiesWithPropertyAttributes)
            {
                var propertyAttribute = (DocumentTypePropertyAttribute)property.GetCustomAttributes(true).SingleOrDefault(attribute => attribute is DocumentTypePropertyAttribute);
                if (propertyAttribute != null && !propertyAttributeMapping.ContainsKey(property))
                {
                    propertyAttributeMapping.Add(property, propertyAttribute);
                }
            }
            return propertyAttributeMapping;
        }

        private static bool HasAttribute(MemberInfo memberInfo, Type attributeType)
        {
            return memberInfo.GetCustomAttributes(attributeType, false).Length > 0;
        }

        private static void SynchronizeProperty(ContentType documentType, KeyValuePair<PropertyInfo, DocumentTypePropertyAttribute> property)
        {
            if (documentType == null)
                return;

            var propertyAlias = Utility.FormatPropertyAlias(property.Key.Name);
            var propertyType = documentType.getPropertyType(propertyAlias);
            var propertyAttribute = property.Value;

            if (propertyType == null)
                propertyType = CreateAndAddPropertyTypeToDocumentType(propertyAlias, documentType, propertyAttribute);
            else
                UpdateExistingProperty(propertyType, propertyAttribute, documentType);

            propertyType.Save();
        }

        private static PropertyType CreateAndAddPropertyTypeToDocumentType(string propertyAlias, ContentType documentType, DocumentTypePropertyAttribute propertyAttribute)
        {
            var dataTypeDefinition = DataTypeDefinition.GetDataTypeDefinition(propertyAttribute.DataTypeId);
            if (dataTypeDefinition == null)
                throw new DataTypeDefinitionUnknownException(propertyAttribute.DataTypeId);

            var propertyType = documentType.AddPropertyType(dataTypeDefinition, propertyAlias, propertyAttribute.Name);

            SetPropertyTypeMandatoryIfDifferent(propertyType, propertyAttribute.Mandatory);
            SetPropertyTypeValidationExpressionIfDifferent(propertyType, propertyAttribute.ValidationExpression);
            SetPropertyTypeSortOrderIfDifferent(propertyType, propertyAttribute.SortOrder);
            SetPropertyTypeDescriptionIfDifferent(propertyType, propertyAttribute.Description);
            SetPropertyTypeTabIfDifferent(documentType, propertyType, propertyAttribute.Tab);

            return propertyType;
        }

        private static void SetPropertyTypeMandatoryIfDifferent(PropertyType propertyType, bool mandatory)
        {
            if (propertyType.Mandatory == mandatory)
                return;

            propertyType.Mandatory = mandatory;
        }

        private static void SetPropertyTypeValidationExpressionIfDifferent(PropertyType propertyType, string validationExpression)
        {
            if (propertyType.ValidationRegExp == validationExpression)
                return;

            propertyType.ValidationRegExp = validationExpression ?? String.Empty;
        }

        private static void SetPropertyTypeSortOrderIfDifferent(PropertyType propertyType, int sortOrder)
        {
            if (propertyType.SortOrder == sortOrder)
                return;

            propertyType.SortOrder = sortOrder;
        }

        private static void SetPropertyTypeDescriptionIfDifferent(PropertyType propertyType, string description)
        {
            if (propertyType.Description == description)
                return;

            propertyType.Description = description ?? String.Empty;
        }

        private static void SetPropertyTypeTabIfDifferent(ContentType documentType, PropertyType propertyType, Type tab)
        {
            var propertyTabId = EnsureTab(documentType, tab);

            if (propertyType.TabId == propertyTabId)
                return;

            propertyType.TabId = propertyTabId;
        }

        private static int EnsureTab(ContentType documentType, Type tab)
        {
            if (!tab.IsSubclassOf(typeof(Tab)))
                throw new InvalidTabTypeException(tab);

            if (tab == typeof(DefaultTab))
                return 0;

            var tabInstance = (Tab)Activator.CreateInstance(tab);

            var documentTab = documentType.getVirtualTabs.FirstOrDefault(t => t.GetRawCaption() == tabInstance.Name);
            if (documentTab == null)
            {
                var tabId = documentType.AddVirtualTab(tabInstance.Name);
                // This doesn't always return a tab, why? Any other way?
                documentType.ClearVirtualTabs();
                documentTab = documentType.getVirtualTabs.FirstOrDefault(t => t.Id == tabId);
            }

            if (documentTab != null)
            {
                documentType.SetTabName(documentTab.Id, tabInstance.Name);
                documentType.SetTabSortOrder(documentTab.Id, tabInstance.SortOrder);
            }

            return documentTab == null ? 0 : documentTab.Id;
        }

        private static void UpdateExistingProperty(PropertyType propertyType, DocumentTypePropertyAttribute propertyAttribute, ContentType documentType)
        {
            SetPropertyTypeNameIfDifferent(propertyType, propertyAttribute.Name);
            SetPropertyTypeDescriptionIfDifferent(propertyType, propertyAttribute.Description);
            SetPropertyTypeMandatoryIfDifferent(propertyType, propertyAttribute.Mandatory);
            SetPropertyTypeValidationExpressionIfDifferent(propertyType, propertyAttribute.ValidationExpression);
            SetPropertyTypeSortOrderIfDifferent(propertyType, propertyAttribute.SortOrder);
            SetPropertyTypeTabIfDifferent(documentType, propertyType, propertyAttribute.Tab);
        }

        private static void SetPropertyTypeNameIfDifferent(PropertyType propertyType, string name)
        {
            if (propertyType.Name == name)
                return;

            propertyType.Name = name;
        }
    }
}