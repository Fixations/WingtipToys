<%@ Page Title="Welcome to Wingtip Toys" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WingtipToys._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1><%: Title %></h1>
        <h2>We can help you find that perfect Gift.</h2>
        <p class="lead">Wingtip Toys is all about transportation toys. You can order any of our toys today and with in 3 busness days it will be on 
            your door step (USA only). Each toy listing has the detailed information about that product to help you choose the right toy.
        </p>
    </div>

    <div class="row">
        <div class="col-md-4">
            <h2>Sports Cars</h2>
            <p>Search through all of our specialty cars and find the perfect one for your "little racer". We even have one with a real gas engine. Many 
                to choose from and they range from mild to wild.
            </p>
            <p>
                <a class="btn btn-default" href="ProductList.aspx?id=1">See more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Boats</h2>
            <p>Sail boats with authentic cloth sails, detailing, and working rudders. Boats with artificial intelligence, allowing for hours of 
                remote control fun.
            </p>
            <p>
                <a class="btn btn-default" href="ProductList.aspx?id=4">See more &raquo;</a>
            </p>
        </div>
        <div class="col-md-4">
            <h2>Rockets</h2>
            <p>Why not find out how high you can go? Build one of our Rockets to reach 200 feet plus in to the sky. These model rocket kits 
                are fun to build and fabulous to fly.
            </p>
            <p>
                <a class="btn btn-default" href="ProductList.aaspx?id=5">See more &raquo;</a>
            </p>
        </div>
    </div>

</asp:Content>
