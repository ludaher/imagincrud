using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;

namespace ImaginCrud.Reports
{
    public partial class ProcessingTime : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(User.Identity.IsAuthenticated == false) {
                Response.Redirect("~/Home/Index",true);
                return;
            }
            if (IsPostBack)
                return;
            string formId = Request.QueryString["form"];
            //reportes.ProcessingMode = Microsoft.Reporting.WebForms.ProcessingMode.Remote;
            //reportes.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["URLReportes"]);
            //reportes.ServerReport.ReportPath = string.Format("{0}Form_{1}", ConfigurationManager.AppSettings["PathReportes"], formId);
            //IReportServerCredentials objCredencial = new CustomReportCredentials(ConfigurationManager.AppSettings["UserReportes"], ConfigurationManager.AppSettings["PwdReportes"], "");
            //reportes.ServerReport.ReportServerCredentials = objCredencial;
            //reportes.ShowParameterPrompts = false;
            ////Si se necesitan parámetros se utiliza el siguiente código
            ////ReportParameter[] parameters = new ReportParameter[2];
            ////parameters[1].Values.Add(final);
            ////this.reportes.ServerReport.SetParameters(parameters);
            //this.reportes.Visible = true;
            //this.reportes.ServerReport.Refresh();
        }

        public class CustomReportCredentials : IReportServerCredentials
        {
            private string _UserName;
            private string _PassWord;
            private string _DomainName;

            public CustomReportCredentials(string UserName, string PassWord, string DomainName)
            {
                _UserName = UserName;
                _PassWord = PassWord;
                _DomainName = DomainName;
            }

            public System.Security.Principal.WindowsIdentity ImpersonationUser
            {
                get { return null; }
            }

            public ICredentials NetworkCredentials
            {
                get { return new NetworkCredential(_UserName, _PassWord, _DomainName); }
            }
            
            public bool GetFormsCredentials(out Cookie authCookie, out string user,
             out string password, out string authority)
            {
                authCookie = null;
                user = password = authority = null;
                return false;
            }
            
        }
    }
}