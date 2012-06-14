using System;
using System.Web.UI.WebControls;
using UmbraCodeFirst.Factories;
using umbraco.interfaces;

namespace UmbraCodeFirst.UI.DataTypes.Preview
{
    public class PreviewEditor : BaseDataEditor<bool>
    {
        public PreviewEditor(IData data) : base(data) { }
        public override bool ShowLabel { get { return false; } }

        public override void BindData()
        {

        }

        public override void Save()
        {

        }

        protected override void OnInit(EventArgs e)
        {
            UsesClientScript = true;
            base.OnInit(e);

            AddPreviewControl();
        }

        #region Controls
        protected Literal Preview = new Literal();
        #endregion

        private void AddPreviewControl()
        {
            Preview.ID = String.Concat(ID, "_iframe");

            Page.ClientScript.RegisterStartupScript(GetType(), Preview.ID, String.Format("{0}.EnlargePreview('{1}');", Constants.Internals.ClientScriptFileKey, Preview.ClientID), true);

            Preview.Text = String.Format("<iframe id=\"{0}\" src=\"{1}/{2}.aspx?{3}={4}\" border=\"0\"></iframe>",
                              Preview.ClientID,
                              GetBaseUrl(),
                              CurrentContent.Id, 
                              UmbraCodeFirst.Constants.VersionParameterName, 
                              GetLastestVersionId());

            Controls.Add(Preview);
        }

        private static string GetBaseUrl()
        {
            return String.Concat(((HttpContextFactory.Current.Request.IsSecureConnection) ? "https://" : "http://"),
                                             HttpContextFactory.Current.Request.ServerVariables["SERVER_NAME"],
                                             ((HttpContextFactory.Current.Request.ServerVariables["SERVER_PORT"] != "80")
                                                  ? String.Concat(":", HttpContextFactory.Current.Request.ServerVariables["SERVER_PORT"])
                                                  : String.Empty));
        }

        private string GetLastestVersionId()
        {
            return CurrentContent == null ? String.Empty : CurrentContent.Version.ToString();
        }
    }
}
