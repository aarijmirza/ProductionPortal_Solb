using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace WebAPICode.Helpers
{
    public class OracleDBHelper
    {
        private static readonly string connectionString =
            "User Id=CWARE;Password=CWARE;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.11.220)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=L2server)))";

        /// <summary>
        /// Executes a stored procedure with optional IN parameters and expects an OUT REF CURSOR named "P_CURSOR".
        /// </summary>
        public DataTable GetTableFromSP(string sp, Dictionary<string, object> inParams = null)
        {
            using (var conn = new OracleConnection(connectionString))
            using (var cmd = new OracleCommand(sp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add IN parameters if provided
                if (inParams != null)
                {
                    foreach (var kvp in inParams)
                    {
                        var param = new OracleParameter(kvp.Key, kvp.Value ?? DBNull.Value)
                        {
                            Direction = ParameterDirection.Input
                        };
                        cmd.Parameters.Add(param);
                    }
                }

                // Add OUT REF CURSOR parameter (adjust name if your SP uses a different one)
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                var dt = new DataTable();
                conn.Open();

                using (var da = new OracleDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }

        /// <summary>
        /// Executes a stored procedure with OracleParameter array (for more control over types/directions).
        /// Assumes the caller adds the REF CURSOR parameter if required.
        /// </summary>
        public DataTable GetTableFromSP(string sp, OracleParameter[] prms)
        {
            using (var conn = new OracleConnection(connectionString))
            using (var cmd = new OracleCommand(sp, conn))
            using (var da = new OracleDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (prms != null)
                    cmd.Parameters.AddRange(prms);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        /// <summary>
        /// Executes a stored procedure without parameters.
        /// Assumes SP returns a REF CURSOR with name "P_CURSOR".
        /// </summary>
        public DataTable GetTableFromSP(string sp)
        {
            using (var conn = new OracleConnection(connectionString))
            using (var cmd = new OracleCommand(sp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add REF CURSOR OUT parameter
                cmd.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                var dt = new DataTable();
                conn.Open();

                using (var da = new OracleDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }

        /// <summary>
        /// Executes a stored procedure that does not return data (for inserts, updates, deletes).
        /// </summary>
        public void ExecuteNonQuery(string sp, OracleParameter[] prms)
        {
            using (var conn = new OracleConnection(connectionString))
            using (var cmd = new OracleCommand(sp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (prms != null)
                    cmd.Parameters.AddRange(prms);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ExecuteNonQuery(string sp, OracleParameter prm)
        {
            ExecuteNonQuery(sp, new OracleParameter[] { prm });
        }

        /// <summary>
        /// Executes a stored procedure that does not return data and returns the number of affected rows.
        /// </summary>
        public int ExecuteNonQueryReturn(string sp, OracleParameter[] prms)
        {
            using (var conn = new OracleConnection(connectionString))
            using (var cmd = new OracleCommand(sp, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (prms != null)
                    cmd.Parameters.AddRange(prms);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a scalar SQL query or function.
        /// </summary>
        public string ExecuteScalarFunction(string query)
        {
            using (var conn = new OracleConnection(connectionString))
            using (var cmd = new OracleCommand(query, conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }

        /// <summary>
        /// Executes a stored procedure and fills a DataSet.
        /// Assumes caller adds REF CURSOR parameter if needed.
        /// </summary>
        public DataSet GetDatasetFromSP(string sp, OracleParameter[] prms)
        {
            using (var conn = new OracleConnection(connectionString))
            using (var cmd = new OracleCommand(sp, conn))
            using (var da = new OracleDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (prms != null)
                    cmd.Parameters.AddRange(prms);

                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// Alias for GetTableFromSP with OracleParameter array.
        /// </summary>
        public DataTable GetTableRow(string sp, OracleParameter[] prms)
        {
            return GetTableFromSP(sp, prms);
        }

        /// <summary>
        /// Executes a stored procedure with pagination parameters.
        /// The stored procedure must accept parameters p_offset and p_fetch.
        /// </summary>
        public DataTable GetTableFromSPWithPagination(string sp, int offset, int fetch)
        {
            var prms = new OracleParameter[]
            {
                new OracleParameter("p_offset", OracleDbType.Int32) { Value = offset, Direction = ParameterDirection.Input },
                new OracleParameter("p_fetch", OracleDbType.Int32) { Value = fetch, Direction = ParameterDirection.Input },
                new OracleParameter("P_CURSOR", OracleDbType.RefCursor) { Direction = ParameterDirection.Output }
            };

            return GetTableFromSP(sp, prms);
        }
    }
}
