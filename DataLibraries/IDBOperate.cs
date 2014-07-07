using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using DataLibraries.DBModelAttribute;
using DataLibraries.ModelData;

namespace DataLibraries
{
    public interface IDBOperate
    {

        bool Insere(BaseModel model);

        bool Update(BaseModel model);

        bool Delete(BaseModel model);

        T Get<T>(object id) where T : BaseModel, new();

        DataTable QueryDt(string sql, params IDataParameter[] args);

        IList<T> QueryList<T>(string sql, params IDataParameter[] args) where T : BaseModel, new();

        IDbCommand Command { get; }

        IDbConnection Connection { get; }
        /// <summary>
        /// 获取并开启事务
        /// </summary>
        IDbTransaction Transaction { get; }

        int ExecuteNonQuery(string sql, params IDataParameter[] parms);

        object ExecuteScalar(string sql, params IDataParameter[] parms);

        IDataReader ExecuteReader(string sql, params IDataParameter[] parms);

        IDataReader ExecuteReader(CommandBehavior behavior, string sql, params IDataParameter[] parms);

        IDataParameter CreateParameter();

        IList<T> QueryList<T>(int pageIndex, int pageSize, out int count, string[] where, params IDataParameter[] parms) where T : BaseModel, new();
    }
}
