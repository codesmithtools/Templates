using System;
using System.Data;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Web.DynamicData;

namespace DynamicDataProject
{
    public partial class Enumeration_EditField : FieldTemplateUserControl
    {
        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            var enumList = Enum.GetValues(Column.ColumnType);
            DropDownList1.DataSource = enumList;
            DropDownList1.DataBind();
        }

        protected override void ExtractValues(System.Collections.Specialized.IOrderedDictionary dictionary)
        {
            dictionary[Column.Name] = ConvertEditedValue(DropDownList1.SelectedValue);
        }

        protected void DropDownList1_DataBound(object sender, EventArgs e)
        {
            if (Mode == DataBoundControlMode.Edit)
            {
                ListItem item = DropDownList1.Items.FindByValue(FieldValueString);
                if (item != null)
                {
                    DropDownList1.SelectedValue = FieldValueString;
                }
            }
        }

        public override Control DataControl
        {
            get { return DropDownList1; }
        }
    }
}
