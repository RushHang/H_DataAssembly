
namespace DataLibraries.ModelData
{
    /// <summary>
    /// 此为拼接sql需要的所有数据
    /// </summary>
    public class ModelDataForHs
    {
        public string InsereSql { get; set; }

        public string UpdateSql { get; set; }

        public string DeleteSql { get; set; }

        public string SelectAllSql { get; set; }

        public string GetByIdSql { get; set; }

        /// <summary>
        /// model所有属性
        /// </summary>
        public ModelPropertyAndDelegate[] Properys { get; set; }
    }
}
