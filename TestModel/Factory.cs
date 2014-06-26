using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLibraries;
using System.Reflection;

namespace TestModel
{
    public class Factory
    {
        private static DataAccess da;

        public static DataAccess Da
        {
            get
            {
                if (da == null)
                {
                    da = new DataAccess();
                    //da.DataType = DBTypes.MsSql;
                    //da.Connstring = @"data source=HANGJIAN\DB2008;user id=sa;password=hj1992;database=MyTest;";
                    da.DataType = DBTypes.SqlLite;
                    da.Connstring = @"Data Source=WishingTree;Version=3;Pooling=true;Max Pool Size=200;FailIfMessing=false";
                    da.AddModels(Assembly.GetExecutingAssembly().GetTypes());
                    da.ConnCount = 20;
                }
                return da;
            }
        }
    }
}
