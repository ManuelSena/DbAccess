using System;
using System.Collections.Generic;
using System.Data;

namespace DbAccess.DbAdapter
{
    public interface IDbAdapter
    {
        IDbCommand Cmd { get; }
        IDbConnection Conn { get; }

        T ExecuteDbScalar<T>(string storedProcedure, IDbDataParameter[] parameters);
        int ExecuteQuery(string storedProcedure, IDbDataParameter[] parameters, Action<IDbDataParameter[]> returnParameters = null);
        List<T> LoadObject<T>(string storedProcedure, IDbDataParameter[] parameters = null) where T : class;
    }
}