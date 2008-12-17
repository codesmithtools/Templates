<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i, true)%>#region Delegation Methods
<% if(!(objInfo.IsChild && objInfo.ParentType.Length==0)) {%>
<%=Indent(i)%>private delegate bool predicateEx<U, V>(U obj1, V obj2);
<%=Indent(i)%>private delegate void actionEx<U, V>(U obj1, V obj2);
<%}%>
<%=Indent(i)%>private delegate void rulesAction();
<%=Indent(i)%>//
<%=Indent(i)%>// Business Rules
<%=Indent(i)%>//
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private rulesAction addCustomRulesDelegate = null;
<%=Indent(i)%>private void onAddCustomRules()
<%=Indent(i++)%>{
<%=Indent(i)%>if (addCustomRulesDelegate != null)
<%=Indent(i+1)%>addCustomRulesDelegate();
<%=Indent(--i)%>}
<%=Indent(i)%>//
<%=Indent(i)%>// Fetch
<%=Indent(i)%>//
<%if(!objInfo.IsChild) {%>
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private predicateEx<<%if(objInfo.UseAdoTransaction){%>SqlTransaction<%}else{%>SqlConnection<%}%>, Criteria> executeFetchDelegate = null;
<%=Indent(i)%>private bool onExecuteFetch(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>, Criteria criteria)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeFetchDelegate != null)
<%=Indent(i+1)%>return executeFetchDelegate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, criteria);
<%=Indent(i)%>return true;
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private actionEx<SqlCommand, Criteria> executeFetchCompleteDelegate = null;
<%=Indent(i)%>private void onExecuteFetchComplete(SqlCommand cm, Criteria criteria)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeFetchCompleteDelegate != null)
<%=Indent(i+1)%>executeFetchCompleteDelegate(cm, criteria);
<%=Indent(--i)%>}
<%} //!IsChild%>
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private Action<SafeDataReader> fetchObjectCompleteDelegate = null;
<%=Indent(i)%>private void onFetchObjectComplete(SafeDataReader dr)
<%=Indent(i++)%>{
<%=Indent(i)%>if (fetchObjectCompleteDelegate != null)
<%=Indent(i+1)%>fetchObjectCompleteDelegate(dr);
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private Action<SafeDataReader> fetchChildrenCompleteDelegate = null;
<%=Indent(i)%>private void onFetchChildrenComplete(SafeDataReader dr)
<%=Indent(i++)%>{
<%=Indent(i)%>if (fetchChildrenCompleteDelegate != null)
<%=Indent(i+1)%>fetchChildrenCompleteDelegate(dr);
<%=Indent(--i)%>}
<%=Indent(i)%>//
<%=Indent(i)%>// Insert
<%=Indent(i)%>//
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<% if(objInfo.IsChild && objInfo.ParentType.Length>0) {%>
<%=Indent(i)%>private predicateEx<<%if(objInfo.UseAdoTransaction){%>SqlTransaction<%}else{%>SqlConnection<%}%>, <%=objInfo.ParentType%>> executeInsertDelegate = null;
<%} else {%>
<%=Indent(i)%>private Predicate<<%if(objInfo.UseAdoTransaction){%>SqlTransaction<%}else{%>SqlConnection<%}%>> executeInsertDelegate = null;
<%}%>
<%=Indent(i)%>private bool onExecuteInsert(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%><%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeInsertDelegate != null)
<%=Indent(i+1)%>return executeInsertDelegate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%><%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, parent<%}%>);
<%=Indent(i)%>return true;
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<% if(objInfo.IsChild && objInfo.ParentType.Length>0) {%>
<%=Indent(i)%>private actionEx<SqlCommand, <%=objInfo.ParentType%>> executeInsertCompleteDelegate = null;
<%} else {%>
<%=Indent(i)%>private Action<SqlCommand> executeInsertCompleteDelegate = null;
<%}%>
<%=Indent(i)%>private void onExecuteInsertComplete(SqlCommand cm<%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeInsertCompleteDelegate != null)
<%=Indent(i+1)%>executeInsertCompleteDelegate(cm<%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, parent<%}%>);
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<% if(objInfo.IsChild && objInfo.ParentType.Length>0) {%>
<%=Indent(i)%>private actionEx<SqlCommand, <%=objInfo.ParentType%>> addInsertParamatersCompleteDelegate = null;
<%} else {%>
<%=Indent(i)%>private Action<SqlCommand> addInsertParamatersCompleteDelegate = null;
<%}%>
<%=Indent(i)%>private void onAddInsertParametersComplete(SqlCommand cm<%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (addInsertParamatersCompleteDelegate != null)
<%=Indent(i+1)%>addInsertParamatersCompleteDelegate(cm<%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, parent<%}%>);
<%=Indent(--i)%>}
<%=Indent(i)%>//
<%=Indent(i)%>// Update
<%=Indent(i)%>//
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<% if(objInfo.IsChild && objInfo.ParentType.Length>0) {%>
<%=Indent(i)%>private predicateEx<<%if(objInfo.UseAdoTransaction){%>SqlTransaction<%}else{%>SqlConnection<%}%>, <%=objInfo.ParentType%>> executeUpdateDelegate = null;
<%} else {%>
<%=Indent(i)%>private Predicate<<%if(objInfo.UseAdoTransaction){%>SqlTransaction<%}else{%>SqlConnection<%}%>> executeUpdateDelegate = null;
<%}%>
<%=Indent(i)%>private bool onExecuteUpdate(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%><%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeUpdateDelegate != null)
<%=Indent(i+1)%>return executeUpdateDelegate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%><%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, parent<%}%>);
<%=Indent(i)%>return true;
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<% if(objInfo.IsChild && objInfo.ParentType.Length>0) {%>
<%=Indent(i)%>private actionEx<SqlCommand, <%=objInfo.ParentType%>> executeUpdateCompleteDelegate = null;
<%} else {%>
<%=Indent(i)%>private Action<SqlCommand> executeUpdateCompleteDelegate = null;
<%}%>
<%=Indent(i)%>private void onExecuteUpdateComplete(SqlCommand cm<%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeUpdateCompleteDelegate != null)
<%=Indent(i+1)%>executeUpdateCompleteDelegate(cm<%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, parent<%}%>);
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<% if(objInfo.IsChild && objInfo.ParentType.Length>0) {%>
<%=Indent(i)%>private actionEx<SqlCommand, <%=objInfo.ParentType%>> addUpdateParamatersCompleteDelegate = null;
<%} else {%>
<%=Indent(i)%>private Action<SqlCommand> addUpdateParamatersCompleteDelegate = null;
<%}%>
<%=Indent(i)%>private void onAddUpdateParametersComplete(SqlCommand cm<%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, <%=objInfo.ParentType%> parent<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (addUpdateParamatersCompleteDelegate != null)
<%=Indent(i+1)%>addUpdateParamatersCompleteDelegate(cm<%if(objInfo.IsChild && objInfo.ParentType.Length>0){%>, parent<%}%>);
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private Action<<%if(objInfo.UseAdoTransaction){%>SqlTransaction<%}else{%>SqlConnection<%}%>> updateChildrenCompleteDelegate = null;
<%=Indent(i)%>private void onUpdateChildrenComplete(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (updateChildrenCompleteDelegate != null)
<%=Indent(i+1)%>updateChildrenCompleteDelegate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<%=Indent(--i)%>}
<%if(!objInfo.IsChild) {%>
<%=Indent(i)%>//
<%=Indent(i)%>// Delete
<%=Indent(i)%>//
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private predicateEx<<%if(objInfo.UseAdoTransaction){%>SqlTransaction<%}else{%>SqlConnection<%}%>, Criteria> executeDeleteDelegate = null;
<%=Indent(i)%>private bool onExecuteDelete(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>, Criteria criteria)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeDeleteDelegate != null)
<%=Indent(i+1)%>return executeDeleteDelegate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>, criteria);
<%=Indent(i)%>return true;
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private actionEx<SqlCommand, Criteria> executeDeleteCompleteDelegate = null;
<%=Indent(i)%>private void onExecuteDeleteComplete(SqlCommand cm, Criteria criteria)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeDeleteCompleteDelegate != null)
<%=Indent(i+1)%>executeDeleteCompleteDelegate(cm, criteria);
<%=Indent(--i)%>}
<%} else {%>
<%=Indent(i)%>//
<%=Indent(i)%>// Delete
<%=Indent(i)%>//
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private Predicate<<%if(objInfo.UseAdoTransaction){%>SqlTransaction<%}else{%>SqlConnection<%}%>> executeDeleteDelegate = null;
<%=Indent(i)%>private bool onExecuteDelete(<%if(objInfo.UseAdoTransaction){%>SqlTransaction tr<%}else{%>SqlConnection cn<%}%>)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeDeleteDelegate != null)
<%=Indent(i+1)%>return executeDeleteDelegate(<%if(objInfo.UseAdoTransaction){%>tr<%}else{%>cn<%}%>);
<%=Indent(i)%>return true;
<%=Indent(--i)%>}
<%=Indent(i)%>[NonSerialized()]
<%=Indent(i)%>[NotUndoable()]
<%=Indent(i)%>private Action<SqlCommand> executeDeleteCompleteDelegate = null;
<%=Indent(i)%>private void onExecuteDeleteComplete(SqlCommand cm)
<%=Indent(i++)%>{
<%=Indent(i)%>if (executeDeleteCompleteDelegate != null)
<%=Indent(i+1)%>executeDeleteCompleteDelegate(cm);
<%=Indent(--i)%>}
<%} //!IsChild%>
<%=Indent(i)%>#endregion //Delegation Methods
<%} //IsGeneratedPartial%>
