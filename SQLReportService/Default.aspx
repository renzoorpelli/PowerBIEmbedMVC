<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SQLReportService._Default" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<style>
    html, body, #divReport {
        height: 100%;
    }
</style>
    <div>
        <div class="container">
            <h1>Reporte generos</h1>
            <hr />
            <asp:Button ID="Button1" runat="server" Text="Cargar Reportes" OnClick="Button1_Click" />
        </div>
    </div>
        <div align="center" id="divReport">
            <rsweb:ReportViewer ID="ReporteGenerosItm" runat="server" Height="1200px" Width="1200px"></rsweb:ReportViewer>
        </div>
</asp:Content>
