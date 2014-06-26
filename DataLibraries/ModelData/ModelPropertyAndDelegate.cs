using DataLibraries.GetAndSet;

namespace DataLibraries.ModelData
{
    /// <summary>
    /// 属性信息类
    /// </summary>
    public class ModelPropertyAndDelegate
    {
        public string PropertyName { get; set; }

        public string ColumnName { get; set; }

        public IGetValue GetValue { get; set; }

        public ISetValue SetValue { get; set; }

        public bool IsAutomatically { get; set; }

        public bool IsPrimaryKey { get; set; }

        
    }
}
