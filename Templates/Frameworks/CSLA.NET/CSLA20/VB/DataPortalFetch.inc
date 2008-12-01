<%=Indent(0, true)%>#Region " Data Access - Fetch "
<% If objInfo.UseTransactionalAttribute Then %>
<%=Indent(i, True)%><Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)> _ 
<%=Indent(i)%>Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
<% Else %>
<%=Indent(i, True)%>Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)
<% End If %>
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then%>
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
<%=Indent(i, True)%>tr.Commit()
<% i-=1 %>
<%=Indent(i)%>Catch
<% i+=1 %>
<%=Indent(i)%>tr.Rollback()
<%=Indent(i)%>Throw
<% i-=1 %>
<%=Indent(i)%>End Try
<% End If%>
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub ExecuteFetch(<% If objInfo.UseAdoTransaction Then %>ByVal tr As SqlTransaction<% Else %>ByVal cn As SqlConnection<% End If %>, ByVal criteria As Criteria)
<% i+=1 %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>If Not OnExecuteFetch(<% If objInfo.UseAdoTransaction Then %>tr<% Else %>cn<% End If %>, criteria) Then Return
<%End If%>
<%=Indent(i)%>Using cm As SqlCommand = <% If objInfo.UseAdoTransaction Then %>tr.Connection<% Else %>cn<% End If %>.CreateCommand()
<% i+=1 %>
<% If objInfo.UseAdoTransaction Then %>
<%=Indent(i)%>cm.Transaction = tr
<% End If %>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure
<%=Indent(i)%>cm.CommandText = "<%=objInfo.FetchCommandText%>"
<%=GetFetchParameters(objInfo, i)%>
<%=Indent(i, True)%>Using dr As SafeDataReader = New SafeDataReader(cm.ExecuteReader())
<% i+=1 %>
<% If objInfo.CslaObjectType = ObjectType.EditableSwitchable Then %>
<%=Indent(i)%>dr.Read()
<% End If %>
<%=Indent(i)%>FetchObject(dr)
<% If Not objInfo.IsReadOnly Then %>
<%=Indent(i)%>ValidationRules.CheckRules()
<% End If %>
<%=Indent(i, True)%>'load child object(s)
<%=Indent(i)%>FetchChildren(dr)
<% i-=1 %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnExecuteFetchComplete(cm, criteria)
<% End If %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Using
<% i-=1 %>
<%=Indent(i)%>End Sub
<% If objInfo.CslaObjectType <> ObjectType.EditableSwitchable Then %>
<%=Indent(i, True)%><%=objInfo.LocalMethodModifiers%> Sub FetchObject(ByVal dr As SafeDataReader)
<% i+=1 %>
<%=Indent(i)%>dr.Read()
<% For Each prop As PropertyInfo In objInfo.Properties : If prop.HasDbColumn Then %>
<%=Indent(i)%><%= GetReaderAssignmentStatement(prop) %>
<% End If : Next %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnFetchObjectComplete(dr)
<% End If%>
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
<% End If %>
<%=Indent(0, True)%>#End Region ' Data Access - Fetch
