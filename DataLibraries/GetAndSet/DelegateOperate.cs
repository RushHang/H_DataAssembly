using System;
using System.Reflection;

namespace DataLibraries.GetAndSet
{
    /// <summary>
    /// 此方法用于对get，set的泛型委托
    /// </summary>
    public static class DelegateOperate
    {
        public static IGetValue CreatePropertyGetterWrapper(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (propertyInfo.CanRead == false)
                throw new InvalidOperationException("属性不支持读操作。");

            MethodInfo mi = propertyInfo.GetGetMethod(true);

            if (mi.GetParameters().Length > 0)
                throw new NotSupportedException("不支持构造索引器属性的委托。");

            Type instanceType = typeof(GetterWrapper<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
            return (IGetValue)Activator.CreateInstance(instanceType, propertyInfo);
        }

        public static ISetValue CreatePropertySetterWrapper(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");
            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException("属性不支持写操作。");

            MethodInfo mi = propertyInfo.GetSetMethod(true);

            if (mi.GetParameters().Length > 1)
                throw new NotSupportedException("不支持构造索引器属性的委托。");

            Type instanceType = typeof(SetterWrapper<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType);
            return (ISetValue)Activator.CreateInstance(instanceType, propertyInfo);
        }
    }

    public interface IGetValue
    {
        object Get(object target);
    }

    public class GetterWrapper<TTarget, TValue> : IGetValue
    {
        private Func<TTarget, TValue> _getter;

        public GetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanRead == false)
                throw new InvalidOperationException(propertyInfo.Name + "属性不支持读操作。");

            MethodInfo m = propertyInfo.GetGetMethod(true);
            _getter = (Func<TTarget, TValue>)Delegate.CreateDelegate(typeof(Func<TTarget, TValue>), null, m);
        }

        public TValue GetValue(TTarget target)
        {
            return _getter(target);
        }
        object IGetValue.Get(object target)
        {
            return _getter((TTarget)target);
        }
    }

    public interface ISetValue
    {
        void Set(object target, object val);

        event TryParseDelegate ParseData;
    }

    public class SetterWrapper<TTarget, TValue> : ISetValue
    {
        private Action<TTarget, TValue> _setter;

        public SetterWrapper(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (propertyInfo.CanWrite == false)
                throw new NotSupportedException(propertyInfo.Name + "属性不支持写操作。");

            MethodInfo m = propertyInfo.GetSetMethod(true);
            _setter = (Action<TTarget, TValue>)Delegate.CreateDelegate(typeof(Action<TTarget, TValue>), null, m);
        }

        public void SetValue(TTarget target, TValue val)
        {
            _setter(target, val);
        }

        void ISetValue.Set(object target, object val)
        {
            //取消dbnull时的类型赋值报错
            if (val == DBNull.Value)
                val = null;
            if (ParseData != null)
                val = ParseData(val);//触发事件转换类型
            _setter((TTarget)target, (TValue)val);
        }

        public event TryParseDelegate ParseData;
    }
}
