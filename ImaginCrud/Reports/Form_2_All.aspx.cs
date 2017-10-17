using ImaginCrud.Reports.DataSets.ExportFormsTableAdapters;
using ImaginCrud.Util;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static ImaginCrud.Reports.DataSets.ExportForms;

namespace ImaginCrud.Reports
{
    public partial class Form_2_All : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void BtnGenerar_Click(object sender, EventArgs e)
        {
            FormReportViewer1.ProcessingMode = ProcessingMode.Local;
            Form_2_AllDataTable dataTable = new Form_2_AllDataTable();
            Form_2_AllTableAdapter tableadapter = new Form_2_AllTableAdapter();

            tableadapter.Fill(dataTable);
            if (dataTable.Any() == false)
            {
                FormReportViewer1.Visible = false;
                return;
            }
            ReportDataSource source = new ReportDataSource("Form_2_All_DS", dataTable.CopyToDataTable());
            FormReportViewer1.LocalReport.ReportPath = "Reports/Rdlc/Form_2_All.rdlc";
            FormReportViewer1.LocalReport.DataSources.Add(source);
            FormReportViewer1.Visible = true;
            FormReportViewer1.DataBind();
            FormReportViewer1.LocalReport.Refresh();
            //ExportDataToCsv.mCreateCSV(dataTable, Response.OutputStream, true);
        }

    }
}