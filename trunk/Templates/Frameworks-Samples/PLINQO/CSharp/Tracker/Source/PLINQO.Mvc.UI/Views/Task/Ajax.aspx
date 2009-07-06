<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Ajax
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Ajax</h2>
    
    Enter Task Id:
    <input type="text" value="" id="task-id" /> 
    <input type="button" value="Get Task" id="get-task-button"/>

    <div id="error-box">
    </div>

    <fieldset id="task-details" class="hide">
        <legend>Task Details</legend>
        <p>
            Id:
            <span id="Id"></span>
        </p>
        <p>
            Status:
            <span id="Status"></span>
        </p>
        <p>
            Created By:
            <span id="CreatedUser"></span>
        </p>
        <p>
            Summary:
            <span id="Summary"></span>
        </p>
        <p>
            Details:
            <span id="Details"></span>
        </p>
        <p>
            Start Date:
            <span id="StartDate"></span>
        </p>
        <p>
            Due Date:
            <span id="DueDate"></span>
        </p>
        <p>
            Complete Date:
            <span id="CompleteDate"></span>
        </p>
        <p>
            Assigned To:
            <span id="AssignedUser"></span>
        </p>
        <p>
            Last Modified By:
            <span id="LastModifiedBy"></span>
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

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadContent" runat="server">
<script language="javascript" type="text/javascript" src="/lib/scripts/ext-core.js"></script>
<script language="javascript" type="text/javascript" src="/lib/scripts/JSON.js"></script>

<script type="text/javascript" >



    Ext.onReady(function() {

        getTaskDetails();

        Ext.fly('get-task-button').on('click', function(e, t) {

            getTaskDetails();

        });


    });

    function getTaskDetails() {

        Ext.fly('error-box').update('');
        
        if (Ext.fly('task-id').getValue() != "") {
            Ext.Ajax.request({
                url: '/task/get',
                method: 'post',
                params: { id: Ext.fly('task-id').getValue() },
                success: function(response, opts) {
                    var task = Ext.decode(response.responseText);
                    Ext.fly('Id').update(task.Id);
                    Ext.fly('Status').update(task.Status.Name);
                    Ext.fly('CreatedUser').update(task.CreatedUser != null ? task.CreatedUser.FullName : "");
                    Ext.fly('Summary').update(task.Summary);
                    Ext.fly('Details').update(task.Details);
                    Ext.fly('StartDate').update(task.StartDate);
                    Ext.fly('DueDate').update(task.DueDate);
                    Ext.fly('CompleteDate').update(task.CompleteDate);
                    Ext.fly('AssignedUser').update(task.AssignedUser.FullName);
                    Ext.fly('LastModifiedBy').update(task.LastModifiedBy);
                    Ext.fly('CreatedDate').update(task.CreatedDate);
                    Ext.fly('ModifiedDate').update(task.ModifiedDate);
                    Ext.fly('task-details').removeClass('hide');
                    Ext.fly('task-details').addClass('show');
                },
                failure: function(response, opts) {
                    Ext.fly('task-details').removeClass('show');
                    Ext.fly('task-details').addClass('hide');
                    Ext.fly('error-box').update('server-side failure with status code ' + response.status);
                }
            });
        }
        else {
            Ext.fly('task-details').removeClass('show');
            Ext.fly('task-details').addClass('hide');
        }
    }
</script>
</asp:Content>
