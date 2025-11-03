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
    public class OtherRepository
    {
        public static DataTable _dt;
        public static DataSet _ds;
        public OtherRepository() : base()
        {
            _dt = new DataTable();
            _ds = new DataSet();
        }

        public List<DelaysBLL> GetAllDelays()
        {
            try
            {
                var lst = new List<DelaysBLL>();

                SqlParameter[] p = new SqlParameter[0];

                _dt = (new DBHelper().GetTableFromSP)("sp_GetAllDelays_PP", p);
                if (_dt != null)
                {
                    if (_dt.Rows.Count > 0)
                    {
                        lst = JArray.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_dt)).ToObject<List<DelaysBLL>>();
                    }
                }

                return lst;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int Insert(UserBLL data)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[7];
                p[0] = new SqlParameter("@RoleID", data.RoleID);
                p[1] = new SqlParameter("@Name", data.Name);
                p[2] = new SqlParameter("@Email", data.Email);
                p[3] = new SqlParameter("@Password", data.Password);
                p[4] = new SqlParameter("@StatusID", data.StatusID);
                p[5] = new SqlParameter("@CreatedDate", data.CreatedDate);
                p[6] = new SqlParameter("@CreatedBy", "Administrator");
                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_InsertUser_PP", p);
                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public int Delete(int id)
        {
            try
            {
                int rtn = 0;
                SqlParameter[] p = new SqlParameter[4];

                p[0] = new SqlParameter("@StatusID", 3);
                p[1] = new SqlParameter("@ID", id);
                p[2] = new SqlParameter("@LastUpdateDate", DateTime.Now);
                p[3] = new SqlParameter("@LastUpdateBy", "Administrator");

                rtn = (new DBHelper().ExecuteNonQueryReturn)("sp_DeleteUserRole_PP", p);
                return rtn;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
