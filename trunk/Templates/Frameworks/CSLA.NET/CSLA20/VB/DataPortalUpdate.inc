<%=Indent(0, true)%>#Region " Data Access - Insert "
<% If objInfo.UseTransactionalAttribute Then %>
<%=Indent(i, True)%><Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)> _ 
<%=Indent(i)%>Protected Overrides Sub DataPortal_Insert()
<% Else %>
<%=Indent(i, True)%>Protected Overrides Sub DataPortal_Insert()
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
<%=Indent(i)%>ExecuteInsert(<%If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
<%=Indent(i, True)%>'update child object(s)
<%=Indent(i)%>UpdateChildren(<%If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i, True)%>tr.Commit()
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
<% If objInfo.CslaObjectType <> ObjectType.EditableSwitchable Then %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteInsert(<%If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>)
<% i+=1 %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>If Not OnExecuteInsert(<%If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>) Then Return
<%End If%>
<%=Indent(i)%>Using cm As SqlCommand = <%If objInfo.UseAdoTransaction Then %>tr.Connection<% Else %>cn<% End If %>.CreateCommand()
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>cm.Transaction = tr
<% End If %>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure
<%=Indent(i)%>cm.CommandText = "<%=objInfo.InsertCommandText%>"
<%=Indent(i, True)%>AddInsertParameters(cm)
<%=Indent(i, True)%>cm.ExecuteNonQuery()
<%=GetInsertReturnParameterStatements(objInfo, i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnExecuteInsertComplete(cm)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub AddInsertParameters(ByVal cm As SqlCommand)
<% i+=1 %>
<%=GetInsertParameters(objInfo, i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnAddInsertParametersComplete(cm)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub<% End If %>

<%=Indent(0)%>#End Region 'Data Access - Insert

<%=Indent(0)%>#Region " Data Access - Update "
<% If objInfo.UseTransactionalAttribute Then%>
<%=Indent(i, True)%><Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)> _ 
<%=Indent(i)%>Protected Overrides Sub DataPortal_Update()
<% Else %>
<%=Indent(i, True)%>Protected Overrides Sub DataPortal_Update()
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
<%=Indent(i)%>If MyBase.IsDirty Then
<%=Indent(i+1)%>ExecuteUpdate(<%If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
<%=Indent(i)%>End If
<%=Indent(i, True)%>' update child object(s)
<%=Indent(i)%>UpdateChildren(<%If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
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
<%=Indent(i, True)%>End Sub
<% If objInfo.CslaObjectType <> ObjectType.EditableSwitchable Then %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteUpdate(<%If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>)
<% i+=1 %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>If Not OnExecuteUpdate(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>) Then Return
<% End If %>
<%=Indent(i)%>Using cm As SqlCommand = <% If objInfo.UseAdoTransaction Then %>tr.Connection<% Else %>cn<% End If %>.CreateCommand()
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>cm.Transaction = tr
<% End If %>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure
<%=Indent(i)%>cm.CommandText = "<%=objInfo.UpdateCommandText%>"
<%=Indent(i, True)%>AddUpdateParameters(cm)
<%=Indent(i, True)%>cm.ExecuteNonQuery()
<%=GetUpdateReturnParameterStatements(objInfo, i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnExecuteUpdateComplete(cm)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub AddUpdateParameters(ByVal cm As SqlCommand)
<% i+=1 %>
<%=GetUpdateParameters(objInfo, i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnAddUpdateParametersComplete(cm)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub UpdateChildren(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>)
<% i+=1 %>
<% If objInfo.HasChild Then %>
<% Dim cast As String = "Me" %>
<% If objInfo.IsGeneratedBase Then %>
<% cast="CType(Me, T)" %>
<% End If %>
<%	For Each prop As PropertyInfo In objInfo.ChildCollection
	If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%><%=string.Format("{0}.Update(tr, {1})", prop.MemberName, cast)%>
<% Else %>
<%=Indent(i)%><%=string.Format("{0}.Update(cn, {1})", prop.MemberName, cast)%>
<% End If : Next : End If %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnUpdateChildrenComplete(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub
<% End If %>

<%=Indent(0)%>#End Region 'Data Access - Update
 