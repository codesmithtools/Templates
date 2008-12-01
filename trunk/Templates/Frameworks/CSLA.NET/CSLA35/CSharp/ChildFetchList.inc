<% if(objInfo.IsGeneratedClass) {%>
<%=Indent(i)%><%=objInfo.LocalMethodModifiers%> void Fetch(IEnumerable<<%=objInfo.DalNamespace%>.<%=objInfo.EntityName%>> dataSet)
<%=Indent(i++)%>{
<%if(objInfo.IsGeneratedPartial) {%>
<%=Indent(i)%>bool cancel = false;
<%=Indent(i)%>OnFetching(dataSet, ref cancel);
<%=Indent(i)%>if(cancel) return;

<%}%>
<%=Indent(i)%>RaiseListChangedEvents = false;
<% if(objInfo.IsReadOnly) {%>
<%=Indent(i)%>IsReadOnly = false;
<%}%>

<%=Indent(i)%>foreach (var data in dataSet)
<%=Indent(i+1)%>this.Add(<%=objInfo.Child%>.<%=objInfo.GetChildMethodName%>(data));

<% if(objInfo.IsReadOnly) {%>
<%=Indent(i)%>IsReadOnly = true;
<%}%>
<%=Indent(i)%>RaiseListChangedEvents = true;
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>OnFetched();
<%}%>
<%=Indent(--i)%>}
<%if(objInfo.IsGeneratedPartial) {%>

<%=Indent(i)%>partial void OnFetching(IEnumerable<<%=objInfo.DalNamespace%>.<%=objInfo.EntityName%>> dataSet, ref bool cancel);
<%=Indent(i)%>partial void OnFetched();
<%}%>
<%}%>