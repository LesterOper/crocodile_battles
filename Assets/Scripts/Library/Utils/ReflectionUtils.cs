using System;
using System.Linq;

namespace Library.Utils
{
    public static class ReflectionUtils
    {
        public static T GetAttribute<T>(this Enum e) where T : Attribute
        {
            var type = e.GetType();
            var name = Enum.GetName(type, e);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<T>()
                .SingleOrDefault();
        }
    }
}