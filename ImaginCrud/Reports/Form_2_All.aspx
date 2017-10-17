<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form_2_All.aspx.cs" Inherits="ImaginCrud.Reports.Form_2_All" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server">
        </asp:ScriptManager>
        <asp:Button ID="BtnGenerar" runat="server" Text="Button" OnClick="BtnGenerar_Click" Width="100%" />
        <rsweb:ReportViewer ID="FormReportViewer1" runat="server" Width="100%" >
            
        </rsweb:ReportViewer>
        <%--<rsweb:Report<rsweb:ReportViewer runat="server"></rsweb:ReportViewer>Viewer ID="FormReportViewer1" runat="server"></rsweb:ReportViewer>--%>
        <div class="clearfix"></div>
    </form>
</body>
</html>
