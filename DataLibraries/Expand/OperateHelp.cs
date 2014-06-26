using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLibraries.ModelData;
using System.Data;
using DataLibraries.Factory;

namespace DataLibraries
{
    public static class OperateHelp
    {
        /// <summary>
        /// 转换为一个IList
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IList<TResult> ToList<TResult>(this DataTable dt) where TResult : class,new()
        {
            IList<TResult> list = new List<TResult>();
            ModelDataForHs mdh = GetMDH(typeof(TResult));
            if (mdh != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TResult model = new TResult();
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
            //else//TResult是普通类型(简单类型)时
            //{
            //    TryParseDelegate parsed = SplitJointSql.SwitchType(typeof(TResult));
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        list.Add((TResult)parsed(dr[0]));
            //    }
            //}

            return list;
        }

        /// <summary>
        /// 转换为一个DataTable
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<TResult>(this IEnumerable<TResult> values) where TResult : class
        {
            DataTable dt = new DataTable();
            ModelDataForHs mdh = GetMDH(typeof(TResult));
            if (mdh != null)
            {
                foreach (ModelPropertyAndDelegate item in mdh.Properys)
                {
                    dt.Columns.Add(item.ColumnName);
                }
                foreach (var value in values)
                {
                    DataRow dr = dt.NewRow();
                    foreach (ModelPropertyAndDelegate item in mdh.Properys)
                    {
                        dr[item.ColumnName] = item.GetValue.Get(value);
                    }
                    dt.Rows.Add(dr);
                }
            }

            return dt;
        }

        private static ModelDataForHs GetMDH(Type t)
        {
            foreach (var key in DataAccess.Models.Keys)
            {
                if (DataAccess.Models[key][t] != null)
                    return DataAccess.Models[key][t];
            }
            return null;
        }
    }
}
