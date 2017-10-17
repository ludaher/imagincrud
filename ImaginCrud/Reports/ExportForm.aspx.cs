using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using static ImaginCrud.Reports.ProcessingTime;

namespace ImaginCrud.Reports
{
    public partial class ExportForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(User.Identity.IsAuthenticated == false) {
                Response.Redirect("~/Home/Index",true);
                return;
            }
            if (IsPostBack)
                return;
            reportes.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
            reportes.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["URLReportes"]);
            reportes.ServerReport.ReportPath = string.Format("{0}Form_3_All", ConfigurationManager.AppSettings["PathReportes"]);
            IReportServerCredentials objCredencial = new CustomReportCredentials(ConfigurationManager.AppSettings["UserReportes"], ConfigurationManager.AppSettings["PwdReportes"], "");
            reportes.ServerReport.ReportServerCredentials = objCredencial;
            reportes.ShowParameterPrompts = false;
            //Si se necesitan parámetros se utiliza el siguiente código
            ReportParameter[] parameters = new ReportParameter[2];
            parameters[0] = new ReportParameter("Orip");
            parameters[0].Values.Add("368");
            parameters[1] = new ReportParameter("Caja");
            parameters[1].Values.Add("60");
            this.reportes.ServerReport.SetParameters(parameters);
            this.reportes.Visible = true;
            this.reportes.ServerReport.Refresh();
        }

    }
}