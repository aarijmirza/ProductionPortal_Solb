using DAL.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPICode.Helpers;

namespace BAL.Repositories
{
    public class SMPReportsRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public SMPReportsRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }
    }
}
