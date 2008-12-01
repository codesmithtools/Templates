<% If objInfo.IsUserClass AndAlso objInfo.UseSecurity Then %>
<%=Indent(0, True)%>#Region " Authorization Rules "
<% If Not objInfo.IsCollection Then %>
<% If Not objInfo.PropertyAuthorization = PropertyAccessSecurity.None Then %>
<%=Indent(i, True)%>Protected Overrides Sub AddAuthorizationRules()
<%i+=1%>
<% If objInfo.PropertyAuthorization = PropertyAccessSecurity.Read OrElse objInfo.PropertyAuthorization = PropertyAccessSecurity.Both Then %>
<%=Indent(i)%>'TODO: Define authorization rules in <%=objInfo.Name%>
<% For Each prop As PropertyInfo In objInfo.Properties %>
<%=Indent(i)%>'AuthorizationRules.AllowRead("<%= prop.Name %>", "<%=objInfo.Name%>ReadGroup")
<% If prop.Type = "SmartDate" Then %>
<%=Indent(i)%>'AuthorizationRules.AllowRead("<%= prop.Name %>String", "<%=objInfo.Name%>ReadGroup")
<% End If %>
<% Next %>
<% End If %>
<% If objInfo.PropertyAuthorization = PropertyAccessSecurity.Write OrElse objInfo.PropertyAuthorization = PropertyAccessSecurity.Both Then %>

<% For Each prop As PropertyInfo In objInfo.Properties : If Not prop.IsReadOnly AndAlso Not prop.IsDbComputed Then %>
<% If prop.Type = "SmartDate" Then %>
<%=Indent(i)%>'AuthorizationRules.AllowWrite("<%= prop.Name %>String", "<%=objInfo.Name%>WriteGroup")
<% Else %>
<%=Indent(i)%>'AuthorizationRules.AllowWrite("<%= prop.Name %>", "<%=objInfo.Name%>WriteGroup")
<% End If %>
<% End If : Next %>
<% End If%>
<% i-=1 %>
<%=Indent(i)%>End Sub
<% End If%>
<% End If%>
<% If Not objInfo.IsChild OrElse objInfo.CslaObjectType = ObjectType.EditableSwitchable Then %>
<%=Indent(i, True)%>Public Shared Function CanGetObject() As Boolean
<% i+=1 %>
<%=Indent(i)%>'TODO: Define CanGetObject permission in <%=objInfo.Name%>
<%=Indent(i)%>Return True
<%=Indent(i)%>'If Csla.ApplicationContext.User.IsInRole("<%=objInfo.Name%>ViewGroup") Then
<%=Indent(i)%>'<%=Indent(1)%>Return True
<%=Indent(i)%>'End If
<%=Indent(i)%>'Return False
<% i-=1 %>
<%=Indent(i)%>End Function
<% If Not objInfo.IsReadOnly Then%>
<%=Indent(i, True)%>Public Shared Function CanAddObject() As Boolean
<%i+=1%>
<%=Indent(i)%>'TODO: Define CanAddObject permission in <%=objInfo.Name%>
<%=Indent(i)%>Return True
<%=Indent(i)%>'If Csla.ApplicationContext.User.IsInRole("<%=objInfo.Name%>AddGroup") Then
<%=Indent(i)%>'<%=Indent(1)%>Return True
<%=Indent(i)%>'End If
<%=Indent(i)%>'Return False
<%i-=1%>
<%=Indent(i)%>End Function
<%=Indent(i, True)%>Public Shared Function CanEditObject() As Boolean
<%i+=1%>
<%=Indent(i)%>'TODO: Define CanEditObject permission in <%=objInfo.Name%>
<%=Indent(i)%>Return True
<%=Indent(i)%>'If Csla.ApplicationContext.User.IsInRole("<%=objInfo.Name%>EditGroup") Then
<%=Indent(i)%>'<%=Indent(1)%>Return True
<%=Indent(i)%>'End If
<%=Indent(i)%>'Return False
<%i-=1%>
<%=Indent(i)%>End Function
<%=Indent(i, True)%>Public Shared Function CanDeleteObject() As Boolean
<%i+=1%>
<%=Indent(i)%>'TODO: Define CanDeleteObject permission in <%=objInfo.Name%>
<%=Indent(i)%>Return True
<%=Indent(i)%>'If Csla.ApplicationContext.User.IsInRole("<%=objInfo.Name%>DeleteGroup") Then
<%=Indent(i)%>'<%=Indent(1)%>Return True
<%=Indent(i)%>'End If
<%=Indent(i)%>'Return False
<%i-=1%>
<%=Indent(i)%>End Function
<% End If : End If %>
<%=Indent(0, True)%>#End Region ' Authorization Rules
<% End If %>
