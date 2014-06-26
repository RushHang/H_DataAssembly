using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DataLibraries.DBModelAttribute;
using DataLibraries.GetAndSet;
using DataLibraries.ModelData;

namespace DataLibraries.Factory
{
    public class SplitJointSql
    {
        /// <summary>
        /// 生成每个映射类的基础信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="identification"></param>
        /// <returns></returns>
        public static ModelDataForHs BuildSql(Type type, char identification)
        {
            StringBuilder sbInsereKey = new StringBuilder();
            StringBuilder sbInsereValue = new StringBuilder();
            StringBuilder sbSelectAll = new StringBuilder();
            string tablename = string.Empty; ;

            object[] members = type.GetCustomAttributes(false);

            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].GetType() == typeof(SqlTableAttribute))
                {
                    tablename = ((SqlTableAttribute)members[i]).TableName;
                    break;
                }
            }
            if (string.IsNullOrEmpty(tablename))
            {
                throw new Exception("请确认" + type.Name + "的表名");
            }
            int identnum = 0;
            ModelDataForHs Data = new ModelDataForHs();
            List<ModelPropertyAndDelegate> lists = new List<ModelPropertyAndDelegate>();
            StringBuilder updateitem = new StringBuilder();
            foreach (PropertyInfo info in type.GetProperties())
            {
                foreach (Attribute ab in Attribute.GetCustomAttributes(info))
                {
                    if (ab.GetType() == typeof(SqlColumnAttribute))
                    {
                        SqlColumnAttribute sqlcolumn = ((SqlColumnAttribute)ab);
                        string name = sqlcolumn.ColumName;
                        if (!sqlcolumn.IsAutomatically)
                        {
                            if (identnum != 0)
                            {
                                sbInsereKey.Append(",");
                                sbInsereValue.Append(",");
                                updateitem.Append(",");
                            }
                            identnum++;
                            sbInsereKey.Append(name);
                            sbInsereValue.Append(identification + name);
                            updateitem.Append(string.Format("{0}={1}", name, identification + name));
                        }
                        sbSelectAll.Append(name + ",");
                        //各个属性的值
                        ModelPropertyAndDelegate item = new ModelPropertyAndDelegate();
                        item.PropertyName = info.Name;
                        item.GetValue = info.CreatePropertyGetterWrapper();
                        item.IsAutomatically = sqlcolumn.IsAutomatically;
                        item.SetValue = info.CreatePropertySetterWrapper();
                        item.SetValue.ParseData += SwitchType(info.PropertyType);
                        item.IsPrimaryKey = sqlcolumn.IsPrimaryKey;
                        item.ColumnName = name;
                        lists.Add(item);
                    }
                }

            }

            Data.Properys = lists.ToArray();
            Data.InsereSql = string.Format("insert into {0} ({1}) values ({2})", tablename, sbInsereKey.ToString(), sbInsereValue.ToString());

            Data.SelectAllSql = string.Format("select {0} from {1}", sbSelectAll.Remove(sbSelectAll.Length - 1, 1), tablename);
            Data.GetByIdSql = Data.SelectAllSql;
            bool havepk = false; ;
            string pkstring = "";

            foreach (ModelPropertyAndDelegate it in lists)
            {
                if (it.IsPrimaryKey)
                {
                    if (havepk == false)
                    {
                        pkstring += string.Format(" where {0}={1}", it.ColumnName, identification + it.ColumnName);
                    }
                    else
                    {
                        pkstring += string.Format(" and {0}={1}", it.ColumnName, identification + it.ColumnName);
                    }

                    havepk = true;
                }
            }
            Data.GetByIdSql += pkstring;
            Data.DeleteSql = string.Format("delete from {0}", tablename + pkstring);
            Data.UpdateSql = string.Format("update {0} set {1}", tablename, updateitem.ToString() + pkstring);


            return Data;
        }

        /// <summary>
        /// 对象创建委托
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TryParseDelegate SwitchType(Type type)
        {
            if (type == typeof(string))
                return new TryParseDelegate(TryPasrse.TryString);
            else if (type == typeof(Int32))
                return new TryParseDelegate(TryPasrse.TryInt);
            else if (type == typeof(Int16))
                return new TryParseDelegate(TryPasrse.TryShort);
            else if (type == typeof(Int64))
                return new TryParseDelegate(TryPasrse.TryLong);
            else if (type == typeof(UInt32))
                return new TryParseDelegate(TryPasrse.TryUint);
            else if (type == typeof(UInt16))
                return new TryParseDelegate(TryPasrse.TryUShort);
            else if (type == typeof(UInt64))
                return new TryParseDelegate(TryPasrse.TryULong);
            else if (type == typeof(Boolean))
                return new TryParseDelegate(TryPasrse.TryBool);
            else if (type == typeof(DateTime))
                return new TryParseDelegate(TryPasrse.TryDateTime);
            else if (type == typeof(byte))
                return new TryParseDelegate(TryPasrse.Trybyte);
            else if (type == typeof(Decimal))
                return new TryParseDelegate(TryPasrse.TryDecimal);
            else if (type == typeof(Double))
                return new TryParseDelegate(TryPasrse.TryDouble);
            else if (type == typeof(SByte))
                return new TryParseDelegate(TryPasrse.TrySByte);
            else if (type == typeof(float))
                return new TryParseDelegate(TryPasrse.Tryfloat);
            else if (type == typeof(byte[]))
                return new TryParseDelegate(TryPasrse.TryBytes);
            else
                return null;
        }
    }
}
