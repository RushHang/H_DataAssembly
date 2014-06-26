﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLibraries.ModelData;
using System.Data.OracleClient;
using System.Data;

namespace DataLibraries.Oracle
{
    public class OracleOperate:IDBOperate
    {
        public OracleOperate(string connstring, IDictionary<Type, ModelDataForHs> modeDIC)
        {

            _Connection = new OracleConnection(connstring);
            ModeDIC = modeDIC;
            _Command = _Connection.CreateCommand();
        }
        #region 私有对象

        private OracleConnection _Connection;

        private OracleCommand _Command;

        private OracleTransaction _Transaction;

        private IDictionary<Type, ModelDataForHs> ModeDIC;

        private void AddBasic(params IDataParameter[] pams)
        {
            foreach (IDataParameter item in pams)
            {
                if (item.Value == null)
                    item.Value = DBNull.Value;
                _Command.Parameters.Add(item);
            }
            if (_Connection.State != ConnectionState.Open)
                _Connection.Open();
        }

        #endregion

        public bool Insere(object model)
        {
            ModelDataForHs mdh = ModeDIC[model.GetType()];

            List<IDataParameter> parms = new List<IDataParameter>();

            foreach (ModelPropertyAndDelegate item in mdh.Properys)
            {
                IDataParameter parm = new OracleParameter();
                if (item.IsAutomatically == false)
                {
                    parm.ParameterName = item.ColumnName;
                    parm.Value = item.GetValue.Get(model);
                    parms.Add(parm);
                }
            }
            ModelPropertyAndDelegate[] automaticallys;
            try
            {
                automaticallys = mdh.Properys.Where(x => x.IsAutomatically == true).ToArray();
            }
            catch
            {
                automaticallys = new ModelPropertyAndDelegate[0];
            }
            string sql = "";
            if (automaticallys.Length > 0)
            {
                //通过复杂语句获取插入时的自动生成的ID和其他信息
                sql = "DECLARE {0} BEGIN {1} returning {2} into {3};";
                string sqloutput = "";
                for (int i = 0; i < automaticallys.Length; i++)
                {
                    if (i != 0)
                    {
                        sql = string.Format(sql, "{0}", "{1}", ",{2}", ",{3}");
                    }
                    sql = string.Format(sql, "v_" + automaticallys[i].ColumnName + " VARCHAR2(100);{0}", "{1}", automaticallys[i].ColumnName + "{2}", "v_" + automaticallys[i].ColumnName + "{3}");
                    sqloutput += "dbms_output.put_line(v_" + automaticallys[i].ColumnName + ");";
                }
                sql = string.Format(sql, "", mdh.InsereSql, "", "") + sqloutput + "END;";
                DataTable dt = QueryDt(sql, parms.ToArray());
                if (dt.Rows.Count == 0)
                    return false;
                foreach (ModelPropertyAndDelegate item in automaticallys)
                {
                    item.SetValue.Set(model, dt.Rows[0][item.ColumnName]);
                }
            }
            else
            {
                sql = mdh.InsereSql;
                if (ExecuteNonQuery(sql, parms.ToArray()) <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Update(object model)
        {
            ModelDataForHs mdh = ModeDIC[model.GetType()];

            List<IDataParameter> parms = new List<IDataParameter>();
            foreach (ModelPropertyAndDelegate item in mdh.Properys)
            {
                IDataParameter parm = new OracleParameter();
                if (item.IsAutomatically == false && item.IsPrimaryKey == false)
                {
                    parm.ParameterName = item.ColumnName;
                    parm.Value = item.GetValue.Get(model);
                    parms.Add(parm);
                }
            }

            ModelPropertyAndDelegate[] pks;
            try
            {
                pks = mdh.Properys.Where(x => x.IsPrimaryKey == true).ToArray();
            }
            catch
            {
                throw new Exception("未设置主键，无法通过此方法修改！");
            }
            foreach (ModelPropertyAndDelegate item in pks)
            {
                IDataParameter parm = new OracleParameter();
                parm.ParameterName = item.ColumnName;
                parm.Value = item.GetValue.Get(model);
                parms.Add(parm);
            }

            if (ExecuteNonQuery(mdh.UpdateSql, parms.ToArray()) <= 0)
            {
                return false;
            }
            return true;
        }

        public bool Delete(object model)
        {
            ModelDataForHs mdh = ModeDIC[model.GetType()];

            ModelPropertyAndDelegate[] pks;
            try
            {
                pks = mdh.Properys.Where(x => x.IsPrimaryKey == true).ToArray();
            }
            catch
            {
                throw new Exception("未设置主键，无法通过此方法查询！");
            }
            List<IDataParameter> parms = new List<IDataParameter>();
            foreach (ModelPropertyAndDelegate item in pks)
            {
                IDataParameter parm = new OracleParameter();
                parm.ParameterName = item.ColumnName;
                parm.Value = item.GetValue.Get(model);
                parms.Add(parm);
            }
            if (ExecuteNonQuery(mdh.DeleteSql, parms.ToArray()) <= 0)
            {
                return false;
            }
            return true;
        }

        public T Get<T>(object id) where T : new()
        {
            T model = new T();
            ModelDataForHs mdh = ModeDIC[typeof(T)];
            IDataParameter parm = new OracleParameter();
            ModelPropertyAndDelegate pk;
            try
            {
                pk = mdh.Properys.Where(x => x.IsPrimaryKey == true).First();
            }
            catch
            {
                throw new Exception("未设置主键，无法通过此方法查询！");
            }
            parm.ParameterName = pk.ColumnName;
            parm.Value = id;
            using (DataTable dt = QueryDt(mdh.GetByIdSql, parm))
            {
                if (dt.Rows.Count > 1)
                {
                    throw new Exception("有多条数据！");
                }
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (ModelPropertyAndDelegate item in mdh.Properys)
                    {
                        item.SetValue.Set(model, dr[item.ColumnName]);
                    }
                }
            }

            return model;
        }

        private void Get(object model)
        {
            ModelDataForHs mdh = ModeDIC[model.GetType()];
            List<IDataParameter> list = new List<IDataParameter>();

            ModelPropertyAndDelegate[] pks;
            try
            {
                pks = mdh.Properys.Where(x => x.IsPrimaryKey == true).ToArray();
            }
            catch
            {
                throw new Exception("未设置主键，无法通过此方法查询！");
            }
            foreach (var item in pks)
            {
                IDataParameter parm = new OracleParameter();
                parm.ParameterName = item.ColumnName;
                parm.Value = item.GetValue.Get(model);
                list.Add(parm);
            }
            using (DataTable dt = QueryDt(mdh.GetByIdSql, list.ToArray()))
            {
                if (dt.Rows.Count > 1)
                {
                    throw new Exception("有多条数据！");
                }
                foreach (DataRow dr in dt.Rows)
                {
                    foreach (ModelPropertyAndDelegate item in mdh.Properys)
                    {
                        item.SetValue.Set(model, dr[item.ColumnName]);
                    }
                }
            }
        }

        public DataTable QueryDt(string sql, params IDataParameter[] args)
        {
            DataSet ds = new DataSet();
            try
            {
                _Command.CommandText = sql;
                AddBasic(args);

                OracleDataAdapter da = new OracleDataAdapter(_Command);

                da.Fill(ds);
                da.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _Command.Parameters.Clear();
            }
            return ds.Tables[0];
        }

        public IList<T> QueryList<T>(string sql, params IDataParameter[] args) where T : new()
        {
            IList<T> list = new List<T>();

            ModelDataForHs mdh = ModeDIC[typeof(T)];
            using (DataTable dt = QueryDt(sql, args))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T model = new T();
                    foreach (ModelPropertyAndDelegate item in mdh.Properys)
                    {
                        try
                        {
                            item.SetValue.Set(model, dr[item.ColumnName]);
                        }
                        catch { }
                    }
                    list.Add(model);
                }
            }

            return list;
        }

        public IDbCommand Command
        {
            get
            {
                return _Command;
            }
        }

        public IDbConnection Connection
        {
            get
            {
                return _Connection;
            }
        }

        public IDbTransaction Transaction
        {
            get
            {
                if (_Transaction == null)
                {
                    _Transaction = _Connection.BeginTransaction();
                    _Command.Transaction = _Transaction;
                }

                return _Transaction;
            }
        }

        public int ExecuteNonQuery(string sql, params IDataParameter[] parms)
        {
            _Command.CommandText = sql;
            int count = 0;
            try
            {
                AddBasic(parms);
                count = _Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _Command.Parameters.Clear();
            }

            return count;
        }

        public object ExecuteScalar(string sql, params IDataParameter[] parms)
        {
            _Command.CommandText = sql;
            object rmsg = 0;
            try
            {
                AddBasic(parms);
                rmsg = _Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _Command.Parameters.Clear();
            }

            return rmsg;
        }

        public IDataReader ExecuteReader(string sql, params IDataParameter[] parms)
        {
            _Command.CommandText = sql;
            IDataReader rmsg;
            try
            {
                AddBasic(parms);
                rmsg = _Command.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _Command.Parameters.Clear();
            }

            return rmsg;
        }

        public IDataReader ExecuteReader(CommandBehavior behavior, string sql, params IDataParameter[] parms)
        {
            _Command.CommandText = sql;
            IDataReader rmsg;
            try
            {
                AddBasic(parms);
                rmsg = _Command.ExecuteReader(behavior);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _Command.Parameters.Clear();
            }

            return rmsg;
        }

        public IDataParameter CreateParameter()
        {
            return new OracleParameter();
        }
    }
}
