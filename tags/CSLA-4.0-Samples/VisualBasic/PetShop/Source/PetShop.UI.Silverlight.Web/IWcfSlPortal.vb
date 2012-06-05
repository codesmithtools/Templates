Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.Text

Namespace CsSlSample.Web
    ' NOTE: If you change the interface name "IWcfSlPortal" here, you must also update the reference to "IWcfSlPortal" in Web.config.
    <ServiceContract()> _
    Public Interface IWcfSlPortal
        <OperationContract()> _
        Sub DoWork()
    End Interface
End Namespace