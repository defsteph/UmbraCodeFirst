using System;

namespace UmbraCodeFirst
{
    public class Global : System.Web.HttpApplication
    {
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            Synchronization.SynchronizationManager.Instance.Synchronize();
        }
    }
}