
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Tester.Data
{
    public partial class TesterDataContext
    {
        #region Extensibility Method Definitions
        //TODO: Uncomment and implement partial method
        //partial void OnCreated()
        //{
        //    
        //}
        #endregion

        //partial void BeforeSubmitChanges()
        //{
        //    if (LastAudit == null)
        //        return;

        //    Audit audit = new Audit();

        //    audit.Source = "Test";

        //    audit.AuditXml = LastAudit.ToXml();
        //    audit.CreatedDate = DateTime.Now;
        //    audit.ModifiedDate = DateTime.Now;

        //    Audit.InsertOnSubmit(audit);
        //}
    }
}