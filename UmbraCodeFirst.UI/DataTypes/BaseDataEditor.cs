using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using umbraco.interfaces;

namespace UmbraCodeFirst.UI.DataTypes
{
    public abstract class BaseDataEditor<T> : Panel, IDataEditor
    {
        private int? _currentIdParameter;
        protected virtual int CurrentIdParameter
        {
            get
            {
                if (_currentIdParameter.HasValue)
                    return _currentIdParameter.Value;

                var idParameter = Page.Request.QueryString[UmbraCodeFirst.Constants.DefaultIdParameterName];
                if (!String.IsNullOrEmpty(idParameter))
                {
                    int id;
                    if (Int32.TryParse(idParameter, out id))
                    {
                        _currentIdParameter = id;
                        return id;
                    }
                }

                return (_currentIdParameter = 0).Value;
            }
        }

        protected umbraco.cms.businesslogic.Content CurrentContent
        {
            get
            {
                return new umbraco.cms.businesslogic.Content(CurrentIdParameter);
            }
        }

        protected readonly IData Data;
        protected bool CanAccessData;

        public virtual T Value { get; set; }

        protected BaseDataEditor() { }

        protected BaseDataEditor(IData data)
        {
            Data = data;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (UsesClientScript)
                Page.ClientScript.RegisterClientScriptResource(typeof(Constants.Internals), Constants.Internals.EmbeddedResourceScriptFile);

            if (Data == null)
                return;

            CanAccessData = true;
        }

        public abstract void BindData();

        protected virtual bool UsesClientScript { get; set; }

        protected virtual bool HasValue
        {
            get
            {
                if (!Equals(Value, default(T)))
                    return true;

                if (CanAccessData)
                    return (Data != null && Data.Value != null && !String.IsNullOrEmpty(Data.Value.ToString()));

                return false;
            }
        }

        public abstract void Save();

        public virtual bool ShowLabel
        {
            get { return true; }
        }

        public bool TreatAsRichTextEditor
        {
            get { return false; }
        }

        public Control Editor
        {
            get { return this; }
        }
    }
}