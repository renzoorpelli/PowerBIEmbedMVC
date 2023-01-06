using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SQLReportService
{
    public partial class _Default : Page
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        
        protected void Button1_Click(object sender, EventArgs e)
        {
            this.CargarReportes();
        }

        private void CargarReportes()
        {
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlCommand consulta = new SqlCommand("SELECT * FROM usuarios", sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(consulta);
                DataTable data = new DataTable();
                sqlDataAdapter.Fill(data);
                ReporteGenerosItm.LocalReport.DataSources.Clear();
                ReportDataSource reportDataSource = new ReportDataSource("DataSet1", data);
                ReporteGenerosItm.LocalReport.ReportPath = Server.MapPath("./Reports/ReporteGeneros.rdl");
                ReporteGenerosItm.LocalReport.DataSources.Add(reportDataSource);
                ReporteGenerosItm.LocalReport.Refresh();
            }
        }
    }
}