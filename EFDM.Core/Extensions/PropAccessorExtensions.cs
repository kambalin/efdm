using System;

namespace EFDM.Core.Extensions
{
    public static class PropAccessorExtensions
    {
        public static Object GetPropValue(this Object obj, String name)
        {
            foreach (var part in name.Split('.'))
            {
                if (obj == null)
                    return null;

                var type = obj.GetType();
                var info = type.GetProperty(part);
                if (info == null)
                    return null;

                obj = info.GetValue(obj, null);
            }
            return obj;
        }

        public static T GetPropValue<T>(this Object obj, String name)
        {
            var retval = GetPropValue(obj, name);
            if (retval == null)
                return default(T);
            // throws InvalidCastException if types are incompatible
            return (T)retval;
        }
    }
}
