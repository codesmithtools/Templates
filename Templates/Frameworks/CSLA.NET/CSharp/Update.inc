<%=Indent(i, true)%>#region Data Access - Insert
<% if(objInfo.IsGeneratedBase && objInfo.ParentType.Length>0) { %>
<%=Indent(i)%>internal void Insert<<%=objInfo.ParentType%>>(<%if(objInfo.UseAdoTransaction) {%>SqlTransaction tr<%} else {%>SqlConnection cn<%}%>, <%=objInfo.ParentType%> parent) where <%=objInfo.ParentType%> : <%=objInfo.ParentNameAndSuffix%>
<% } else { %>
<%=Indent(i)%>internal void Insert(<%if(objInfo.UseAdoTransaction) {%>SqlTransaction tr<%} else {%>SqlConnection cn<%}%><%if(objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%}%>
<%=Indent(i++)%>{
<%=Indent(i)%>if (!IsDirty) return;

<%=Indent(i)%>ExecuteInsert(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%><%if(objInfo.ParentType.Length>0){%>, parent<%}%>);
<%=Indent(i)%>MarkOld();

<%=Indent(i)%>//update child object(s)
<%=Indent(i)%>UpdateChildren(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<%=Indent(--i)%>}

<% if(objInfo.IsGeneratedBase && objInfo.ParentType.Length>0) { %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteInsert<<%=objInfo.ParentType%>>(<%if(objInfo.UseAdoTransaction) {%>SqlTransaction tr<%} else {%>SqlConnection cn<%}%>, <%=objInfo.ParentType%> parent) where <%=objInfo.ParentType%> : <%=objInfo.ParentNameAndSuffix%>
<% } else { %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteInsert(<%if(objInfo.UseAdoTransaction) {%>SqlTransaction tr<%} else {%>SqlConnection cn<%}%><%if(objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%}%>
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>if (!onExecuteInsert(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%><%if(objInfo.ParentType.Length>0){%>, parent<%}%>)) return;
<%}%>
<%=Indent(i)%>using (SqlCommand cm = <%if(objInfo.UseAdoTransaction){%>tr.Connection<%}else{%>cn<%}%>.CreateCommand())
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>cm.Transaction = tr;
<%}%>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure;
<%=Indent(i)%>cm.CommandText = "<%=objInfo.InsertCommandText%>";

<%=Indent(i)%>AddInsertParameters(cm<%if(objInfo.ParentType.Length>0){%>, parent<%}%>);

<%=Indent(i)%>cm.ExecuteNonQuery();
<%=GetInsertReturnParameterStatements(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onExecuteInsertComplete(cm<%if(objInfo.ParentType.Length>0){%>, parent<%}%>);
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}

<% if(objInfo.IsGeneratedBase && objInfo.ParentType.Length>0) { %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void AddInsertParameters<<%=objInfo.ParentType%>>(SqlCommand cm, <%=objInfo.ParentType%> parent) where <%=objInfo.ParentType%> : <%=objInfo.ParentNameAndSuffix%>
<% } else { %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void AddInsertParameters(SqlCommand cm<%if(objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%}%>
<%=Indent(i++)%>{
<%if(objInfo.ParentType.Length>0 && objInfo.IsUserClass) {%>
<%=Indent(i)%>//TODO: if parent use identity key, fix fk member with value from parent
<%}%>
<%=GetInsertParameters(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onAddInsertParametersComplete(cm<%if(objInfo.ParentType.Length>0){%>, parent<%}%>);
<%}%>
<%=Indent(--i)%>}
<%=Indent(i)%>#endregion //Data Access - Insert

<%=Indent(i)%>#region Data Access - Update
<% if(objInfo.IsGeneratedBase && objInfo.ParentType.Length>0) { %>
<%=Indent(i)%>internal void Update<<%=objInfo.ParentType%>>(<%if(objInfo.UseAdoTransaction) {%>SqlTransaction tr<%} else {%>SqlConnection cn<%}%>, <%=objInfo.ParentType%> parent) where <%=objInfo.ParentType%> : <%=objInfo.ParentNameAndSuffix%>
<% } else { %>
<%=Indent(i)%>internal void Update(<%if(objInfo.UseAdoTransaction) {%>SqlTransaction tr<%} else {%>SqlConnection cn<%}%><%if(objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%}%>
<%=Indent(i++)%>{
<%=Indent(i)%>if (!IsDirty) return;

<%=Indent(i)%>if (base.IsDirty)
<%=Indent(i++)%>{
<%=Indent(i)%>ExecuteUpdate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%><%if(objInfo.ParentType.Length>0){%>, parent<%}%>);
<%=Indent(i)%>MarkOld();
<%=Indent(--i)%>}

<%=Indent(i)%>//update child object(s)
<%=Indent(i)%>UpdateChildren(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<%=Indent(--i)%>}

<% if(objInfo.IsGeneratedBase && objInfo.ParentType.Length>0) { %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteUpdate<<%=objInfo.ParentType%>>(<%if(objInfo.UseAdoTransaction) {%>SqlTransaction tr<%} else {%>SqlConnection cn<%}%>, <%=objInfo.ParentType%> parent) where <%=objInfo.ParentType%> : <%=objInfo.ParentNameAndSuffix%>
<% } else { %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteUpdate(<%if(objInfo.UseAdoTransaction) {%>SqlTransaction tr<%} else {%>SqlConnection cn<%}%><%if(objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%}%>
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>if (!onExecuteUpdate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%><%if(objInfo.ParentType.Length>0){%>, parent<%}%>)) return;
<%}%>
<%=Indent(i)%>using (SqlCommand cm = <%if(objInfo.UseAdoTransaction){%>tr.Connection<%}else{%>cn<%}%>.CreateCommand())
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>cm.Transaction = tr;
<%}%>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure;
<%=Indent(i)%>cm.CommandText = "<%=objInfo.UpdateCommandText%>";

<%=Indent(i)%>AddUpdateParameters(cm<%if(objInfo.ParentType.Length>0){%>, parent<%}%>);

<%=Indent(i)%>cm.ExecuteNonQuery();
<%=GetUpdateReturnParameterStatements(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onExecuteUpdateComplete(cm<%if(objInfo.ParentType.Length>0){%>, parent<%}%>);
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}

<% if(objInfo.IsGeneratedBase && objInfo.ParentType.Length>0) { %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void AddUpdateParameters<<%=objInfo.ParentType%>>(SqlCommand cm, <%=objInfo.ParentType%> parent) where <%=objInfo.ParentType%> : <%=objInfo.ParentNameAndSuffix%>
<% } else { %>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void AddUpdateParameters(SqlCommand cm<%if(objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%}%>
<%=Indent(i++)%>{
<%=GetUpdateParameters(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onAddUpdateParametersComplete(cm<%if(objInfo.ParentType.Length>0){%>, parent<%}%>);
<%}%>
<%=Indent(--i)%>}

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void UpdateChildren(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>)
<%=Indent(i++)%>{
<% if(objInfo.HasChild) { %>
<%	string cast=objInfo.IsGeneratedBase? "(T)": ""; 
	foreach(PropertyInfo prop in objInfo.ChildCollection) { 
	if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%><%=string.Format("{0}.Update(tr, {1}this);", prop.MemberName, cast)%>
<% } else { %>
<%=Indent(i)%><%=string.Format("{0}.Update(cn, {1}this);", prop.MemberName, cast)%>
<%}}%>
<% } //if(HasChild) %>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onUpdateChildrenComplete(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<%}%>
<%=Indent(--i)%>}
<%=Indent(i)%>#endregion //Data Access - Update
