<%=Indent(i, true)%>#region Data Access - Delete
<%=Indent(i)%>internal void DeleteSelf(<% if(objInfo.UseAdoTransaction) { %>SqlTransaction tr<%} else {%>SqlConnection cn<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (!IsDirty) return;
<%=Indent(i)%>if (IsNew) return;

<% if(objInfo.CslaObjectType == ObjectType.EditableSwitchable) {%>
<%=Indent(i)%>ExecuteDelete(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, new Criteria(<%=GetDeleteSelfCriteriaCallArguments(objInfo)%>));
<%}else{%>
<%=Indent(i)%>ExecuteDelete(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<%}%>
<%=Indent(i)%>MarkNew();
<%=Indent(--i)%>}
<% if(objInfo.CslaObjectType != ObjectType.EditableSwitchable) {%>

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteDelete(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>if (!onExecuteDelete(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>)) return;
<%}%>
<%=Indent(i)%>using (SqlCommand cm = <%if(objInfo.UseAdoTransaction){%>tr.Connection<%}else{%>cn<%}%>.CreateCommand())
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>cm.Transaction = tr;
<%}%>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure;
<%=Indent(i)%>cm.CommandText = "<%=objInfo.DeleteCommandText%>";

<%=GetFetchParameters(objInfo, "", "this", i)%>

<%=Indent(i)%>cm.ExecuteNonQuery();
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onExecuteDeleteComplete(cm);
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}
<%}//!Switchable%>
<%=Indent(i)%>#endregion //Data Access - Delete
