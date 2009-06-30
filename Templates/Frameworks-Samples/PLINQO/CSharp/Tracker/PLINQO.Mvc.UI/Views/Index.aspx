<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PLINQO.Tracker.Data.User>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="Id">Id:</label>
                <%= Html.TextBox("Id") %>
                <%= Html.ValidationMessage("Id", "*") %>
            </p>
            <p>
                <label for="UserName">UserName:</label>
                <%= Html.TextBox("UserName") %>
                <%= Html.ValidationMessage("UserName", "*") %>
            </p>
            <p>
                <label for="Password">Password:</label>
                <%= Html.TextBox("Password") %>
                <%= Html.ValidationMessage("Password", "*") %>
            </p>
            <p>
                <label for="EmailAddress">EmailAddress:</label>
                <%= Html.TextBox("EmailAddress") %>
                <%= Html.ValidationMessage("EmailAddress", "*") %>
            </p>
            <p>
                <label for="FirstName">FirstName:</label>
                <%= Html.TextBox("FirstName") %>
                <%= Html.ValidationMessage("FirstName", "*") %>
            </p>
            <p>
                <label for="LastName">LastName:</label>
                <%= Html.TextBox("LastName") %>
                <%= Html.ValidationMessage("LastName", "*") %>
            </p>
            <p>
                <label for="CreatedDate">CreatedDate:</label>
                <%= Html.TextBox("CreatedDate") %>
                <%= Html.ValidationMessage("CreatedDate", "*") %>
            </p>
            <p>
                <label for="ModifiedDate">ModifiedDate:</label>
                <%= Html.TextBox("ModifiedDate") %>
                <%= Html.ValidationMessage("ModifiedDate", "*") %>
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

