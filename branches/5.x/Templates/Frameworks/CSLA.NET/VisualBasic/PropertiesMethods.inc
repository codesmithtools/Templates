<% If objInfo.IsGeneratedClass Then %>
<%=Indent(0, True)%>#Region " Business Properties and Methods "

<%=GetMemberDeclarations(objInfo, i)%>
<%=GetPropertyDeclarations(objInfo, i)%> 
<% If objInfo.HasChild AndAlso Not objInfo.IsReadOnly Then
	Dim ptrn As String = "Return MyBase.{0}"
	For Each prop As PropertyInfo In objInfo.ChildCollection
		ptrn += " {1} " & prop.MemberName & ".{0}"
	Next
%>
<%=Indent(i, True)%>''' <summary>
<%=Indent(i)%>''' Get the validity of this <%=objInfo.Name%><% If objInfo.HasChild Then %>, including child items<% End If %>.
<%=Indent(i)%>''' </summary>
<%=Indent(i)%>''' <value>A Boolean that represents the validity of the current object.</value>
<%=Indent(i)%>''' <returns>True if all business rules pass, otherwise False.</returns>
<%=Indent(i)%>''' <remarks></remarks>
<%=Indent(i)%>Public Overrides ReadOnly Property IsValid() As Boolean
<%=Indent(i+1)%>Get 
<%=Indent(i+2)%><%= String.Format(ptrn, "IsValid", "AndAlso")%>
<%=Indent(i+1)%>End Get
<%=Indent(i)%>End Property
<%=Indent(i, True)%>''' <summary>
<%=Indent(i)%>''' Get the change state of this <%=objInfo.Name%><% If objInfo.HasChild Then %>, including child items<% End If %>.
<%=Indent(i)%>''' </summary>
<%=Indent(i)%>''' <value>A Boolean that represents the change state of the current object.</value>
<%=Indent(i)%>''' <returns>True if any property has changed on this object<% If objInfo.HasChild Then %>, or any child item's property<% End If %>.</returns>
<%=Indent(i)%>''' <remarks></remarks>
<%=Indent(i)%>Public Overrides ReadOnly Property IsDirty() As Boolean
<%=Indent(i+1)%>Get 
<%=Indent(i+2)%><%= String.Format(ptrn, "IsDirty", "OrElse")%>
<%=Indent(i+1)%>End Get 
<%=Indent(i)%>End Property
<% End If %>
<%
	'build unique id String value
	Dim uniqueIdFormat As String = String.Empty
	Dim uniqueIdValues As String = String.Empty
	Dim uniqueId As String = String.Empty
	If objInfo.UniqueProperties.Count=1 Then
		uniqueId = CType(objInfo.UniqueProperties(0),PropertyInfo).MemberName 
	Else
		For uniqueIdIdx As Integer = 0 To objInfo.UniqueProperties.Count - 1
			uniqueIdFormat = uniqueIdFormat & ":{" & uniqueIdIdx.ToString & "}"
			uniqueIdValues = uniqueIdValues & ", " & objInfo.UniqueProperties(uniqueIdIdx).MemberName
		Next
		If uniqueIdFormat.Length > 0 Then
			uniqueId = "String.Format(""" & uniqueIdFormat.Substring(1) & """" & uniqueIdValues & ")"
		End If
	End If
%>
<%=Indent(i, True)%>''' <summary>
<%=Indent(i)%>''' Get the unique id for the current object.
<%=Indent(i)%>''' </summary>
<%=Indent(i)%>''' <returns>A value that is the unique id of the object.</returns>
<%=Indent(i)%>''' <remarks></remarks>
<%=Indent(i)%>Protected Overrides Function GetIdValue() As Object
<%=Indent(i+1)%>Return <%= uniqueId %>
<%=Indent(i)%>End Function
<%=Indent(0, True)%>#End Region ' Business Properties and Methods<% End If %>