<%=Indent(0, True)%>#Region " Data Access - Delete "
<%=Indent(i, True)%>Friend Sub DeleteSelf(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>)
<% i+=1 %>
<%=Indent(i)%>If Not IsDirty Then Return
<%=Indent(i)%>If IsNew Then Return
<% If objInfo.CslaObjectType = ObjectType.EditableSwitchable Then %>
<%=Indent(i)%>ExecuteDelete(<%If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>, New Criteria(<%=GetDeleteSelfCriteriaCallArguments(objInfo)%>))
<% Else %>
<%=Indent(i)%>ExecuteDelete(<%If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
<% End If %>
<%=Indent(i)%>MarkNew()
<% i-=1 %>
<%=Indent(i)%>End Sub
<% If objInfo.CslaObjectType <> ObjectType.EditableSwitchable Then %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteDelete(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>)
<% i+=1 %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>If Not OnExecuteDelete(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>) Then Return
<% End If %>
<%=Indent(i)%>Using cm As SqlCommand = <% If objInfo.UseAdoTransaction Then %>tr.Connection<% Else %>cn<% End If %>.CreateCommand()
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>cm.Transaction = tr
<% End If %>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure
<%=Indent(i)%>cm.CommandText = "<%=objInfo.DeleteCommandText%>"
<%=GetFetchParameters(objInfo, "", "Me", i)%>
<%=Indent(i, True)%>cm.ExecuteNonQuery()
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>onExecuteDeleteComplete(cm)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<% End If %>
<%=Indent(0, True)%>#End Region ' Data Access - Delete
