using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLibraries.DBModelAttribute;

namespace TestModel
{
     [SqlTable("user")]
    public sealed class TestUser:BaseModel
    {
         [SqlColumn("Id", true,true,true)]
         public int Id { get; set; }

         //[SqlColumn("Area")]
         //public string Area { get; set; }

         [SqlColumn("Name",true)]
         public string UserName { get; set; }
    }
}
