<%=Indent(0, True)%>#Region " Data Access - Fetch "
<%=Indent(i, True)%>Private Sub Fetch(ByVal dr As SafeDataReader)
<% i+=1 %>
<%=Indent(i)%>FetchObject(dr)
<% If Not objInfo.IsReadOnly Then %>
<%=Indent(i)%>MarkOld()
<%=Indent(i)%>ValidationRules.CheckRules()
<% End If %>
<%=Indent(i, True)%>'load child object(s)
<%=Indent(i)%>FetchChildren(dr)
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub FetchObject(ByVal dr As SafeDataReader)
<% i+=1 %>
<% For Each prop As PropertyInfo In objInfo.Properties : If prop.HasDbColumn Then %>
<%=Indent(i)%><%=GetReaderAssignmentStatement(prop) %>
<% End If : Next %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnFetchObjectComplete(dr)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub FetchChildren(ByVal dr As SafeDataReader)
<% i+=1 %>
<% If objInfo.HasChild Then %>
<% For Each prop As PropertyInfo In objInfo.ChildCollection %>
<%=Indent(i)%>dr.NextResult()
<%=Indent(i)%><%=string.Format("{0} = {1}.Get{1}(dr)", prop.MemberName, prop.Type)%>
<% Next %>
<% End If %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnFetchChildrenComplete(dr)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(0, True)%>#End Region ' Data Access - Fetch