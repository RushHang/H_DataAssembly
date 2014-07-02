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
                    da.DataType = DBTypes.MsSql;
                    da.Connstring = @"data source=RUSH-PC\MSSQL2008R2;user id=sa;password=hj1992;database=JXHG_NEWSWEB;";
                    da.AddModels(Assembly.GetExecutingAssembly().GetTypes());
                    da.ConnCount = 20;
                }
                return da;
            }
        }
    }
}
