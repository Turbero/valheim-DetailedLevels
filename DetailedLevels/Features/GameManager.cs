using System.Reflection;

namespace DetailedLevels.Features
{
    public class GameManager
    {
        public static object GetPrivateValue(object obj, string fieldName, BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic)
        {
            return obj.GetType().GetField(fieldName, bindingAttr).GetValue(obj);
        }
    }
}