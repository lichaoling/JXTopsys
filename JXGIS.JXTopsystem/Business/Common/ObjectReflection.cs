using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace JXGIS.JXTopsystem.Business.Common
{
    public class ObjectReflection
    {
        /// <summary>
        /// 反射赋值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static PropertyInfo[] GetPropertyInfos(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }
        /// <summary>
        /// 实体属性反射
        /// </summary>
        /// <typeparam name="S">赋值对象</typeparam>
        /// <typeparam name="T">被赋值对象</typeparam>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public static void AutoMapping<S, T>(S s, T t)
        {
            PropertyInfo[] pps = GetPropertyInfos(s.GetType());
            Type target = t.GetType();
            foreach (var pp in pps)
            {
                PropertyInfo targetPP = target.GetProperty(pp.Name);
                object value = pp.GetValue(s, null);
                if (targetPP != null && value != null)
                    targetPP.SetValue(t, value, null);
            }
        }
        /// <summary>
        /// 利用反射和字典中属性名称，用修改后的实体数据对原实体数据进行赋值
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceData">那些修改了的字段组成的实体</param>
        /// <param name="targetData">从数据库中查询出来的一条老的数据实体</param>
        /// <param name="Dic">字典中保存了一个实体中修改了的字段和值，这里需用到修改了的字段名称</param>
        public static void ModifyByReflection<S, T>(S sourceData, T targetData, Dictionary<string, object> Dic)
        {
            Type targetType = targetData.GetType();
            Type sourceType = sourceData.GetType();
            foreach (var key in Dic.Keys)
            {
                PropertyInfo targetPP = targetType.GetProperty(key);
                PropertyInfo sourcePP = sourceType.GetProperty(key);
                var value = sourcePP.GetValue(sourceData);
                targetPP.SetValue(targetData, value);
            }
        }
    }
}