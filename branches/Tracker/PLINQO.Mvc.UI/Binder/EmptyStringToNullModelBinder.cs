using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PLINQO.Mvc.UI.Binder
{
 
  public sealed class EmptyStringToNullModelBinder : DefaultModelBinder
  {
    protected override void SetProperty(ControllerContext controllerContext,
      ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor, object value)
    {
      if ((value != null)
          && (value.GetType() == typeof(string)
              && ((string)value).Length == 0))
      {
        value = null;
      }

      base.SetProperty(controllerContext, bindingContext, propertyDescriptor, value);
    }
  }
}

