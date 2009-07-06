<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TaskViewData>" %>
<%@ Import Namespace="PLINQO.Mvc.UI.Models"%>
<%@ Import Namespace="PLINQO.Mvc.UI.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Editing Task #<%=Model.Task.Id %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Editing Task #<%=Model.Task.Id %></h2>
    
    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
        <%=Html.ActionLink("Create New", "Create") %>
        <%=Html.ActionLink("Copy Task", "CopyTask", new {id=Model.Task.Id}) %>
    </div>
    
    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            
            <p>
                <input type="submit" value="Save" />
            </p>

            <p>
                <label for="AssignedId">Assigned To:</label>
                <%= Html.DropDownList("AssignedId", Model.AssignedUsers, "Select User")%>
                <%= Html.ValidationMessage("AssignedId", "*") %>
            </p>
            <p>
                <label for="PriorityId">Priority:</label>
                <%= Html.DropDownList("PriorityId", Model.Priorities)%>
                <%= Html.ValidationMessage("PriorityId", "*")%>
            </p>
            <p>
                <label for="StatusId">StatusId:</label>
                 <%= Html.DropDownList("StatusId", Model.Statuses)%>
                <%= Html.ValidationMessage("StatusId", "*") %>
            </p>
            <p>
                <label for="StartDate">Start Date:</label>
                <%= Html.TextBox("StartDate", String.Format("{0:g}", Model.Task.StartDate)) %>
                <%= Html.ValidationMessage("StartDate", "*") %>
            </p>
            <p>
                <label for="DueDate">Due Date:</label>
                <%= Html.TextBox("DueDate", String.Format("{0:g}", Model.Task.DueDate)) %>
                <%= Html.ValidationMessage("DueDate", "*") %>
            </p>
            <p>
                <label for="CompleteDate">Complete Date:</label>
                <%= Html.TextBox("CompleteDate", String.Format("{0:g}", Model.Task.CompleteDate)) %>
                <%= Html.ValidationMessage("CompleteDate", "*") %>
            </p>
            <p>
                <label for="Summary">Summary:</label>
                <%= Html.TextBox("Summary", Model.Task.Summary) %>
                <%= Html.ValidationMessage("Summary", "*") %>
            </p>
            <p>
                <label for="Details">Details:</label>
                <%= Html.TextBox("Details", Model.Task.Details) %>
                <%= Html.ValidationMessage("Details", "*") %>
            </p>

            <p>
                Created By:
                <%=Html.Encode(Model.Task.CreatedUser.FullName)%>
            </p>
            <p>
                Last Modified By:
                <%= Html.Encode(Model.Task.LastModifiedBy) %>
            </p>

            <p>
                CreatedDate:
                <%= Html.Encode(String.Format("{0:g}", Model.Task.CreatedDate)) %>
            </p>
            <p>
                ModifiedDate:
                <%= Html.Encode(String.Format("{0:g}", Model.Task.ModifiedDate))%>
            </p>

        </fieldset>

    <% } %>

    <div>
        
    </div>

    <%if( Model.Audits.Count > 0) {%>
    <div class="audit-notes">
    <% foreach (Audit audit in Model.Audits) {%>
    <%if(!String.IsNullOrEmpty(audit.HtmlContent)) {%>
    <div class="audit-note">
    <%=audit.HtmlContent %>
    <% } %>
    <% } %>
    </div>
    <% } %>

</asp:Content>

