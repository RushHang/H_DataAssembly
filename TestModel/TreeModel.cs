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
}
