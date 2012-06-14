using System;
using UmbraCodeFirst.Attributes;
using UmbraCodeFirst.Sample.Domain.DocumentTypes;
using UmbraCodeFirst.UI;

namespace UmbraCodeFirst.Sample.masterpages
{
    [DocumentTypeTemplate(Name = "Inherited Template", MasterTemplate = typeof(Master))]
    public partial class Inherited : MasterPageBase<StandardModel>
    {

    }
}