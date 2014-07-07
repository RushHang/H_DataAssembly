using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using DataLibraries.ModelData;
using DataLibraries.DBModelAttribute;
using System.Text;

namespace DataLibraries.MSSQL
{
    public class MSSQLOperate : IDBOperate
    {
        private GetMDHDelegate GetMDH;
        public MSSQLOperate(string connstring, GetMDHDelegate modeDIC)
        {
            _Connection = new SqlConnection(connstring);
            GetMDH = modeDIC;
            _Command = _Connection.CreateCommand();
        }

        #region 私有对象

        private SqlConnection _Connection;

        private SqlCommand _Command;

        private SqlTransaction _Transaction;


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

        public bool Insere(BaseModel model)
        {
            ModelDataForHs mdh = GetMDH(model.GetType());

            List<IDataParameter> parms = new List<IDataParameter>();

            foreach (ModelPropertyAndDelegate item in mdh.Properys)
            {
                IDataParameter parm = new SqlParameter();
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
            string sql = mdh.InsereSql;
            if (automaticallys.Length > 0)
            {
                string output = "output";
                foreach (ModelPropertyAndDelegate item in automaticallys)
                {
                    output += " inserted." + item.ColumnName + ",";
                }
                output = output.Remove(output.Length - 1);

                sql = sql.Replace("values", output + " values");
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
                if (ExecuteNonQuery(sql, parms.ToArray()) <= 0)
                {
                    return false;
                }

            }
            model.ClearState();
            return true;
        }

        public bool Update(BaseModel model)
        {
            if (string.IsNullOrEmpty(model.ChangeProperty))
            {
                return true;
            }

            ModelDataForHs mdh = GetMDH(model.GetType());

            List<IDataParameter> parms = new List<IDataParameter>();
            StringBuilder updateitem = new StringBuilder();
            foreach (ModelPropertyAndDelegate item in mdh.Properys)
            {
                IDataParameter parm = new SqlParameter();
                if (item.IsAutomatically == false && item.IsPrimaryKey == false && model.ChangeProperty.Contains(item.PropertyName))
                {
                    updateitem.Append(string.Format("{0}={1},", item.ColumnName, mdh.Identification + item.ColumnName));
                    parm.ParameterName = item.ColumnName;
                    parm.Value = item.GetValue.Get(model);
                    parms.Add(parm);
                }
            }
            updateitem.Remove(updateitem.Length - 1, 1);

            ModelPropertyAndDelegate[] pks;
            try
            {
                pks = mdh.Properys.Where(x => x.IsPrimaryKey == true).ToArray();
            }
            catch
            {
                throw new Exception("未设置主键，无法通过此方法修改！");
            }
            string pkstring = "";
            int i = 0;
            foreach (ModelPropertyAndDelegate item in pks)
            {
                if (i == 0)
                {
                    pkstring += string.Format(" where {0}={1}", item.ColumnName, mdh.Identification + item.ColumnName);
                }
                else
                {
                    pkstring += string.Format(" and {0}={1}", item.ColumnName, mdh.Identification + item.ColumnName);
                }
                IDataParameter parm = new SqlParameter();
                parm.ParameterName = item.ColumnName;
                parm.Value = item.GetValue.Get(model);
                parms.Add(parm);
                i++;
            }

            if (ExecuteNonQuery(string.Format("update {0} set {1}", mdh.TableName, updateitem.ToString() + pkstring), parms.ToArray()) <= 0)
            {
                return false;
            }
            return true;
        }

        public bool Delete(BaseModel model)
        {
            ModelDataForHs mdh = GetMDH(model.GetType());

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
                IDataParameter parm = new SqlParameter();
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

        public T Get<T>(object id) where T : BaseModel, new()
        {
            T model = new T();
            ModelDataForHs mdh = GetMDH(typeof(T));
            IDataParameter parm = new SqlParameter();
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
            model.ClearState();
            return model;
        }

        public DataTable QueryDt(string sql, params IDataParameter[] args)
        {
            DataSet ds = new DataSet();
            try
            {
                _Command.CommandText = sql;
                AddBasic(args);

                SqlDataAdapter da = new SqlDataAdapter(_Command);

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

        public IList<T> QueryList<T>(string sql, params IDataParameter[] args) where T : BaseModel, new()
        {
            IList<T> list = new List<T>();

            ModelDataForHs mdh = GetMDH(typeof(T));
            using (DataTable dt = QueryDt(sql, args))
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T model = new T();
                    foreach (ModelPropertyAndDelegate item in mdh.Properys)
                    {
                        try
                        {
                            if (dr[item.ColumnName] != null)
                            { item.SetValue.Set(model, dr[item.ColumnName]); }
                        }
                        catch { }
                    }
                    model.ClearState();
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
            return new SqlParameter();
        }

        public IList<T> QueryList<T>(int pageIndex, int pageSize, out int count, string[] where, params IDataParameter[] parms) where T : BaseModel, new()
        {
            IList<T> list = new List<T>();

            ModelDataForHs mdh = GetMDH(typeof(T));
            ModelPropertyAndDelegate pk;
            try
            {
                pk = mdh.Properys.Where(x => x.IsPrimaryKey == true).First();
            }
            catch
            {
                throw new Exception("未设置主键，无法通过此方法查询！");
            }
            string sql = "";
            
            string conditions = where.ArrayToString(" and ");
            if (pageSize == 0)
            { sql = mdh.SelectAllSql + " where 1=1 " + conditions; }
            else
            {
                pageIndex--;
                string fengy = string.Format(" number between {0} and {1}", pageSize * pageIndex + 1, pageSize * (pageIndex + 1));
                sql = string.Format("select * from ({0} where 1=1 {1}) trs where {2}", mdh.SelectAllSql.Replace("from", ",ROW_NUMBER() OVER (ORDER BY " + pk.ColumnName + ") as number from"), conditions, fengy);
            }
            count = Convert.ToInt32(ExecuteScalar(string.Format("select count(1) from {0} {1}", mdh.TableName, conditions), parms));


            return QueryList<T>(sql, parms);
        }

    }
}
