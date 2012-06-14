
using umbraco.BusinessLogic;

namespace UmbraCodeFirst
{
    public static class Constants
    {
        public static User DefaultBackendUser = new User(0);
        public const string VersionParameterName = "pageVersion";
        public const string UmbracoContextPreviewKey = "UMB_PREVIEW";
        public const string HideFromNavigationPropertyName = "umbracoNaviHide";
        public const string FetchDataPropertyName = "umbracoInternalRedirectId";
        public const string RedirectPropertyName = "umbracoRedirect";
        public const string PageUrlNamePropertyName = "umbracoUrlName";
        public const string PageUrlAliasPropertyName = "umbracoUrlAlias";
        public const string DefaultIdParameterName = "id";
    }
}
