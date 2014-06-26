using System;

namespace DataLibraries.DBModelAttribute
{
    [Serializable]
    public class SqlTableAttribute : Attribute
    {
        public SqlTableAttribute(string name)
        {
            this.TableName = name;
        }

        public string TableName { get; set; }
    }
}
