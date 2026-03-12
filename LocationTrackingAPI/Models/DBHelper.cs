using System;
using System.Data;
using System.Data.SqlClient;


namespace LocationTrackingAPI.Models
{
    public class DBHelper
    {

        private string strMyConnectionString = string.Empty;
        
        public DBHelper(string _strConnection)
        {
            strMyConnectionString = _strConnection;
        }
        private SqlConnection GetConection()
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(strMyConnectionString);

                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                
            }
            catch (Exception ex)
            { 
                //_lHelp.WriteFileToLocal(ex.StackTrace); 
            }

            return con;
        }

        public DataTable GetDataTableByProcedure(string strProcedureName)
        {
            DataTable dt = new DataTable();
            SqlConnection con = null;
            try
            {
                con = GetConection();
                SqlCommand cmd = new SqlCommand(strProcedureName.Trim(), con);
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                con.Close();
                //_lHelp.WriteFileToLocal(ex.StackTrace);
            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public DataTable GetDataTableByProcedure(string strProcedureName, SqlParameter[] sqlParam)
        {
            DataTable dt = new DataTable();
            SqlConnection con = null;
            try
            {
                con = GetConection();
                SqlCommand cmd = new SqlCommand(strProcedureName.Trim(), con);
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (var p in sqlParam)
                {
                    cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                con.Close();
                //_lHelp.WriteFileToLocal(ex.StackTrace);
            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public DataSet GetDataSetByProcedure(string strProcedureName)
        {
            DataSet ds = new DataSet();
            SqlConnection con = null;
            try
            {
                con = GetConection();
                SqlCommand cmd = new SqlCommand(strProcedureName.Trim(), con);
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                con.Close();
                //_lHelp.WriteFileToLocal(ex.StackTrace);
            }
            finally
            {
                con.Close();
            }

            return ds;
        }

        public DataSet GetDataSetByProcedure(string strProcedureName, SqlParameter[] sqlParam)
        {
            DataSet ds = new DataSet();
            SqlConnection con = null;
            try
            {
                con = GetConection();
                using (SqlCommand cmd = new SqlCommand(strProcedureName.Trim(), con))
                {
                    cmd.CommandTimeout = 240;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var p in sqlParam)
                    {
                        cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(ds);
                    }                        
                    
                }
                
            }
            catch (Exception ex)
            {
                con.Close();
                //_lHelp.WriteFileToLocal(ex.StackTrace);
            }
            finally
            {
                con.Close();
            }

            return ds;
        }

        public long ExecuteProcedure(string strProcedureName, SqlParameter[] sqlParam)
        {
            long nResult = 0;
            SqlConnection connection = null;
            try
            {

                connection = GetConection();
                SqlCommand cmd = new SqlCommand(strProcedureName.Trim(), connection);
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (var p in sqlParam)
                {
                    cmd.Parameters.AddWithValue(p.ParameterName, p.Value);
                }

                nResult = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
                //_lHelp.WriteFileToLocal(ex.StackTrace);
            }
            finally
            {
                connection.Close();
            }

            return nResult;
        }

        public DataTable GetDataTableByExpression(string strExp)
        {
            DataTable dt = new DataTable();
            SqlConnection con = null;
            try
            {
                con = GetConection();
                SqlCommand cmd = new SqlCommand(strExp.Trim(), con);
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                con.Close();
                //_lHelp.WriteFileToLocal(ex.StackTrace);
            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public DataSet GetDataSetByExpression(string strExp)
        {
            DataSet ds = new DataSet();
            SqlConnection con = null;
            try
            {
                con = GetConection();
                SqlCommand cmd = new SqlCommand(strExp.Trim(), con);
                cmd.CommandTimeout = 240;
                cmd.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                con.Close();
                //_lHelp.WriteFileToLocal(ex.StackTrace);
            }
            finally
            {
                con.Close();
            }

            return ds;
        }

        public long ExecuteByExpression(string strExp)
        {
            long nResult = 0;
            SqlConnection con = null;
            try
            {
                con = GetConection();
                SqlCommand cmd = new SqlCommand(strExp.Trim(), con);
                cmd.CommandTimeout = 240;
                nResult = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                con.Close();
                //_lHelp.WriteFileToLocal(ex.StackTrace);
            }
            finally
            {
                con.Close();
            }

            return nResult;
        }
    }
}