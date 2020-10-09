using System.Data;
using System.Data.SqlClient;

namespace WPFBusinessSense_Task
{
    public class MyFunctions
    {
        public static string MyConnection
        {
            get
            {
                return WPFBusinessSense_Task.Properties.Settings.Default.BusinessSense_Task_DBConnectionString;
            }
        }

        public static bool IsFound(string sql)
        {
            int x = 0;
            using (SqlConnection connection = new SqlConnection(MyConnection))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    try
                    {
                        x = (int)command.ExecuteScalar();
                    }
                    catch
                    {
                        x = 0;
                    }
                }
            }
            return x > 0;
        }

        public static bool IsFound(string sql, object parameters, CommandType CType = CommandType.Text)
        {
            int x = 0;
            using (SqlConnection connection = new SqlConnection(MyConnection))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CType;
                    if (parameters != null)
                    {
                        if (parameters is SqlParameter[] v)
                            command.Parameters.AddRange(values: v);
                        else if (parameters is SqlParameter parameter)
                            command.Parameters.Add(value: parameter);
                    }
                    connection.Open();
                    try
                    {
                        x = (int)command.ExecuteScalar();
                    }
                    catch
                    {
                        x = 0;
                    }
                }
            }
            return x > 0;
        }

        public static object GetScalar(string sql, object parameters = null, CommandType CType = CommandType.Text)
        {
            object obj = null;
            using (SqlConnection connection = new SqlConnection(MyConnection))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CType;
                    if (parameters != null)
                    {
                        if (parameters is SqlParameter[] v)
                            command.Parameters.AddRange(values: v);
                        else if (parameters is SqlParameter parameter)
                            command.Parameters.Add(value: parameter);
                    }
                    connection.Open();
                    try
                    {
                        obj = command.ExecuteScalar();
                    }
                    catch
                    {
                        obj = null;
                    }
                }
            }
            return obj;
        }

        public static bool ExecuteNonQuery(string sql, object parameters = null, CommandType CType = CommandType.Text)
        {
            int retVal = 0;
            using (SqlConnection connection = new SqlConnection(MyConnection))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CType;
                    if (parameters != null)
                    {
                        if (parameters is SqlParameter[] v)
                            command.Parameters.AddRange(values: v);
                        else if (parameters is SqlParameter parameter)
                            command.Parameters.Add(value: parameter);
                    }
                    connection.Open();
                    try
                    {
                        retVal = command.ExecuteNonQuery();
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return retVal > 0;
        }
    }
}