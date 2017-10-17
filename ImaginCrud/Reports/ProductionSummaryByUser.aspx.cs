using ImaginCrud.Logic;
using ImaginCrud.Reports.DataSets.InternalDataTableAdapters;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ImaginCrud.Reports.DataSets.InternalData;

namespace ImaginCrud.Reports
{
    public partial class ProductionSummaryByUser : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;
            if (Request.QueryString["FormId"] == null)
                return;
            var formId = Convert.ToInt32(Request.QueryString["FormId"]);
            DateTime form = DateTime.Now;
            if(DateTime.TryParseExact(Request.QueryString["From"], "yyyy-MM-dd",null,System.Globalization.DateTimeStyles.None,out form)==false)
                return;
            DateTime to;
            if(DateTime.TryParseExact(Request.QueryString["To"], "yyyy-MM-dd",null,System.Globalization.DateTimeStyles.None,out to) == false)
                return;
            FormReportViewer1.ProcessingMode = ProcessingMode.Local;
            ProductionSummaryByUser_ViewTableAdapter tableadapter = new ProductionSummaryByUser_ViewTableAdapter();
            var datatable = tableadapter.GetData(formId, form, to.AddDays(1));
            if (datatable.Any() == false)
            {
                FormReportViewer1.Visible = false;
                return;
            }
            ReportDataSource source = new ReportDataSource("ProductionSummaryByUser", datatable.CopyToDataTable());
            FormReportViewer1.LocalReport.ReportPath = "Reports/Rdlc/ProductionSummaryByUser.rdlc";
            FormReportViewer1.LocalReport.DataSources.Clear();
            FormReportViewer1.LocalReport.DataSources.Add(source);
            FormReportViewer1.Visible = true;
            FormReportViewer1.DataBind();
            FormReportViewer1.LocalReport.Refresh();
            //TxtFromDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            //TxtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            //var logic = new FormLogic();
            //var forms = logic.FindWithParameters(null, null);
            //DdlForm.DataSource = forms;
            //DdlForm.DataTextField = "Description";
            //DdlForm.DataValueField = "FormId";
            //DdlForm.DataBind();
            //DdlForm.Items.Insert(0, new ListItem("", "0"));
        }

        protected void BtnGenerar_Click(object sender, EventArgs e)
        {

        }
    }
}