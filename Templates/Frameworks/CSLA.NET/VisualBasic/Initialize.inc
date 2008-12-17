<% If objInfo.IsUserPartial Then %>
<%=Indent(0, True)%>#Region " Initialize "
<%=Indent(i, True)%>Protected Overrides Sub Initialize()
<% i+=1 %>
<%=Indent(i, True)%>'todo: Uncomment the required events and add the Methods in this class
<%=Indent(i, True)%>'addCustomRulesDelegate = AddressOf AddCustomRules
<% If Not objInfo.IsChild Then %>
<%=Indent(i)%>'executeFetchDelegate = AddressOf ExecuteFetch
<%=Indent(i)%>'executeFetchCompleteDelegate = AddressOf ExecuteFetchComplete
<% End If %>
<%=Indent(i)%>'fetchObjectCompleteDelegate = AddressOf FetchObjectComplete
<%=Indent(i)%>'fetchChildrenCompleteDelegate = AddressOf FetchChildrenComplete
<%=Indent(i)%>'executeInsertDelegate = AddressOf ExecuteInsert
<%=Indent(i)%>'executeInsertCompleteDelegate = AddressOf ExecuteInsertComplete
<%=Indent(i)%>'addInsertParamatersCompleteDelegate = AddressOf AddInsertParamatersComplete
<%=Indent(i)%>'executeUpdateDelegate = AddressOf ExecuteUpdate
<%=Indent(i)%>'executeUpdateCompleteDelegate = AddressOf ExecuteUpdateComplete
<%=Indent(i)%>'addUpdateParamatersCompleteDelegate = AddressOf AddUpdateParamatersComplete
<%=Indent(i)%>'updateChildrenCompleteDelegate = AddressOf UpdateChildrenComplete
<%=Indent(i)%>'executeDeleteDelegate = AddressOf ExecuteDelete
<%=Indent(i)%>'executeDeleteCompleteDelegate = AddressOf ExecuteDeleteComplete
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%>Protected Overrides Sub OnDeserialized(ByVal context As System.Runtime.Serialization.StreamingContext)
<%=Indent(i+1)%>Initialize()
<%=Indent(i)%>End Sub
<%=Indent(0, True)%>#End Region ' Inititalize
<% End If %>
