<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	PLINQO Ajax Sample
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>PLINQO Ajax Sample</h2>
    
    <%=Html.DropDownList("Users", Model as SelectList, "Select User") %>

    <div id="user-details" class="hide">
        <fieldset>
            <legend>Fields</legend>
            <p>
                Id:
                <span id="Id"></span>
            </p>
            <p>
                Email Address:
                <span id="EmailAddress"></span>
            </p>
            <p>
                First Name:
                <span id="FirstName"></span>
            </p>
            <p>
                Last Name:
                <span id="LastName"></span>
            </p>
            <p>
                Created Date:
                 <span id="CreatedDate"></span>
            </p>
            <p>
                Modified Date:
                 <span id="ModifiedDate"></span>
            </p>

        </fieldset>
        
        <table id="roles">
            <thead>
                <tr>
                    <th>Role</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        </table>
    
    </div>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">

<script language="javascript" type="text/javascript">


    $(document).ready(function() {

        $('#Users').change(function() {

            if (this.value == "") {
                $('#user-details').addClass('hide');
                $('#user-details').removeClass('show');
            }
            else {
                $.getJSON('/user/get/' + this.value, null, function(user) {
                    $('#Id').text(user.Id);
                    $('#EmailAddress').text(user.EmailAddress);
                    $('#FirstName').text(user.FirstName);
                    $('#LastName').text(user.LastName);
                    $('#CreatedDate').text(user.CreatedDate);
                    $('#ModifiedDate').text(user.ModifiedDate);
                    $('#user-details').removeClass('hide');
                    $('#user-details').addClass('show');

                    $('#roles tbody tr').remove();

                    for (var i = 0; i < user.RoleList.length; i++) {
                        $('#roles').children('tbody').append(
                        '<tr>' +
                            '<td>' + user.RoleList[i].Name + '</td>' +
                        '</tr>');
                    }

                });
            }
        });
    });
</script>
</asp:Content>
