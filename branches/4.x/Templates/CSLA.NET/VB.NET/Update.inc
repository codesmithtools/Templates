<%=Indent(0, True)%>#Region " Data Access - Insert "
<% If objInfo.IsGeneratedBase AndAlso objInfo.ParentType.Length>0 Then %>
<%=Indent(i, True)%>Friend Sub Insert(Of <%=objInfo.ParentType%> As <%=objInfo.ParentNameAndSuffix%>)(<%If(objInfo.UseAdoTransaction) Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>, ByVal parent As <%=objInfo.ParentType%>)
<% Else %>
<%=Indent(i, True)%>Friend Sub Insert(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %><%If objInfo.ParentType.Length > 0 Then %>, ByVal parent As <%=objInfo.ParentType%><% End If %>)
<% End If %>
<% i+=1 %>
<%=Indent(i)%>If Not IsDirty Then Return
<%=Indent(i)%>ExecuteInsert(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If%><%If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>)
<%=Indent(i)%>MarkOld()
<%=Indent(i, True)%>UpdateChildren(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
<% i-=1 %>
<%=Indent(i)%>End Sub
<% If objInfo.IsGeneratedBase AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteInsert(Of <%=objInfo.ParentType%> As <%=objInfo.ParentNameAndSuffix%>)(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<%End If%>, ByVal parent As <%=objInfo.ParentType%>)
<% Else %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteInsert(<% If(objInfo.UseAdoTransaction) Then%>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<%End If%><%If(objInfo.ParentType.Length>0) Then%>, ByVal parent As <%=objInfo.ParentType%><%End If%>)
<% End If %>
<% i+=1 %>
<% If objInfo.IsGeneratedPartial Then%>
<%=Indent(i)%>If Not OnExecuteInsert(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %><%If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>) Then Return
<% End If %>
<%=Indent(i)%>Using cm As SqlCommand = <% If objInfo.UseAdoTransaction Then %>tr.Connection<% Else %>cn<% End If %>.CreateCommand()
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>cm.Transaction = tr
<% End If %>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure
<%=Indent(i)%>cm.CommandText = "<%=objInfo.InsertCommandText%>"
<%=Indent(i)%>AddInsertParameters(cm<% If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>)
<%=Indent(i)%>cm.ExecuteNonQuery()
<%=GetInsertReturnParameterStatements(objInfo, i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnExecuteInsertComplete(cm<% If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>)
<%End If%>
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<% If objInfo.IsGeneratedBase AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub AddInsertParameters(Of <%=objInfo.ParentType%> As <%=objInfo.ParentNameAndSuffix%>)(ByVal cm As SqlCommand, ByVal parent As <%=objInfo.ParentType%>)
<% Else %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub AddInsertParameters(ByVal cm As SqlCommand<%If(objInfo.ParentType.Length>0) Then %>, ByVal parent As <%=objInfo.ParentType%><% End If %>)
<%End If%>
<% i+=1 %>
<% If objInfo.ParentType.Length > 0 AndAlso objInfo.IsUserClass Then %>
<%=Indent(i)%>'todo: If parent use identity key, fix fk member with value from parent
<%End If%>
<%=GetInsertParameters(objInfo, i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnAddInsertParametersComplete(cm<%If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>)
<%End If%>
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(0, True)%>#End Region ' Data Access - Insert
<%=Indent(0, True)%>#Region " Data Access - Update "
<% If objInfo.IsGeneratedBase AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i, True)%>Friend Sub Update(Of <%=objInfo.ParentType%> As <%=objInfo.ParentNameAndSuffix%>)(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>, ByVal parent As <%=objInfo.ParentType%>)
<% Else %>
<%=Indent(i, True)%>Friend Sub Update(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<%End If%><%If objInfo.ParentType.Length > 0 Then %>, ByVal parent As <%=objInfo.ParentType%><% End If %>)
<% End If %>
<% i+=1 %>
<%=Indent(i)%>If Not IsDirty Then Return
<%=Indent(i)%>If MyBase.IsDirty Then
<% i+=1 %>
<%=Indent(i)%>ExecuteUpdate(<%If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %><% If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>)
<%=Indent(i)%>MarkOld()
<% i-=1 %>
<%=Indent(i)%>End If
<%=Indent(i, True)%>UpdateChildren(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
<% i-=1 %>
<%=Indent(i)%>End Sub
<% If objInfo.IsGeneratedBase AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteUpdate(Of <%=objInfo.ParentType%> As <%=objInfo.ParentNameAndSuffix%>)(<%If(objInfo.UseAdoTransaction) Then%>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>, ByVal parent As <%=objInfo.ParentType%>)
<% Else %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteUpdate(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<%End If%><%If(objInfo.ParentType.Length>0) Then%>, ByVal parent As <%=objInfo.ParentType%><% End If %>)
<% End If %>
<% i+=1 %>
<%=Indent(i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>If Not onExecuteUpdate(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %><% If objInfo.ParentType.Length > 0 Then%>, parent<% End If %>) Then Return
<% End If %>
<%=Indent(i)%>Using cm As SqlCommand = <% If objInfo.UseAdoTransaction Then %>tr.Connection<% Else %>cn<% End If %>.CreateCommand()
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>cm.Transaction = tr
<% End If %>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure
<%=Indent(i)%>cm.CommandText = "<%=objInfo.UpdateCommandText%>"
<%=Indent(i)%>AddUpdateParameters(cm<% If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>)
<%=Indent(i)%>cm.ExecuteNonQuery()
<%=GetUpdateReturnParameterStatements(objInfo, i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnExecuteUpdateComplete(cm<% If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<% If objInfo.IsGeneratedBase AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub AddUpdateParameters(Of <%=objInfo.ParentType%> As <%=objInfo.ParentNameAndSuffix%>)(ByVal cm As SqlCommand, ByVal parent As <%=objInfo.ParentType%>)
<% Else %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub AddUpdateParameters(ByVal cm As SqlCommand<% If objInfo.ParentType.Length > 0 Then %>, ByVal parent As <%=objInfo.ParentType%><% End If %>)
<% End If %>
<% i+=1 %>
<%=GetUpdateParameters(objInfo, i)%>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnAddUpdateParametersComplete(cm<% If objInfo.ParentType.Length > 0 Then %>, parent<% End If %>)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub UpdateChildren(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>)
<% i+=1 %>
<% If objInfo.HasChild Then %>
<%	Dim cast As string = iif(objInfo.IsGeneratedBase, "(T)", "" )
	For Each prop As PropertyInfo In objInfo.ChildCollection
	If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%><%=string.Format("{0}.Update(tr, {1}Me)", prop.MemberName, cast)%>
<% Else %>
<%=Indent(i)%><%=string.Format("{0}.Update(cn, {1}Me)", prop.MemberName, cast)%>
<% End If : Next %>
<% End If %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnUpdateChildrenComplete(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>)
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(0, True)%>#End Region ' Data Access - Update
