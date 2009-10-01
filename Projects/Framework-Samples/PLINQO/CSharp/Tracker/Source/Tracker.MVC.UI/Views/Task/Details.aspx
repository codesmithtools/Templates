<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Tracker.Core.Data.Task>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Details
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Details</h2>
    
    <div>
        <%=Html.ActionLink("Bask To List", "Index") %>
        <%=Html.ActionLink("Create Task", "Create") %>
        <%=Html.ActionLink("Copy Task", "CopyTask", new {id=Model.Id}) %>
    </div>

    <fieldset>
        <legend>Task Details</legend>
        <p>
            Id:
            <%=Html.Encode(Model.Id)%>
        </p>
        <p>
            Status:
            <%=Html.Encode(Model.Status.Name)%>
        </p>
        <p>
            Created By:
            <%=Html.Encode(Model.CreatedUser.FullName)%>
        </p>
        <p>
            Summary:
            <%=Html.Encode(Model.Summary)%>
        </p>
        <p>
            Details:
            <%=Html.Encode(Model.Details)%>
        </p>
        <p>
            Start Date:
            <%=Html.Encode(String.Format("{0:g}", Model.StartDate))%>
        </p>
        <p>
            Due Date:
            <%=Html.Encode(String.Format("{0:g}", Model.DueDate))%>
        </p>
        <p>
            Complete Date:
            <%=Html.Encode(String.Format("{0:g}", Model.CompleteDate))%>
        </p>
        <p>
            Assigned To:
            <%if (Model.AssignedUser != null){%>
            <%=Html.Encode(Model.AssignedUser.FullName)%>
            <%} %>
        </p>
        <p>
            CreatedDate:
            <%= Html.Encode(String.Format("{0:g}", Model.CreatedDate)) %>
        </p>
        <p>
            ModifiedDate:
            <%= Html.Encode(String.Format("{0:g}", Model.ModifiedDate)) %>
        </p>
        <p>
            Last Modified By:
            <%= Html.Encode(Model.LastModifiedBy) %>
        </p>
    </fieldset>
    <p>

        <%=Html.ActionLink("Edit", "Edit", new { id=Model.Id }) %> |
        <%=Html.ActionLink("Back to List", "Index") %>
    </p>

</asp:Content>

