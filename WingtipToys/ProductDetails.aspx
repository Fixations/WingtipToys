<%@ Page Title="Product Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductDetails.aspx.cs" Inherits="WingtipToys.ProductDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:FormView ID="ProductDetail" runat="server" ItemType="WingtipToys.Models.Product" SelectMethod="GetProduct" RenderOuterTable="false">
        <ItemTemplate>
            <div>
                <h1><%#: Item.ProductName %></h1>
            </div>
            <br />
            <table>
                <tr>
                    <td>
                        <img src="/Catalog/Images/<%#: Item.ImagePath %>" style="border:solid; height:300px;" alt="<%#: Item.ProductName %>" />
                    </td>
                    <td>&nbsp;</td>
                    <td style="vertical-align:top; text-align:left;">
                        <div style="margin-left:10px;">
                            <b>Description: </b><br /><%#: Item.Description %>
                            <br />
                            <span><b>Price: </b>&nbsp;<%#: String.Format("{0:c}", Item.UnitPrice) %></span>
                            <br />
                            <span><b>Product Number: </b>&nbsp;<%#: Item.ProductID %></span>
                            <br />
                        </div>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:FormView>
</asp:Content>
