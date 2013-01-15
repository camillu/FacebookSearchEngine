<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="SearchEngineAssignment_NO_MVC.Search.WebForm1" %>

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
        FB.Canvas.setAutoGrow(false);
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
    <h2>Search</h2>
    <img class="displayed" src="../Content/logo.png" alt="Facebook Instant Search" runat="server"/>
    <div class="textbox">
        <asp:textbox Width="400px" ID="searchBox" runat="server" TextMode="SingleLine"/>
        <br />
        <asp:Button Text="Search" runat="server" OnClick="Redirect"/>
    </div>

</asp:Content>
