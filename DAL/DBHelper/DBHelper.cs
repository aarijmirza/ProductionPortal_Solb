using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Newtonsoft.Json;

namespace WebAPICode.Helpers
{
    public class DBHelper
    {
        //private static readonly string connectionString = "data source=sql8002.site4now.net;initial catalog=db_ac68e9_productionportal;persist security info=True;user id=db_ac68e9_productionportal_admin;password=P@solb2192929;";
        private static readonly string connectionString = "data source=10.1.10.115\\PROD01;initial catalog=Production_Solb;persist security info=True;user id=WebReportViewer;password=WebReportViewer;";

        public DataTable GetTableFromSP(string sp, Dictionary<string, object> parametersCollection)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };

                foreach (KeyValuePair<string, object> parameter in parametersCollection)
                    command.Parameters.AddWithValue(parameter.Key, parameter.Value);

                DataSet dataSet = new DataSet();
                (new SqlDataAdapter(command)).Fill(dataSet);
                command.Parameters.Clear();

                if (dataSet.Tables.Count > 0)
                {
                    return dataSet.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
            finally
            {
                connection.Close();

            }
        }

        public static List<T> GetList<T>(string query)
        {
            var list = new List<T>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            list = Newtonsoft.Json.JsonConvert
                                .DeserializeObject<List<T>>(
                                    Newtonsoft.Json.JsonConvert.SerializeObject(dt)
                                );
                        }
                    }
                }
            }

            return list;
        }

        public DataTable GetTableFromQuery(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 🔴 Log error here if you have logging
                throw;
            }

            return dt;
        }


        public DataTable GetTableFromSP(string sp, SqlParameter[] prms)
        {


            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                connection.Open();

                command.Parameters.AddRange(prms);

                DataSet dataSet = new DataSet();
                (new SqlDataAdapter(command)).Fill(dataSet);
                command.Parameters.Clear();

                if (dataSet.Tables.Count > 0)
                {
                    return dataSet.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public DataTable GetTableFromSP(string sp)
        {


            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            try
            {
                command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                connection.Open();

                DataSet dataSet = new DataSet();
                (new SqlDataAdapter(command)).Fill(dataSet);
                command.Parameters.Clear();

                if (dataSet.Tables.Count > 0)
                {
                    return dataSet.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
            finally
            {
                connection.Close();
                command.Dispose();
            }
        }

        public void ExecuteNonQuery(string sp, SqlParameter[] prms)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            try
            {
                command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                connection.Open();

                command.Parameters.AddRange(prms);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                connection.Close();
                command.Dispose();
            }
        }

        public void ExecuteNonQuery(string sp, SqlParameter prms)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            try
            {
                command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                connection.Open();
                prms.SqlDbType = SqlDbType.Structured;
                command.Parameters.Add(prms);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
                command.Dispose();
            }
        }

        public void ExecuteNonQuery(string sp, SqlParameter prm, SqlParameter[] prms)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            try
            {
                command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                connection.Open();
                prm.SqlDbType = SqlDbType.Structured;
                command.Parameters.Add(prm);
                command.Parameters.AddRange(prms);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
                command.Dispose();
            }
        }

        public DataTable GetTableRow(string sp, SqlParameter[] prms)
        {


            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                command.Parameters.AddRange(prms);
                connection.Open();

                DataSet dataSet = new DataSet();
                (new SqlDataAdapter(command)).Fill(dataSet);
                command.Parameters.Clear();

                if (dataSet.Tables.Count > 0)
                {
                    return dataSet.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public DataSet GetDatasetFromSP(string sp, SqlParameter[] prms)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                SqlCommand command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                connection.Open();

                command.Parameters.AddRange(prms);

                DataSet dataSet = new DataSet();
                (new SqlDataAdapter(command)).Fill(dataSet);
                command.Parameters.Clear();

                return dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
                //return null;
            }
            finally
            {
                connection.Close();
            }
        }

        public object ExecuteScalar(string spName, SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(spName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameters);
                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        public int ExecuteNonQueryReturn(string sp, SqlParameter[] prms)
        {


            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            try
            {
                command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                connection.Open();
                command.Parameters.AddRange(prms);
                int result = command.ExecuteNonQuery();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
                command.Dispose();
            }
        }

        public string ExecuteScalarFunction(string CommandText)
        {
            string Result = "";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            try
            {
                connection.Open();
                command = new SqlCommand(CommandText, connection);
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                Result = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
                command.Dispose();
            }

            return Result;

        }

        public void ExecuteMultipleDatatable(string sp, SqlParameter[] prms, DataSet ds)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand();
            try
            {
                command = new SqlCommand(sp, connection) { CommandType = CommandType.StoredProcedure, CommandTimeout = connection.ConnectionTimeout };
                connection.Open();
                command.Parameters.AddRange(prms);
                if (null != ds)
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        SqlParameter parameter = new SqlParameter();
                        parameter.SqlDbType = SqlDbType.Structured;

                        //DataTable.TableName is the parameter Name
                        //e.g: @AppList
                        parameter.ParameterName = dt.TableName;
                        //DataTable.DisplayExpression is the equivalent SQLType Name. i.e. Name of the UserDefined Table type
                        //e.g: AppCollectionType
                        //parameter.TypeName = dt.DisplayExpression;
                        parameter.TypeName = dt.Namespace;
                        parameter.Value = dt;

                        command.Parameters.Add(parameter);
                    }
                }
                int result = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
                command.Dispose();
            }


        }
        public DataTable GetTableFromSPWithPagination(string storedProcedureName, int skip, int take)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Skip", skip); // Parameter for skipping records
                    command.Parameters.AddWithValue("@Take", take); // Parameter for taking records

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);
                        return dataTable;
                    }
                }
            }
        }

        private string GetString(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return "";

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return "";

            return Convert.ToString(row[columnName]);
        }

        private int GetInt(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return 0;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return 0;

            int result;
            return int.TryParse(row[columnName].ToString(), out result) ? result : 0;
        }

        private int? GetNullableInt(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            int result;
            return int.TryParse(row[columnName].ToString(), out result) ? result : (int?)null;
        }

        private decimal? GetNullableDecimal(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            decimal result;
            return decimal.TryParse(row[columnName].ToString(), out result) ? result : (decimal?)null;
        }

        private DateTime? GetDate(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName))
                return null;

            if (row[columnName] == DBNull.Value || row[columnName] == null)
                return null;

            DateTime result;
            return DateTime.TryParse(row[columnName].ToString(), out result) ? result : (DateTime?)null;
        }
    }
}
