using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using UmbraCodeFirst.MacroPropertyTypes;
using umbraco.cms.businesslogic;

namespace UmbraCodeFirst.Sample.Domain.MacroPropertyTypes
{
    public class DictionaryItemMacroPropertyType : MacroPropertyType
    {
        protected IList<Dictionary.DictionaryItem> List = new List<Dictionary.DictionaryItem>();

        public override bool Multiple
        {
            get { return false; }
            set { if (value) throw new NotSupportedException("The DictionaryItemMacroPropertyType does not support selection of multiple options."); }
        }

        private static void AddToList(Dictionary.DictionaryItem parent, ref IList<Dictionary.DictionaryItem> list)
        {
            list.Add(parent);
            if (!parent.hasChildren)
                return;
            foreach (var child in parent.Children)
                AddToList(child, ref list);
        }

        protected override void BeforeRenderItems()
        {
            base.BeforeRenderItems();
            foreach (var parent in Dictionary.getTopMostItems)
                AddToList(parent, ref List);
        }

        protected override void RenderItems()
        {
            DataTextField = "key";
            DataValueField = "key";
            DataSource = List;
            DataBind();
            Items.Insert(0, new ListItem("Select", ""));
        }
    }
}