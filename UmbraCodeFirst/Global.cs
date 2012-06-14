using System;

namespace UmbraCodeFirst
{
    public class Global : umbraco.Global
    {
        protected override void Application_Start(object sender, EventArgs e)
        {
            base.Application_Start(sender, e);

            Synchronization.SynchronizationManager.Instance.Synchronize();
        }
    }
}