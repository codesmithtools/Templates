<%=Indent(i, true)%>#region Data Access - Insert
<% if(objInfo.UseTransactionalAttribute) {%>
<%=Indent(i)%>[Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)] 
<%}%>
<%=Indent(i)%>protected override void DataPortal_Insert()
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>SqlTransaction tr;

<%}%>
<%=Indent(i)%>using (SqlConnection cn = new SqlConnection(<%=objInfo.DbConnection%>))
<%=Indent(i++)%>{
<%=Indent(i)%>cn.Open();

<% if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>tr = cn.BeginTransaction();
<%=Indent(i)%>try
<%=Indent(i++)%>{
<%}%>
<% if(objInfo.CslaObjectType == ObjectType.EditableSwitchable && objInfo.ParentType.Length>0) {%>
<%=Indent(i)%>ExecuteInsert(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, null);
<%} else {%>
<%=Indent(i)%>ExecuteInsert(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<%}%>

<%=Indent(i)%>//update child object(s)
<%=Indent(i)%>UpdateChildren(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<% if(objInfo.UseAdoTransaction) {%>

<%=Indent(i)%>tr.Commit();
<%=Indent(--i)%>}
<%=Indent(i)%>catch
<%=Indent(i++)%>{
<%=Indent(i)%>tr.Rollback();
<%=Indent(i)%>throw;
<%=Indent(--i)%>}
<%}%>
<%=Indent(--i)%>}//using

<%=Indent(--i)%>}

<% if(objInfo.CslaObjectType != ObjectType.EditableSwitchable) {%>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteInsert(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>if (!onExecuteInsert(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>)) return;
<%}%>
<%=Indent(i)%>using (SqlCommand cm = <%if(objInfo.UseAdoTransaction){%>tr.Connection<%}else{%>cn<%}%>.CreateCommand())
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>cm.Transaction = tr;
<%}%>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure;
<%=Indent(i)%>cm.CommandText = "<%=objInfo.InsertCommandText%>";

<%=Indent(i)%>AddInsertParameters(cm);

<%=Indent(i)%>cm.ExecuteNonQuery();
<%=GetInsertReturnParameterStatements(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onExecuteInsertComplete(cm);
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void AddInsertParameters(SqlCommand cm)
<%=Indent(i++)%>{
<%=GetInsertParameters(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onAddInsertParametersComplete(cm);
<%}%>
<%=Indent(--i)%>}
<%} //!Switchable%>
<%=Indent(i)%>#endregion //Data Access - Insert

<%=Indent(i)%>#region Data Access - Update
<% if(objInfo.UseTransactionalAttribute) {%>
<%=Indent(i)%>[Transactional(TransactionalTypes.<%=objInfo.TransactionType.ToString()%>)] 
<%}%>
<%=Indent(i)%>protected override void DataPortal_Update()
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>SqlTransaction tr;

<%}%>
<%=Indent(i)%>using (SqlConnection cn = new SqlConnection(<%=objInfo.DbConnection%>))
<%=Indent(i++)%>{
<%=Indent(i)%>cn.Open();

<% if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>tr = cn.BeginTransaction();
<%=Indent(i)%>try
<%=Indent(i++)%>{
<%}%>
<%=Indent(i)%>if (base.IsDirty)
<%=Indent(i++)%>{
<% if(objInfo.CslaObjectType == ObjectType.EditableSwitchable && objInfo.ParentType.Length>0) {%>
<%=Indent(i)%>ExecuteUpdate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, null);
<%} else {%>
<%=Indent(i)%>ExecuteUpdate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<%}%>
<%=Indent(--i)%>}

<%=Indent(i)%>//update child object(s)
<%=Indent(i)%>UpdateChildren(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<% if(objInfo.UseAdoTransaction) {%>

<%=Indent(i)%>tr.Commit();
<%=Indent(--i)%>}
<%=Indent(i)%>catch
<%=Indent(i++)%>{
<%=Indent(i)%>tr.Rollback();
<%=Indent(i)%>throw;
<%=Indent(--i)%>}
<%}%>
<%=Indent(--i)%>}//using

<%=Indent(--i)%>}

<% if(objInfo.CslaObjectType != ObjectType.EditableSwitchable) {%>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void ExecuteUpdate(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>if (!onExecuteUpdate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>)) return;
<%}%>
<%=Indent(i)%>using (SqlCommand cm = <%if(objInfo.UseAdoTransaction){%>tr.Connection<%}else{%>cn<%}%>.CreateCommand())
<%=Indent(i++)%>{
<%if(objInfo.UseAdoTransaction) {%>
<%=Indent(i)%>cm.Transaction = tr;
<%}%>
<%=Indent(i)%>cm.CommandType = CommandType.StoredProcedure;
<%=Indent(i)%>cm.CommandText = "<%=objInfo.UpdateCommandText%>";

<%=Indent(i)%>AddUpdateParameters(cm);

<%=Indent(i)%>cm.ExecuteNonQuery();
<%=GetUpdateReturnParameterStatements(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onExecuteUpdateComplete(cm);
<%}%>
<%=Indent(--i)%>}//using
<%=Indent(--i)%>}

<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void AddUpdateParameters(SqlCommand cm)
<%=Indent(i++)%>{
<%=GetUpdateParameters(objInfo, i)%>
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>onAddUpdateParametersComplete(cm);
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
<%}%>
<%=Indent(i)%>#endregion //Data Access - Update
