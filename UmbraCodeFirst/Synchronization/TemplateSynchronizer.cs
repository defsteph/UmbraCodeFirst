using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using UmbraCodeFirst.Attributes;
using UmbraCodeFirst.Exceptions;
using umbraco.BusinessLogic.Utils;
using umbraco.BusinessLogic.console;
using umbraco.cms.businesslogic.template;

namespace UmbraCodeFirst.Synchronization
{
    internal class TemplateSynchronizer : ISynchronizer
    {
        private readonly IList<Template> _installedTemplates;
        private readonly IList<Type> _typesWithTemplateAttribute;
        
        private readonly IDictionary<int, Type> _templateIdTypeMappings;
        private readonly IDictionary<Type, int> _typeTemplateIdMappings;
 
        #region Singleton

        private TemplateSynchronizer()
        {
            _installedTemplates = Template.GetAllAsList();
            _typesWithTemplateAttribute = TypeFinder.FindClassesMarkedWithAttribute(typeof(DocumentTypeTemplateAttribute)).ToList();
            _templateIdTypeMappings = new Dictionary<int, Type>();
            _typeTemplateIdMappings = new Dictionary<Type, int>();
            LoadInstalledTemplates();
        }

        private static TemplateSynchronizer _instance;

        /// <summary>
        /// Singleton instance of the TemplateSynchronizer
        /// </summary>
        public static TemplateSynchronizer Instance
        {
            get { return _instance ?? (_instance = new TemplateSynchronizer()); }
        }

        #endregion

        private void LoadInstalledTemplates()
        {
            var installedTypes = _typesWithTemplateAttribute.Where(type => _installedTemplates.Any(template => template.Alias == type.Name));
            foreach (var installedType in installedTypes)
            {
                var type = installedType;
                var templateForType = _installedTemplates.FirstOrDefault(template => template.Alias == type.Name);
                if (templateForType == null)
                    continue;

                var templateId = templateForType.Id;

                InsertTemplateIdAndTypeInMappingsDictionaries(templateId, type);
            }
        }

        private void InsertTemplateIdAndTypeInMappingsDictionaries(int id, Type type)
        {
            if (!_typeTemplateIdMappings.ContainsKey(type))
                _typeTemplateIdMappings.Add(type, id);

            if (!_templateIdTypeMappings.ContainsKey(id))
                _templateIdTypeMappings.Add(id, type);
        }

        public void Synchronize()
        {
            EnsureTemplates();

            SynchronizeAllTemplates();
        }

        public int ExecutionOrder
        {
            get { return 0; }
        }

        private void EnsureTemplates()
        {
            var uninstalledTypes = _typesWithTemplateAttribute.Where(type => _installedTemplates.All(template => template.Alias != type.Name));

            foreach (var uninstalledType in uninstalledTypes)
            {
                EnsureTemplate(uninstalledType);
            }
        }

        private int EnsureTemplate(Type type)
        {
            if (!type.IsSubclassOf(typeof(MasterPage)))
                throw new TemplateTypeException();

            if (_typeTemplateIdMappings.ContainsKey(type))
                return _typeTemplateIdMappings[type];

            var templateAttribute = GetTemplateAttributeFromType(type);
            if (templateAttribute == null)
                return 0;

            var alias = type.Name;
            var name = templateAttribute.Name;
            var masterTemplateId = 0;
            var master = templateAttribute.MasterTemplate;
            if (master != null)
            {
                masterTemplateId = EnsureTemplate(master);
            }

            var umbracoTemplate = Template.MakeNew(alias, Constants.DefaultBackendUser);
            if (masterTemplateId > 0)
                umbracoTemplate.MasterTemplate = masterTemplateId;

            umbracoTemplate.Text = name;

            InsertTemplateIdAndTypeInMappingsDictionaries(umbracoTemplate.Id, type);

            _installedTemplates.Add(umbracoTemplate);

            return umbracoTemplate.Id;
        }

        private static DocumentTypeTemplateAttribute GetTemplateAttributeFromType(Type type)
        {
            return (DocumentTypeTemplateAttribute)type.GetCustomAttributes(true).SingleOrDefault(attribute => attribute is DocumentTypeTemplateAttribute);
        }

        private void SynchronizeAllTemplates()
        {
            foreach (var templateIdTypeMapping in _templateIdTypeMappings)
            {
                var type = templateIdTypeMapping.Value;
                var template = _installedTemplates.FirstOrDefault(t => t.Id == templateIdTypeMapping.Key);
                if (template == null)
                    continue;

                var attribute = GetTemplateAttributeFromType(type);
                if (attribute == null)
                    continue;

                UpdateName(template, attribute.Name);
                UpdateMaster(template, attribute.MasterTemplate);
                template.Save();
            }
        }

        private static void UpdateName(IconI template, string name)
        {
            if (template.Text == name)
                return;

            template.Text = name;
        }

        private void UpdateMaster(Template template, Type masterTemplate)
        {
            if (template == null || masterTemplate == null)
                return;

            if (!_typeTemplateIdMappings.ContainsKey(masterTemplate))
                return;

            if (template.MasterTemplate == _typeTemplateIdMappings[masterTemplate])
                return;

            template.MasterTemplate = _typeTemplateIdMappings[masterTemplate];
        }

        public int GetTemplateIdForType(Type templateType)
        {
            if (templateType == null)
                return 0;

            return _typeTemplateIdMappings.ContainsKey(templateType) ? _typeTemplateIdMappings[templateType] : 0;
        }

        public Template[] GetTemplatesForTypes(IEnumerable<Type> templateTypes)
        {
            if (templateTypes == null)
                return new Template[0];

            return
                templateTypes.Select(GetTemplateIdForType)
                .Where(id => id > 0)
                .Select(id => _installedTemplates.FirstOrDefault(t => t.Id == id))
                .Where(template => template != null).ToArray();
        }
    }
}