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
    public class LoginRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public LoginRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }
        public UserBLL GetAuthenticateUser(string username, string password)
        {
            var repo = new UserBLL();
            try
            {
                SqlParameter[] p = new SqlParameter[2];
                p[0] = new SqlParameter("@email", username);
                p[1] = new SqlParameter("@password", password);
                _ds = (new DBHelper().GetDatasetFromSP)("sp_authenticateUser_admin", p);
                if (_ds != null)
                {
                    repo = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_ds.Tables[0])).ToObject<List<UserBLL>>().FirstOrDefault();
                    repo.UserRole = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_ds.Tables[1])).ToObject<List<UserRoleBLL>>();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return repo;
        }
    }
}
