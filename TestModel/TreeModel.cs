using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLibraries.DBModelAttribute;

namespace TestModel
{
    [SqlTable("Tree")]
    public class TreeModel : BaseModel
    {
        [SqlColumn("Id", true, true)]
        public string Id { get; set; }

        [SqlColumn("Content", true)]
        public string Content { get; set; }

        [SqlColumn("IsText", true)]
        public bool IsText { get; set; }
    }

    [SqlTable("report_dept")]
    public class REPORT_DEPTModel : BaseModel
    {
        [SqlColumn("ID", true, true, true)]
        public string ID { get; set; }

        [SqlColumn("Type_ID")]
        public string Type_ID { get; set; }

        [SqlColumn("dept_id")]
        public string DEPT_ID { get; set; }

        [SqlColumn("report_year")]
        public string REPORT_YEAR { get; set; }

        [SqlColumn("report_name")]
        public string REPORT_NAME { get; set; }
    }
}
