<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(0, True)%>#Region " Delegation Methods "
<% If Not (objInfo.IsChild AndAlso objInfo.ParentType.Length = 0) Then %>
<%=Indent(i)%>Private Delegate Function predicateEx(of U, V)(obj1 As U,obj2 As V) As Boolean
<%=Indent(i)%>Private Delegate Sub actionEx(of U, V)(obj1 As U, obj2 As V)
<% End If %>
<%=Indent(i)%>Private Delegate Sub RulesAction()
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private addCustomRulesDelegate As rulesAction = Nothing
<%=Indent(i)%>Private Sub OnAddCustomRules()
<% i+=1 %>
<%=Indent(i)%>If Not addCustomRulesDelegate Is Nothing Then
<%=Indent(i+1)%>addCustomRulesDelegate()
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<% If Not objInfo.IsChild Then %>
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private executeFetchDelegate As predicateEx(Of <%If objInfo.UseAdoTransaction Then%>SqlTransaction<%else%>SqlConnection<%End If%>, Criteria) = Nothing
<%=Indent(i)%>Private Function OnExecuteFetch(<%If objInfo.UseAdoTransaction Then%>ByVal tr As SqlTransaction<%else%>ByVal cn As SqlConnection<%End If%>, ByVal criteria As Criteria) As Boolean
<% i+=1 %>
<%=Indent(i)%>If Not executeFetchDelegate Is Nothing Then
<%=Indent(i+1)%>Return executeFetchDelegate(<%If objInfo.UseAdoTransaction Then%>tr<%else%>cn<%End If%>, criteria)
<%=Indent(i)%>End If
<%=Indent(i)%>Return True
<% i-=1 %>
<%=Indent(i)%>End Function
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private executeFetchCompleteDelegate As actionEx(of SqlCommand, Criteria) = Nothing
<%=Indent(i)%>Private Sub OnExecuteFetchComplete(ByVal cm As SqlCommand, ByVal criteria As Criteria)
<% i+=1 %>
<%=Indent(i)%>If Not executeFetchCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>executeFetchCompleteDelegate(cm, criteria)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub<% End If %>
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private fetchObjectCompleteDelegate As Action(Of SafeDataReader) = Nothing
<%=Indent(i)%>Private Sub OnFetchObjectComplete(ByVal dr As SafeDataReader)
<% i+=1 %>
<%=Indent(i)%>If Not fetchObjectCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>fetchObjectCompleteDelegate(dr)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private fetchChildrenCompleteDelegate As Action(of SafeDataReader) = Nothing
<%=Indent(i)%>Private Sub OnFetchChildrenComplete(ByVal dr As SafeDataReader)
<% i+=1 %>
<%=Indent(i)%>If Not fetchChildrenCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>fetchChildrenCompleteDelegate(dr)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><NonSerialized()> _
<% If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i)%>Private executeInsertDelegate As predicateEx(Of <%If objInfo.UseAdoTransaction Then%>SqlTransaction<%else%>SqlConnection<%End If%>, <%=objInfo.ParentType%>) = Nothing
<% Else %>
<%=Indent(i)%>Private executeInsertDelegate As Predicate(Of <%If objInfo.UseAdoTransaction Then%>SqlTransaction<%else%>SqlConnection<%End If%>) = Nothing
<% End If %>
<%=Indent(i)%>Private Function OnExecuteInsert(<%If objInfo.UseAdoTransaction Then%>ByVal tr As SqlTransaction<%else%>ByVal cn As SqlConnection<%End If%><%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, ByVal parent As <%=objInfo.ParentType%><%End If%>) As Boolean
<% i+=1 %>
<%=Indent(i)%>If Not executeInsertDelegate Is Nothing Then
<%=Indent(i+1)%>Return executeInsertDelegate(<%If objInfo.UseAdoTransaction Then%>tr<%else%>cn<%End If%><%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, parent<%End If%>)
<%=Indent(i)%>End If
<%=Indent(i)%>Return True
<% i-=1 %>
<%=Indent(i)%>End Function
<%=Indent(i, True)%><NonSerialized()> _
<% If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i)%>Private executeInsertCompleteDelegate As actionEx(Of SqlCommand, <%=objInfo.ParentType%>) = Nothing
<% Else %>
<%=Indent(i)%>Private executeInsertCompleteDelegate As Action(Of SqlCommand) = Nothing
<% End If %>
<%=Indent(i)%>Private Sub OnExecuteInsertComplete(ByVal cm As SqlCommand<%If(objInfo.IsChild AndAlso objInfo.ParentType.Length>0) Then%>, ByVal parent As <%=objInfo.ParentType%><%End If%>)
<% i+=1 %>
<%=Indent(i)%>If Not executeInsertCompleteDelegate Is  Nothing Then
<%=Indent(i+1)%>executeInsertCompleteDelegate(cm<%If(objInfo.IsChild AndAlso objInfo.ParentType.Length>0) Then%>, parent<%End If%>)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><NonSerialized()> _
<% If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i)%>Private addInsertParamatersCompleteDelegate As actionEx(Of SqlCommand, <%=objInfo.ParentType%>) = Nothing
<% Else %>
<%=Indent(i)%>Private addInsertParamatersCompleteDelegate As Action(Of SqlCommand) = Nothing
<% End If%>
<%=Indent(i)%>Private Sub OnAddInsertParametersComplete(ByVal cm As SqlCommand<%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, ByVal parent As <%=objInfo.ParentType%><%End If%>)
<% i+=1 %>
<%=Indent(i)%>If Not addInsertParamatersCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>addInsertParamatersCompleteDelegate(cm<%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, parent<%End If%>)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><NonSerialized()> _
<% If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i)%>Private executeUpdateDelegate As predicateEx(Of <%If objInfo.UseAdoTransaction Then%>SqlTransaction<%else%>SqlConnection<%End If%>, <%=objInfo.ParentType%>) = Nothing
<% Else %>
<%=Indent(i)%>Private executeUpdateDelegate As Predicate(Of <%If objInfo.UseAdoTransaction Then%>SqlTransaction<%else%>SqlConnection<%End If%>) = Nothing
<% End If %>
<%=Indent(i)%>Private Function OnExecuteUpdate(<%If objInfo.UseAdoTransaction Then%>ByVal tr As SqlTransaction<%else%>ByVal cn As SqlConnection<%End If%><%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, ByVal parent As <%=objInfo.ParentType%><%End If%>) As Boolean
<% i+=1 %>
<%=Indent(i)%>If Not executeUpdateDelegate Is Nothing Then
<%=Indent(i+1)%>Return executeUpdateDelegate(<%If objInfo.UseAdoTransaction Then%>tr<%else%>cn<%End If%><%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, parent<%End If%>)
<%=Indent(i)%>End If
<%=Indent(i)%>Return True
<% i-=1 %>
<%=Indent(i)%>End Function
<%=Indent(i, True)%><NonSerialized()> _
<% If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i)%>Private executeUpdateCompleteDelegate As actionEx(Of SqlCommand, <%=objInfo.ParentType%>) = Nothing
<% else %>
<%=Indent(i)%>Private executeUpdateCompleteDelegate As Action(Of SqlCommand) = Nothing
<% End If %>
<%=Indent(i)%>Private Sub OnExecuteUpdateComplete(ByVal cm As SqlCommand<%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, ByVal parent As <%=objInfo.ParentType%><%End If%>)
<% i+=1 %>
<%=Indent(i)%>If Not executeUpdateCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>executeUpdateCompleteDelegate(cm<%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, parent<%End If%>)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><NonSerialized()> _
<% If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then %>
<%=Indent(i)%>Private addUpdateParamatersCompleteDelegate As actionEx(Of SqlCommand, <%=objInfo.ParentType%>) = Nothing
<% Else %>
<%=Indent(i)%>Private addUpdateParamatersCompleteDelegate As Action(Of SqlCommand) = Nothing
<% End If %>
<%=Indent(i)%>Private Sub OnAddUpdateParametersComplete(ByVal cm As SqlCommand<%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, ByVal parent As <%=objInfo.ParentType%><%End If%>)
<% i+=1 %>
<%=Indent(i)%>If Not addUpdateParamatersCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>addUpdateParamatersCompleteDelegate(cm<%If objInfo.IsChild AndAlso objInfo.ParentType.Length > 0 Then%>, parent<%End If%>)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private updateChildrenCompleteDelegate As Action(Of <%If objInfo.UseAdoTransaction Then%>SqlTransaction<%else%>SqlConnection<%End If%>) = Nothing
<%=Indent(i)%>Private Sub OnUpdateChildrenComplete(<%If objInfo.UseAdoTransaction Then%>ByVal tr As SqlTransaction<%else%>ByVal cn As SqlConnection<%End If%>)
<% i+=1 %>
<%=Indent(i)%>If Not updateChildrenCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>updateChildrenCompleteDelegate(<%If objInfo.UseAdoTransaction Then%>tr<%else%>cn<%End If%>)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<%If Not objInfo.IsChild Then %>
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private executeDeleteDelegate As predicateEx(Of <%If objInfo.UseAdoTransaction Then%>SqlTransaction<%else%>SqlConnection<%End If%>, Criteria) = Nothing
<%=Indent(i)%>Private Function OnExecuteDelete(<%If objInfo.UseAdoTransaction Then%>ByVal tr As SqlTransaction<%else%>ByVal cn As SqlConnection<%End If%>, ByVal criteria As Criteria) As Boolean
<% i+=1 %>
<%=Indent(i)%>If Not executeDeleteDelegate Is Nothing Then
<%=Indent(i+1)%>Return executeDeleteDelegate(<%If objInfo.UseAdoTransaction Then%>tr<%else%>cn<%End If%>, criteria)
<%=Indent(i)%>End If
<%=Indent(i)%>Return True
<% i-=1 %>
<%=Indent(i)%>End Function
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private executeDeleteCompleteDelegate As actionEx(Of SqlCommand, Criteria) = Nothing
<%=Indent(i)%>Private Sub OnExecuteDeleteComplete(ByVal cm As SqlCommand, ByVal criteria As Criteria)
<% i+=1 %>
<%=Indent(i)%>If Not executeDeleteCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>executeDeleteCompleteDelegate(cm, criteria)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<% else %>
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private executeDeleteDelegate As Predicate(Of <%If objInfo.UseAdoTransaction Then%>SqlTransaction<%else%>SqlConnection<%End If%>) = Nothing
<%=Indent(i)%>Private Function OnExecuteDelete(<%If objInfo.UseAdoTransaction Then%>ByVal tr As SqlTransaction<%else%>ByVal cn As SqlConnection<%End If%>) As Boolean
<% i+=1 %>
<%=Indent(i)%>If Not executeDeleteDelegate Is Nothing Then
<%=Indent(i+1)%>Return executeDeleteDelegate(<%If objInfo.UseAdoTransaction Then%>tr<%else%>cn<%End If%>)
<%=Indent(i)%>End If
<%=Indent(i)%>Return True
<% i-=1 %>
<%=Indent(i)%>End Function
<%=Indent(i, True)%><NonSerialized()> _
<%=Indent(i)%>Private executeDeleteCompleteDelegate As Action(Of SqlCommand) = Nothing
<%=Indent(i)%>Private Sub OnExecuteDeleteComplete(ByVal cm As SqlCommand)
<% i+=1 %>
<%=Indent(i)%>If Not executeDeleteCompleteDelegate Is Nothing Then
<%=Indent(i+1)%>executeDeleteCompleteDelegate(cm)
<%=Indent(i)%>End If
<% i-=1 %>
<%=Indent(i)%>End Sub
<%End If%>
<%=Indent(0, True)%>#End Region ' Delegation Methods<%End If%>