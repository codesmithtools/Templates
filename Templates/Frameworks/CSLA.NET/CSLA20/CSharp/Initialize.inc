<% if(objInfo.IsUserPartial) {%>
<%=Indent(i)%>#region Initialize
<%=Indent(i)%>protected override void Initialize()
<%=Indent(i++)%>{
<%=Indent(i)%>//addCustomRulesDelegate = addCustomRules;
<%if(!objInfo.IsChild) {%>
<%=Indent(i)%>//executeFetchDelegate = executeFetch;
<%=Indent(i)%>//executeFetchCompleteDelegate = executeFetchComplete;
<%}%>
<%=Indent(i)%>//fetchObjectCompleteDelegate = fetchObjectComplete;
<%=Indent(i)%>//fetchChildrenCompleteDelegate = fetchChildrenComplete;
<%=Indent(i)%>//executeInsertDelegate = executeInsert;
<%=Indent(i)%>//executeInsertCompleteDelegate = executeInsertComplete;
<%=Indent(i)%>//addInsertParamatersCompleteDelegate = addInsertParamatersComplete;
<%=Indent(i)%>//executeUpdateDelegate = executeUpdate;
<%=Indent(i)%>//executeUpdateCompleteDelegate = executeUpdateComplete;
<%=Indent(i)%>//addUpdateParamatersCompleteDelegate = addUpdateParamatersComplete;
<%=Indent(i)%>//updateChildrenCompleteDelegate = updateChildrenComplete;
<%=Indent(i)%>//executeDeleteDelegate = executeDelete;
<%=Indent(i)%>//executeDeleteCompleteDelegate = executeDeleteComplete;
<%=Indent(--i)%>}

<%=Indent(i)%>protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
<%=Indent(i++)%>{
<%=Indent(i)%>Initialize();
<%=Indent(--i)%>}
<%=Indent(i)%>#endregion //Initialize
<%}%>
