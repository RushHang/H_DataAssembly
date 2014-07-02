using System.Text;
using System;
namespace DataLibraries.DBModelAttribute
{
    [Aop]
    public class BaseModel : ContextBoundObject
    {
        private StringBuilder _changeProperty=new StringBuilder();

        public string ChangeProperty
        {
            get {
                return _changeProperty.ToString();
            }
        }

        public void ClearState()
        {
            _changeProperty.Remove(0, _changeProperty.Length);
        }

        protected void SetState(string propertyname)
        {
            if (!ChangeProperty.Contains(propertyname))
            {
                _changeProperty.Append(propertyname + ",");
            }
        }
    }
}
