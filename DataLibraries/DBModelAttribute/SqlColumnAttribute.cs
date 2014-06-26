using System;

namespace DataLibraries.DBModelAttribute
{
    [Serializable]
    public class SqlColumnAttribute:Attribute
    {

        public SqlColumnAttribute(string name)
        {
            this.ColumName = name;
            this.IsPrimaryKey = false;
            this.IsNotNull = false;
            this.IsAutomatically = false;
        }

        public SqlColumnAttribute(string name,bool notNull)
        {
            this.ColumName = name;
            this.IsPrimaryKey = false;
            this.IsNotNull = notNull;
            this.IsAutomatically = false;
        }

        public SqlColumnAttribute(string name, bool notNull,bool iskey)
        {
            this.ColumName = name;
            this.IsPrimaryKey = iskey;
            this.IsNotNull = notNull;
            this.IsAutomatically = false;
        }

        public SqlColumnAttribute(string name, bool notNull, bool iskey,bool isautomatically)
        {
            this.ColumName = name;
            this.IsPrimaryKey = iskey;
            this.IsNotNull = notNull;
            this.IsAutomatically = isautomatically;
        }

        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsNotNull { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumName { get; set; }

        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否为数据库自动生成
        /// </summary>
        public bool IsAutomatically { get; set; }
    }
}
