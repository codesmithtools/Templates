<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TaskViewData>" %>
<%@ Import Namespace="PLINQO.Mvc.UI.Models"%>
<%@ Import Namespace="PLINQO.Mvc.UI.Controllers"%>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="AssignedId">Assigned To:</label>
                <%= Html.DropDownList("AssignedId", Model.AssignedUsers, "Select Assigned To")%>
                <%= Html.ValidationMessage("AssignedId", "*") %>
            </p>
            <p>
                <label for="PriorityId">Priority:</label>
                <%= Html.DropDownList("Priority", Model.Priorities, "Select Priority")%>
                <%= Html.ValidationMessage("PriorityId", "*")%>
            </p>
            <p>
                <label for="StatusId">Status:</label>
                 <%= Html.DropDownList("StatusId", Model.Statuses, "Select Status")%>
                <%= Html.ValidationMessage("StatusId", "*") %>
            </p>
            <p>
                <label for="Summary">Summary:</label>
                <%= Html.TextBox("Summary") %>
                <%= Html.ValidationMessage("Summary", "*") %>
            </p>
            <p>
                <label for="Details">Details:</label>
                <%= Html.TextBox("Details") %>
                <%= Html.ValidationMessage("Details", "*") %>
            </p>
            <p>
                <label for="StartDate">Start Date:</label>
                <%= Html.TextBox("StartDate") %>
                <%= Html.ValidationMessage("StartDate", "*") %>
            </p>
            <p>
                <label for="DueDate">Due Date:</label>
                <%= Html.TextBox("DueDate") %>
                <%= Html.ValidationMessage("DueDate", "*") %>
            </p>
            <p>
                <label for="CompleteDate">Complete Date:</label>
                <%= Html.TextBox("CompleteDate") %>
                <%= Html.ValidationMessage("CompleteDate", "*") %>
            </p>
            <p>
                <input type="submit" value="Create" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

