using System.Reflection;

namespace DetailedLevels
{
    public class ReflectionUtils
    {
        public static object GetPrivateValue(object obj, string name, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            return obj.GetType().GetField(name, bindingAttr)?.GetValue(obj);
        }
        
        public static void SetPrivateValue(object obj, string name, object value, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            obj.GetType().GetField(name, bindingAttr)?.SetValue(obj, value);
        }
        
        public static object GetPrivateMethod(object obj, string name, object[] parameters = null, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            return obj.GetType().GetMethod(name, bindingAttr)?.Invoke(obj, parameters);
        }
        
        public static void RunPrivateMethod(object obj, string name, object[] parameters = null, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            obj.GetType().GetMethod(name, bindingAttr)?.Invoke(obj, parameters);
        }
    }
}