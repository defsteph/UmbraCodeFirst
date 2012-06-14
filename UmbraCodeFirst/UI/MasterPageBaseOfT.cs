using System;
using System.Web.UI;
using UmbraCodeFirst.Factories;
using umbraco.presentation;

namespace UmbraCodeFirst.UI
{
    public abstract class MasterPageBase<T> : MasterPage where T : UmbracoPageBase
    {
        protected Guid CurrentVersion = Guid.Empty;

        private bool? _inPreviewMode;
        protected bool InPreviewMode
        {
            get
            {
                if (_inPreviewMode.HasValue)
                    return _inPreviewMode.Value;

                _inPreviewMode = false;

                if (UmbracoContext.Current.InPreviewMode)
                {
                    CurrentVersion = new Guid(UmbracoContext.Current.Request[Constants.UmbracoContextPreviewKey]);
                    _inPreviewMode = true;
                }
                else
                {
                    var hasVersionQueryString = !String.IsNullOrWhiteSpace(UmbracoContext.Current.Request[Constants.VersionParameterName]);
                    var isLoggedOn = UmbracoContext.Current.UmbracoUser != null;

                    if (hasVersionQueryString && isLoggedOn)
                    {
                        CurrentVersion = new Guid(UmbracoContext.Current.Request[Constants.VersionParameterName]);
                        _inPreviewMode = true;
                    }
                }
                return _inPreviewMode.Value;
            }
        }

        private T _currentPage;
        protected virtual T CurrentPage
        {
            get
            {
                if (_currentPage != null)
                    return _currentPage;

                if (InPreviewMode && CurrentVersion != Guid.Empty)
                {
                    _currentPage = PageFactory.Instance.GetPageFromDatabase<T>(UmbracoContext.Current.PageId, CurrentVersion);
                }

                return _currentPage ?? (_currentPage = PageFactory.Instance.GetPage<T>(UmbracoContext.Current.PageId));
            }
        }
    }
}