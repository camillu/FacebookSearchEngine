<%@ Page Language="C#" Title="Search Results" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Results.aspx.cs" Inherits="SearchEngineAssignment_NO_MVC.Search.Results" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        window.fbAsyncInit = function () {
            FB.init({
                appId: '238835239578256', // App ID
                status: true, // check login status
                cookie: true, // enable cookies to allow the server to access the session
                xfbml: true,  // parse XFBML
                oauth: true
            });
            FB.Canvas.setAutoGrow(true);
        };

        (function (d) {
            var js, id = 'facebook-jssdk', ref = d.getElementsByTagName('script')[0];
            if (d.getElementById(id)) { return; }
            js = d.createElement('script'); js.id = id; js.async = true;
            js.src = "//connect.facebook.net/en_US/all.js";
            ref.parentNode.insertBefore(js, ref);
        } (document));

    </script>
    <div id="fb-root">
    
    </div>
    <link rel="Stylesheet" href="../Styles/Results.css" type="text/css" />
    <h2>Search Results</h2>
    <div>
        <asp:PlaceHolder id="SearchResultsPlaceHolder" runat="server"></asp:PlaceHolder>
    </div>
    <div>
        <asp:PlaceHolder id="likenames" runat="server"></asp:PlaceHolder>
    </div>
</asp:Content>