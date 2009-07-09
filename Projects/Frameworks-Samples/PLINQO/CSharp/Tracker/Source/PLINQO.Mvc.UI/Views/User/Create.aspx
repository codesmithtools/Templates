<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PLINQO.Tracker.Data.User>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Create
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Create</h2>

    <%= Html.ValidationSummary("Create was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <div>
            <fieldset>
                <legend>Account Information</legend>
                <p>
                    <label for="EmailAddress">Email:</label>
                    <%= Html.TextBox("EmailAddress")%>
                    <%= Html.ValidationMessage("EmailAddress")%>
                </p>
                <p>
                    <label for="FirstName">First Name</label>
                    <%= Html.TextBox("FirstName") %>
                    <%= Html.ValidationMessage("FirstName", "*") %>
                </p>
                <p>
                    <label for="LastName">Last Name</label>
                    <%= Html.TextBox("LastName") %>
                    <%= Html.ValidationMessage("LastName", "*") %>
                </p>
                <p>
                    <label for="password">Password:</label>
                    <%= Html.Password("password") %>
                    <%= Html.ValidationMessage("password") %>
                </p>
                <p>
                    <label for="confirmPassword">Confirm password:</label>
                    <%= Html.Password("confirmPassword") %>
                    <%= Html.ValidationMessage("confirmPassword") %>
                </p>
                <p>
                    <input type="submit" value="Register" />
                </p>
            </fieldset>
        </div>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

