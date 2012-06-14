using umbraco;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;

namespace UmbraCodeFirst.Extensions
{
    internal static class DocumentExtensions
    {

        /// <summary>
        /// Publishes this document
        /// </summary>
        /// <param name="document">an umbraco.cms.businesslogic.web.Document object</param>
        /// <param name="useAdminUser">if true then publishes under the context of User(0), if false uses current user</param>
        /// <returns>the same document object on which this is an extension method</returns>
        public static Document Publish(this Document document, bool useAdminUser)
        {
            if (useAdminUser)
            {
                document.Publish(Constants.DefaultBackendUser);
            }
            else
            {
                if (User.GetCurrent() != null)
                {
                    document.Publish(User.GetCurrent());
                }
            }

            library.UpdateDocumentCache(document.Id);

            return document;
        }
    }
}