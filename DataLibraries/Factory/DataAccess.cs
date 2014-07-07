using System;
using System.Collections.Generic;
using DataLibraries.ModelData;
using DataLibraries.DBModelAttribute;
using DataLibraries.Factory;
using System.Linq;
using System.Data;

namespace DataLibraries
{
    public delegate ModelDataForHs GetMDHDelegate(Type t);
    public class DataAccess
    {

        public DataAccess()
        {
            modelDic = new Dictionary<Type, ModelDataForHs>();
            ConnCount = 10;//保存10个默认操作对象
            GetMDH = new GetMDHDelegate(GetMDHFor);
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string Connstring { get { return _connstring; } set { _connstring = value; } }

        private string _connstring;


        /// <summary>
        /// 数据库类型
        /// </summary>
        public DBTypes DataType { get { return _datatype; } set { _datatype = value; } }

        private DBTypes _datatype;

        private IDictionary<Type, ModelDataForHs> modelDic { get; set; }
        /// <summary>
        /// 存放各个不同实例中的模型
        /// </summary>
        public static IDictionary<DataAccess, IDictionary<Type, ModelDataForHs>> Models = new Dictionary<DataAccess, IDictionary<Type, ModelDataForHs>>();
        private GetMDHDelegate GetMDH;
        /// <summary>
        /// 加载映射
        /// </summary>
        /// <param name="types"></param>
        public void AddModels(Type[] types)
        {
            char dent = Identification;
            foreach (Type t in types)
            {
                if (t.BaseType.FullName == typeof(BaseModel).FullName)
                {
                    modelDic.Add(t, SplitJointSql.BuildSql(t, dent));
                }
            }
            Models.Add(this, modelDic);
        }

        public ModelDataForHs GetMDHFor(Type t)
        {
            return modelDic[t];
        }

        private char identification;

        public char Identification
        {
            get
            {
                if (identification == char.MinValue)
                {
                    switch (DataType)
                    {
                        case DBTypes.MsSql: identification = '@'; break;

                        case DBTypes.Oracle: identification = ':'; break;

                        case DBTypes.MySql: identification = '?'; break;

                        default: identification = '@'; break;

                    }
                }
                return identification;
            }
        }

        /// <summary>
        /// 默认保存的连接数量
        /// </summary>
        public int ConnCount { get; set; }



        /// <summary>
        /// 生成一个操作对象(获取一个现有空闲的操作对象)
        /// </summary>
        /// <returns></returns>
        public IDBOperate CreateOperate()
        {
            //int id = Thread.CurrentThread.ManagedThreadId;
            //创建默认的操作对象
            if (_operates.Count <= ConnCount)
            {
                int count = ConnCount - _operates.Count;
                for (int i = 0; i < count; i++)
                {
                    _operates.Add(CreateNewOperate());
                }
            }

            IDBOperate oper = null;
            try
            {
                oper = _operates.Where(x => x.Connection.State == ConnectionState.Closed).First();
            }
            catch
            {
                oper = CreateNewOperate();
            }

            return oper;
        }

        /// <summary>
        /// 创建一个新的操作对象(不加入内存中)
        /// </summary>
        /// <returns></returns>
        public IDBOperate CreateNewOperate()
        {
            switch (_datatype)
            {
                case DBTypes.MsSql: return new MSSQL.MSSQLOperate(_connstring,GetMDH);
                case DBTypes.MySql: return new MySql.MySqlOperate(_connstring, GetMDH);
                case DBTypes.Oracle: return new Oracle.OracleOperate(_connstring, GetMDH);
                case DBTypes.SqlLite: return new SqlLite.SqlLiteOperate(_connstring, GetMDH);
            }
            return default(IDBOperate);
        }

        private IList<IDBOperate> _operates = new List<IDBOperate>();

        //private Hashtable _operate = new Hashtable();
    }
}
