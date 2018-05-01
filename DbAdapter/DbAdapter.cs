using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbAccess.Tools;

namespace DbAccess.DbAdapter
{
    public class DbAdapter : IDbAdapter
    {
        public IDbConnection Conn { get; private set; }
        public IDbCommand Cmd { get; private set; }
        //Dependancy Injection; one will always want to inject to an interface
        public DbAdapter(IDbCommand command, IDbConnection conn)
        {
            Cmd = command;
            Conn = conn;
        }

        public List<T> LoadObject<T>(string storedProcedure,
            IDbDataParameter[] parameters = null) where T : class
        {
            List<T> list = new List<T>();
            //Before
            //using (SqlConnection conn = new SqlConnection())
            //using (SqlCommand cmd = new SqlCommand())
            //after
            using (IDbConnection conn = Conn)
            using (IDbCommand cmd = Cmd)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                //---------ADO.NET you need these------------
                cmd.Connection = conn;
                cmd.CommandTimeout = 5000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedure;
                // --------------------------------
                if (parameters != null)
                {
                    foreach (IDbDataParameter parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                }

                IDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //TODO: Map to Object
                    list.Add(DataMapper<T>.Instance.MapToObject(reader));
                }
            }
            return list;
        }

        public int ExecuteQuery(string storedProcedure,
            IDbDataParameter[] parameters,
            Action<IDbDataParameter[]> returnParameters = null)
        {
            using (IDbConnection conn = Conn)
            using (IDbCommand cmd = Cmd)
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandTimeout = 5000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedure;
                foreach (IDbDataParameter parameter in parameters)
                    cmd.Parameters.Add(parameter);
                int returnValue = cmd.ExecuteNonQuery();

                returnParameters?.Invoke(parameters);
                //(simplified) 
                //understanding for student
                //if (returnParameters != null)
                //{
                //    returnParameters(parameters);
                //}
                return returnValue;
            }
        }
        /*-------------------------------------------------------*/
        public T ExecuteDbScalar<T>(string storedProcedure, IDbDataParameter[] parameters)
        {
            using (IDbConnection conn = Conn)
            using (IDbCommand cmd = Cmd)
            {//ADO.NET
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                cmd.Connection = conn;
                cmd.CommandTimeout = 5000;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedure;
                foreach (IDbDataParameter parameter in parameters)
                    cmd.Parameters.Add(parameter);

                object obj = cmd.ExecuteScalar();
                return (T)obj;
                //Convert.ChangeType(obj, typeof(T));
            }
        }
    }
}
