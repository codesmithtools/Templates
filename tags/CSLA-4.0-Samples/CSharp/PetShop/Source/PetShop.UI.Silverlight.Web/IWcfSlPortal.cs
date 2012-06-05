using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CsSlSample.Web
{
    // NOTE: If you change the interface name "IWcfSlPortal" here, you must also update the reference to "IWcfSlPortal" in Web.config.
    [ServiceContract]
    public interface IWcfSlPortal
    {
        [OperationContract]
        void DoWork();
    }
}
