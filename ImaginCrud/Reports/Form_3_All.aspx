<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Form_3_All.aspx.cs" Inherits="ImaginCrud.Reports.Form_3_All" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../Content/bootstrap.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div class="container body-content">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-tasks"></i>Producción por usuario</h3>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label ID="OripLabel" runat="server" Text="ORIP" AssociatedControlID="OripTextBox" CssClass="control-label"></asp:Label>
                                <asp:TextBox ID="OripTextBox" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <asp:Label ID="CajaLabel" runat="server" Text="CAJA" AssociatedControlID="CajaTextBox" CssClass="control-label"></asp:Label>
                                <asp:TextBox ID="CajaTextBox" runat="server" TextMode="Number" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-4">
                            
                        </div>
                    </div>
                    <div class="navbar  col-lg-12 ">
                        <div>
                            <asp:Button ID="BtnGenerar" runat="server" Text="Generar" OnClick="BtnGenerar_Click" CssClass="btn btn-success" />
                            <asp:Button ID="BtnCorte" runat="server" Text="Corte" OnClick="BtnGenerar_Click" CssClass="btn btn-danger" />
                        </div>
                    </div>
                </div>
            </div>
            <rsweb:ReportViewer ID="reportes" runat="server" Width="100%" Height="400px"></rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
