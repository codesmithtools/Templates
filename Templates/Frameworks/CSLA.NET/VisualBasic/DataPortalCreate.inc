<%=Indent(0, True)%>#Region " Data Access - Create "
<%=Indent(i, True)%><RunLocal> _
<% If objInfo.HasIdentity OrElse objInfo.HasObjectGeneratedKey Then %>
<%=Indent(i)%>Protected Overloads Sub DataPortal_Create(ByVal criteria As Object)
<% Else %>
<%=Indent(i)%>Private Overloads Sub DataPortal_Create(ByVal criteria As Criteria)
<% End If %>
<% i+=1 %>
<% If Not objInfo.IsCollection AndAlso Not objInfo.HasIdentity AndAlso Not objInfo.HasObjectGeneratedKey Then
   For Each prop As PropertyInfo In objInfo.UniqueProperties %>
<%=Indent(i)%><%=String.Format("{0} = criteria.{1}", prop.MemberName, prop.Name) %>
<% Next : End if %>
<% If objInfo.IsChild AndAlso objInfo.CslaObjectType <> ObjectType.EditableSwitchable Then %>
<%=Indent(i)%>MarkAsChild()
<% End If %>
<% If Not objInfo.IsReadOnly AndAlso Not objInfo.IsCollection Then %>
<%=Indent(i)%>ValidationRules.CheckRules()
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(0, True)%>#End Region ' Data Access - Create