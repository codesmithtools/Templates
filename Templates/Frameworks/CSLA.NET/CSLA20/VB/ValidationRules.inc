<%=Indent(0, True)%>#Region " Validation Rules "
<% If objInfo.IsUserBase OrElse objInfo.IsUserPartial OrElse objInfo.IsSingle OrElse objInfo.IsGeneratedBase OrElse objInfo.IsGeneratedClass Then %>
<% If objInfo.IsSingle OrElse objInfo.IsUserPartial OrElse objInfo.IsGeneratedBase Then %>
<%=Indent(i, True)%>''' <summary>
<%=Indent(i)%>''' Method that allows the addition of custom business rules.
<%=Indent(i)%>''' </summary>
<% If objInfo.IsGeneratedBase OrElse objInfo.IsUserPartial Then %>
<%=Indent(i)%>''' <remarks>This is purely a hook, all additions should be done in the User version of this class.</remarks>
<% Else %>
<%=Indent(i)%>''' <remarks></remarks>
<% End If %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> Sub AddCustomRules()
<%=Indent(i+1)%>'todo: add custom/non-generated rules here...
<%=Indent(i)%>End Sub
<% If objInfo.IsSingle OrElse objInfo.IsUserPartial OrElse objInfo.IsGeneratedBase Then %>
<%=Indent(i, True)%>''' <summary>
<%=Indent(i)%>''' Method that allows the addition of shared custom business rules.
<%=Indent(i)%>''' </summary>
<% If objInfo.IsGeneratedBase OrElse objInfo.IsUserPartial Then %>
<%=Indent(i)%>''' <remarks>This is purely a hook, all additions should be done in the User version of this class.</remarks>
<% Else %>
<%=Indent(i)%>''' <remarks></remarks>
<% End If %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> Sub AddSharedCustomRules()
<%=Indent(i+1)%>'todo: add custom/non-generated shared rules here...
<%=Indent(i)%>End Sub
<% End If %>
<% End If %>
<% If objInfo.IsGeneratedClass Then %>
<%=Indent(i, True)%>''' <summary>
<%=Indent(i)%>''' Method used to instigate the addition of all Business rules both Custom and Common.
<%=Indent(i)%>''' </summary>
<%=Indent(i)%>''' <remarks></remarks>
<%=Indent(i)%>Protected Overrides Sub AddInstanceBusinessRules()
<% i+=1 %>
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnAddCustomRules()
<% Else %>
<%=Indent(i)%>AddCustomRules()
<% End If %>
<%=Indent(i)%>MyBase.AddBusinessRules()
<% i-=1 %>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%>''' <summary>
<%=Indent(i)%>''' Method that allows the addition of shared common business rules.
<%=Indent(i)%>''' </summary>
<%=Indent(i)%>''' <remarks>These rules represent the database defaults that can be generated.</remarks>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> Sub AddSharedCommonRules()
<%=GetCommonValidationRules(objInfo, i+1, True)%>
<%=Indent(i)%>End Sub
<%=Indent(i, True)%>''' <summary>
<%=Indent(i)%>''' Method used to instigate the addition of all Business rules both Custom and Common.
<%=Indent(i)%>''' </summary>
<%=Indent(i)%>''' <remarks></remarks>
<%=Indent(i)%>Protected Overrides Sub AddBusinessRules()
<% i+=1 %>
<%=Indent(i)%>AddSharedCommonRules()
<% If objInfo.IsGeneratedPartial Then %>
<%=Indent(i)%>OnAddCustomRules()
<% Else %>
<%=Indent(i)%>AddSharedCustomRules()
<%=Indent(i)%>MyBase.AddBusinessRules()
<% End If %>
<% i-=1 %>
<%=Indent(i)%>End Sub
<% End If %>
<% End If %>
<%=Indent(0, True)%>#End Region ' Validation Rules
