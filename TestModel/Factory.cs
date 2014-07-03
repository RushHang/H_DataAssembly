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
                    da.DataType = DBTypes.SqlLite;
                    //da.Connstring = @"data source=RUSH-PC\MSSQL2008R2;user id=sa;password=hj1992;database=JXHG_NEWSWEB;";
                    //da.Connstring = @"Server=127.0.0.1;Database=test;Uid=root;Pwd=hj1992;";
                    //da.Connstring = @"Data Source=IDS;User ID=system;Password=dbwork;Unicode=True";
                    da.Connstring = @"Data Source=testdata;Version=3;Pooling=true;Max Pool Size=200;FailIfMessing=false";
                    da.AddModels(Assembly.GetExecutingAssembly().GetTypes());
                    da.ConnCount = 20;
                }
                return da;
            }
        }
    }
}
