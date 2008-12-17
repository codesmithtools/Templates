<%=Indent(0, True)%>#Region " Factory Methods "
<% If objInfo.IsSingle OrElse objInfo.IsGeneratedPartial OrElse objInfo.IsUserBase Then %>
<%=Indent(i, True)%>Private Sub New()
<%=Indent(i)%>' require use of factory method 
<%=Indent(i)%>End Sub
<% ElseIf objInfo.IsGeneratedBase Then %>
<%=Indent(i, True)%>Protected Sub New()
<%=Indent(i)%>' require use of factory method in user object
<%=Indent(i)%>End Sub
<% End If %>
<% 'all in generated class except when base class with authorization turned on
    If (objInfo.IsSingle OrElse objInfo.IsGeneratedPartial OrElse (objInfo.IsGeneratedBase AndAlso Not objInfo.UseSecurity) OrElse (objInfo.IsUserBase AndAlso objInfo.UseSecurity)) Then %>
<% If Not objInfo.IsReadOnly Then %>
<%=Indent(i, True)%>Public Shared Function <%=objInfo.NewMethodName%>(<%=GetFactoryNewDeclarationArguments(objInfo)%>) As <%=objInfo.Type%>
<% i+=1 %>
<% If objInfo.UseSecurity Then %>
<%=Indent(i)%>If Not CanAddObject() Then
<%=Indent(i+1)%>Throw New System.Security.SecurityException("User not authorized to add a <%=objInfo.Name%>")
<%=Indent(i)%>End If
<% End If %>
<%=Indent(i)%>Return DataPortal.Create(Of <%=objInfo.Type%>)(<% Dim cPars As String = GetFactoryNewCallArguments(objInfo): If cPars.Length > 0 Then %>New Criteria(<%=cPars%>)<%End If%>)
<% i-=1 %>
<%=Indent(i)%>End Function
<% End If %>
<%=Indent(i, True)%>Public<% If objInfo.IsUserBase Then %> Overloads<%End If%> Shared Function <%=objInfo.GetMethodName%>(<%=GetFactoryDeclarationArguments(objInfo)%>) As <%=objInfo.Type%>
<% i+=1 %>
<% If objInfo.UseSecurity Then %>
<%=Indent(i)%>If Not CanGetObject() Then
<%=Indent(i+1)%>Throw New System.Security.SecurityException("User not authorized to view a <%=objInfo.Name%>")
<%=Indent(i)%>End If
<% End If %>
<%=Indent(i)%>Return DataPortal.Fetch(Of <%=objInfo.Type%>)(New Criteria(<%=GetFactoryCallArguments(objInfo)%>))
<% i-=1 %>
<%=Indent(i)%>End Function
<% If Not objInfo.IsReadOnly Then %>
<%=Indent(i, True)%>Public Shared Sub <%=objInfo.DeleteMethodName%>(<%=GetFactoryDeclarationArguments(objInfo)%>)
<% i+=1 %>
<% If objInfo.UseSecurity Then %>
<%=Indent(i)%>If Not CanDeleteObject() Then
<%=Indent(i+1)%>Throw New System.Security.SecurityException("User not authorized to remove a <%=objInfo.Name%>")
<%=Indent(i)%>End If
<% End If %>
<%=Indent(i)%>DataPortal.Delete(new Criteria(<%=GetFactoryCallArguments(objInfo)%>))
<% i-=1 %>
<%=Indent(i)%>End Sub
<% If objInfo.UseSecurity Then %>
<%=Indent(i, True)%>Public Overrides Function Save() As <%=objInfo.Type%>
<% i+=1 %>
<%=Indent(i)%>If IsDeleted AndAlso Not CanDeleteObject() Then
<%=Indent(i+1)%>Throw New System.Security.SecurityException("User not authorized to remove a <%=objInfo.Name%>")
<%=Indent(i)%>ElseIf IsNew AndAlso Not CanAddObject() Then
<%=Indent(i+1)%>Throw New System.Security.SecurityException("User not authorized to add a <%=objInfo.Name%>")
<%=Indent(i)%>ElseIf Not CanEditObject() Then
<%=Indent(i+1)%>Throw New System.Security.SecurityException("User not authorized to update a <%=objInfo.Name%>")
<%=Indent(i)%>End If
<%=Indent(i)%>Return MyBase.Save()
<% i-=1 %>
<%=Indent(i)%>End Function<% End If : End If : End If %>
<%=Indent(0, True)%>#End Region ' Factory Methods