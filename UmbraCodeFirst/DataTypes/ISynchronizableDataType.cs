using System;

namespace UmbraCodeFirst.DataTypes
{
    public interface ISynchronizableDataType
    {
        int NodeId { get; }
        Guid DataTypeId { get; }
        Guid DataEditorId { get; }
        string Name { get; }
        DBType DBType { get; }
    }
}
