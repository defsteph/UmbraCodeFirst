using System;
using System.Web.UI;
using UmbraCodeFirst.Factories;
using umbraco.presentation;

namespace UmbraCodeFirst.UI
{
    public abstract class UserControlBase<T> : UserControl where T : UmbracoModelBase
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


        private T _model;
        protected virtual T Model
        {
            get
            {
                if (_model != null)
                    return _model;

                if (InPreviewMode && CurrentVersion != Guid.Empty)
                {
                    _model = ModelFactory.Instance.GetModelFromDatabase<T>(UmbracoContext.Current.PageId, CurrentVersion);
                }

                return _model ?? (_model = ModelFactory.Instance.GetModel<T>(UmbracoContext.Current.PageId));
            }
        }
    }
}