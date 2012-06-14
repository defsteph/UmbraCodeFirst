using System;
using System.Collections.Generic;
using System.Linq;
using UmbraCodeFirst.DataTypes;
using umbraco.BusinessLogic.Utils;
using umbraco.cms.businesslogic.datatype;
using umbraco.DataLayer;

namespace UmbraCodeFirst.Synchronization
{
    internal class DataTypeDefinitionSynchronizer : ISynchronizer
    {
        private static readonly Guid DataTypeDefinitionObjectTypeGuid = new Guid("30A2A501-1978-4DDB-A57B-F7EFED43BA3C");

        private static ISqlHelper SqlHelper
        {
            get { return umbraco.BusinessLogic.Application.SqlHelper; }
        }

        private readonly IList<DataTypeDefinition> _installedDataTypeDefinitions;
        private readonly IList<Type> _synchronizableDataTypeTypes;
        private readonly IList<ISynchronizableDataType> _synchronizableDataTypes;

        private readonly IDictionary<int, ISynchronizableDataType> _idToTypeMappings;
        private readonly IDictionary<ISynchronizableDataType, int> _typeToIdMappings;
        
        #region Singleton

        private DataTypeDefinitionSynchronizer()
        {
            _installedDataTypeDefinitions = DataTypeDefinition.GetAll();
            _synchronizableDataTypeTypes = TypeFinder.FindClassesOfType<ISynchronizableDataType>();
            _synchronizableDataTypes = new List<ISynchronizableDataType>();
            foreach (var instance in _synchronizableDataTypeTypes.Select(Activator.CreateInstance).OfType<ISynchronizableDataType>())
            {
                _synchronizableDataTypes.Add(instance);
            }

            _idToTypeMappings = new Dictionary<int, ISynchronizableDataType>();
            _typeToIdMappings = new Dictionary<ISynchronizableDataType, int>();
            LoadInstalledDataTypes();
        }

        private static DataTypeDefinitionSynchronizer _instance;

        /// <summary>
        /// Singleton instance of the DataTypeDefinitionSynchronizer
        /// </summary>
        public static DataTypeDefinitionSynchronizer Instance
        {
            get { return _instance ?? (_instance = new DataTypeDefinitionSynchronizer()); }
        }

        #endregion

        private void LoadInstalledDataTypes()
        {
            var installedTypes = _synchronizableDataTypes.Where(synchronizable => _installedDataTypeDefinitions.Any(dataType => dataType.UniqueId.Equals(synchronizable.DataTypeId)));
            InsertInstalledTypesInMappingsDictionaries(installedTypes);
        }

        private void InsertInstalledTypesInMappingsDictionaries(IEnumerable<ISynchronizableDataType> installedTypes)
        {
            foreach (var installedType in installedTypes)
            {
                var type = installedType;
                var dataType = _installedDataTypeDefinitions.FirstOrDefault(dt => dt.UniqueId.Equals(type.DataTypeId));
                if (dataType == null)
                    continue;

                InsertIdAndTypeInMappingsDictionaries(dataType.Id, type);
            }
        }

        private void InsertIdAndTypeInMappingsDictionaries(int id, ISynchronizableDataType type)
        {
            InsertInIdToTypeMapping(id, type);
            InsertInTypeToIdMapping(id, type);
        }

        private void InsertInIdToTypeMapping(int id, ISynchronizableDataType type)
        {
            if (!_idToTypeMappings.ContainsKey(id))
                _idToTypeMappings.Add(id, type);
        }

        private void InsertInTypeToIdMapping(int id, ISynchronizableDataType type)
        {
            if (!_typeToIdMappings.ContainsKey(type))
                _typeToIdMappings.Add(type, id);
        }

        public void Synchronize()
        {
            SynchronizeDataTypes();
        }

        private void SynchronizeDataTypes()
        {
            var uninstalledDataTypes = _synchronizableDataTypes.Where(synchronizableDataType => !_installedDataTypeDefinitions.Any(dataType => dataType.UniqueId.Equals(synchronizableDataType.DataTypeId)));
            foreach (var synchronizableDataType in uninstalledDataTypes)
            {
                InstallDataType(synchronizableDataType);
            }
        }

        private static void InstallDataType(ISynchronizableDataType synchronizableDataType)
        {
            SqlHelper.ExecuteNonQuery(
                "SET IDENTITY_INSERT umbracoNode ON; INSERT INTO umbracoNode ([id], trashed, parentID, nodeObjectType, nodeUser, level, path, sortOrder, uniqueID, text, createDate) VALUES (@nodeId, @trashed, @parentID, @nodeObjectType, @nodeUser, @level, @path, @sortOrder, @uniqueID, @text, @createDate);SET IDENTITY_INSERT umbracoNode OFF",
                SqlHelper.CreateParameter("@nodeId", synchronizableDataType.NodeId),
                SqlHelper.CreateParameter("@trashed", 0),
                SqlHelper.CreateParameter("@parentID", -1),
                SqlHelper.CreateParameter("@nodeObjectType", DataTypeDefinitionObjectTypeGuid),
                SqlHelper.CreateParameter("@nodeUser", Constants.DefaultBackendUser.Id),
                SqlHelper.CreateParameter("@level", 1),
                SqlHelper.CreateParameter("@path", "-1," + synchronizableDataType.NodeId),
                SqlHelper.CreateParameter("@sortOrder", 2),
                SqlHelper.CreateParameter("@uniqueID", synchronizableDataType.DataTypeId),
                SqlHelper.CreateParameter("@text", synchronizableDataType.Name),
                SqlHelper.CreateParameter("@createDate", DateTime.Now));

            SqlHelper.ExecuteNonQuery(
                "INSERT INTO cmsDataType (nodeId, controlId, dbType) values (@nodeId, @controlId, @dbType)",
                SqlHelper.CreateParameter("@nodeId", synchronizableDataType.NodeId),
                SqlHelper.CreateParameter("@controlId", synchronizableDataType.DataEditorId),
                SqlHelper.CreateParameter("@dbType", GetDBTypeString(synchronizableDataType.DBType)));
        }

        private static string GetDBTypeString(DBType dbType)
        {
            switch (dbType)
            {
                case DBType.Date:
                    return "Date";
                case DBType.Integer:
                    return "Integer";
                case DBType.NVarChar:
                    return "Nvarchar";
                default:
                    return "Ntext";
            }
        }

        public int ExecutionOrder
        {
            get { return 1; }
        }
    }
}
