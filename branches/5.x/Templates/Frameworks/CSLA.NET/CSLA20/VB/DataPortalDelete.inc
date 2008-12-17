<%=Indent(0, True)%>#Region " Data Access - Delete "
<% If objInfo.UseTransactionalAttribute Then%>
<%=Indent(i, True)%><Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)> _ 
<%=Indent(i)%>Protected Overrides Sub DataPortal_DeleteSelf()
<% Else %>
<%=Indent(i, True)%>Protected Overrides Sub DataPortal_DeleteSelf()
<% End If %>
<%=Indent(i+1)%>DataPortal_Delete(New Criteria(<%=GetDeleteSelfCriteriaCallArguments(objInfo)%>))
<%=Indent(i)%>End Sub
<% If objInfo.UseTransactionalAttribute Then%>
<%=Indent(i, True)%><Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)> _ 
<%=Indent(i)%>Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)
<% Else %>
<%=Indent(i, True)%>Private Overloads Sub DataPortal_Delete(ByVal criteria As Criteria)
<% End If %>
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>Dim tr As SqlTransaction
<% End If %>
<%=Indent(i)%>Using cn As SqlConnection = New SqlConnection(<%=objInfo.DbConnection%>)
<% i+=1 %>
<%=Indent(i)%>cn.Open()
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>tr = cn.BeginTransaction()
<%=Indent(i)%>Try
<% i+=1 %>
<% End If %>
<%=Indent(i)%>ExecuteDelete(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>, criteria)
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>tr.Commit()
<% i-=1 %>
<%=Indent(i)%>Catch
<%=Indent(i+1)%>tr.Rollback()
<%=Indent(i+1)%>Throw
<%=Indent(i)%>End Try
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteDelete(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>, ByVal criteria As Criteria)
<% i+=1 %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>If Not OnExecuteDelete(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>, criteria) Then Return
<% End If %>
<%=Indent(i)%>Using cm As SqlCommand = <% If objInfo.UseAdoTransaction Then %>tr.Connection<% Else %>cn<% End If %>.CreateCommand()
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>cm.Transaction = tr
<% End If %>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure
<%=Indent(i)%>cm.CommandText = "<%=objInfo.DeleteCommandText%>"

<%=GetFetchParameters(objInfo, i)%>
<%=Indent(i, True)%>cm.ExecuteNonQuery()
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnExecuteDeleteComplete(cm, criteria)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(0, True)%>#End Region ' Data Access - Delete
 