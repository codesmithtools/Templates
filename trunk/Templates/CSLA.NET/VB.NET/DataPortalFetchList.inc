<%=Indent(0, True)%>#Region " Data Access - Fetch "
<% If objInfo.UseTransactionalAttribute Then %>
<%=Indent(i, True)%><Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)> _ 
<%=Indent(i)%>Private Overloads Sub DataPortal_Fetch(ByVal criteria As FilterCriteria)
<% Else %>
<%=Indent(i, True)%>Private Overloads Sub DataPortal_Fetch(ByVal criteria As FilterCriteria)
<% End If %>
<% i+=1 %>
<%=Indent(i)%>RaiseListChangedEvents = False
<% If(objInfo.IsReadOnly) Then%>
<%=Indent(i)%>IsReadOnly = False
<% End If %>
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
<%=Indent(i)%>ExecuteFetch(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>, criteria)
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>tr.Commit()
<% i-=1 %>
<%=Indent(i)%>Catch
<% i+=1 %>
<%=Indent(i)%>tr.Rollback()
<%=Indent(i)%>Throw
<% i-=1 %>
<%=Indent(i)%>End Try
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Using
<% If objInfo.IsReadOnly Then %>
<%=Indent(i)%>IsReadOnly = True
<% End If %>
<%=Indent(i)%>RaiseListChangedEvents = True
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteFetch(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>, ByVal criteria As FilterCriteria)
<% i+=1 %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>If Not OnExecuteFetch(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>, criteria) Then Return
<% End If %>
<%=Indent(i)%>Using cm As SqlCommand = <% If objInfo.UseAdoTransaction Then %>tr.Connection<% Else %>cn<% End If %>.CreateCommand()
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>cm.Transaction = tr
<% End If %>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure
<%=Indent(i)%>cm.CommandText = "<%=objInfo.FetchCommandText%>"
<%=GetFilterParameters(objInfo, i)%>
<%=Indent(i)%>Using dr As SafeDataReader = New SafeDataReader(cm.ExecuteReader())
<% i+=1 %>
<%=Indent(i)%>While dr.Read()
<%=Indent(i+1)%>Me.Add(<%=objInfo.Child%><% If objInfo.IsGeneratedBase Then %>(Of <%=GenericTypeChildParameter%>)<% End If %>.<%=objInfo.GetChildMethodName%>(dr))
<% i-=1 %>
<%=Indent(i)%>End While
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnExecuteFetchComplete(cm, criteria)
<% End If %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(0, True)%>#End Region ' Data Access - Fetch