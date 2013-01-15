<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="SearchEngineAssignment_NO_MVC._Default" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div id="main-content">
    
    </div>
    <div id="fb-root">

    </div>
    <script type="text/javascript">
        window.fbAsyncInit = function () {
            FB.init({
                appId: '238835239578256', // App ID
                status: true, // check login status
                cookie: true, // enable cookies to allow the server to access the session
                xfbml: true,  // parse XFBML
                oauth: true
            });
            FB.Canvas.setAutoGrow(false);

            FB.login(function (response) {
            }, { scope: 'read_stream' });

            FB.api('/me/permissions', function (response) {
                var perms = response.data[0];
                if (perms.read_stream) {

                    var uid = parentresponse.authResponse.userID;
                    var accessToken = parentresponse.authResponse.accessToken;
                    accessToken = httpGet("https://graph.facebook.com/oauth/access_token?client_id=238835239578256&client_secret=37c5746af5d122601a3a1b5c3eb63ba5&grant_type=fb_exchange_token&fb_exchange_token=" + accessToken);
                    var form = document.createElement("form");
                    form.setAttribute("method", 'post');
                    form.setAttribute("action", '/FB/LoginHandler.ashx');
                    var field = document.createElement("input");
                    field.setAttribute("type", "hidden");
                    field.setAttribute("name", 'accessToken');
                    field.setAttribute("value", accessToken);
                    form.appendChild(field);
                    var field2 = document.createElement("input");
                    field2.setAttribute("type", "hidden");
                    field2.setAttribute("name", 'uid');
                    field2.setAttribute("value", uid);
                    form.appendChild(field2);

                    document.body.appendChild(form);
                    form.submit();
                }
            });
            FB.Event.subscribe('auth.authResponseChange', function (parentresponse) {
                FB.api('/me/permissions', function (response) {
                    var perms = response.data[0];
                    if (perms.read_stream) {

                        var uid = parentresponse.authResponse.userID;
                        var accessToken = parentresponse.authResponse.accessToken;
                        accessToken = httpGet("https://graph.facebook.com/oauth/access_token?client_id=238835239578256&client_secret=37c5746af5d122601a3a1b5c3eb63ba5&grant_type=fb_exchange_token&fb_exchange_token=" + accessToken);
                        var form = document.createElement("form");
                        form.setAttribute("method", 'post');
                        form.setAttribute("action", '/FB/LoginHandler.ashx');
                        var field = document.createElement("input");
                        field.setAttribute("type", "hidden");
                        field.setAttribute("name", 'accessToken');
                        field.setAttribute("value", accessToken);
                        form.appendChild(field);
                        var field2 = document.createElement("input");
                        field2.setAttribute("type", "hidden");
                        field2.setAttribute("name", 'uid');
                        field2.setAttribute("value", uid);
                        form.appendChild(field2);

                        document.body.appendChild(form);
                        form.submit();
                    } else {
                        alert("This application needs those permissions to work");
                        location.reload();
                    }
                });
            });

            FB.Canvas.setAutoGrow();

        };

        function httpGet(theUrl) {
            var xmlHttp = null;

            xmlHttp = new XMLHttpRequest();
            xmlHttp.open("GET", theUrl, false);
            xmlHttp.send(null);
            return xmlHttp.responseText;
        }

        // Load the SDK Asynchronously
        (function (d) {
            var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement('script'); js.id = id; js.async = true;
            js.src = "//connect.facebook.net/en_US/all.js";
            ref.parentNode.insertBefore(js, ref);
        } (document));
    </script>

</asp:Content>
