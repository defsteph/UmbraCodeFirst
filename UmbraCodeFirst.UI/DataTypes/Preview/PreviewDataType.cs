using System;
using UmbraCodeFirst.DataTypes;
using umbraco.editorControls;
using umbraco.interfaces;
using BaseDataType = umbraco.cms.businesslogic.datatype.BaseDataType;
using DefaultData = umbraco.cms.businesslogic.datatype.DefaultData;

namespace UmbraCodeFirst.UI.DataTypes.Preview
{
    public class PreviewDataType : BaseDataType, IDataType, ISynchronizableDataType
    {
        private const string DataTypeGuid = "AD5384D6-54B8-4FBD-BA87-BBB0BC7BE782";
        private const string DataEditorGuid = "CC66F333-7AD8-4539-905A-B7B1361D1857";
        public const int DataTypeNodeId = -1337;

        private IDataEditor _editor;
        private IDataPrevalue _prevalueeditor;
        private IData _data;

        public override Guid Id
        {
            get { return new Guid(DataEditorGuid); }
        }

        public override string DataTypeName
        {
            get { return "Preview"; }
        }

        public override IDataEditor DataEditor
        {
            get { return _editor ?? (_editor = new PreviewEditor(Data)); }
        }

        public override IDataPrevalue PrevalueEditor
        {
            get { return _prevalueeditor ?? (_prevalueeditor = new DefaultPrevalueEditor(this, false)); }
        }

        public override IData Data
        {
            get { return _data ?? (_data = new DefaultData(this)); }
        }

        public int NodeId
        {
            get { return DataTypeNodeId; }
        }

        public Guid DataTypeId
        {
            get { return new Guid(DataTypeGuid); }
        }

        public Guid DataEditorId
        {
            get { return Id; }
        }

        public string Name
        {
            get { return DataTypeName; }
        }

        DBType ISynchronizableDataType.DBType
        {
            get { return UmbraCodeFirst.DataTypes.DBType.Integer; }
        }
    }
}