using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using static Jack.Managed.Util.InternalCache;

namespace Jack.Managed.Util
{
    public static class TypeMatchUtils
    {
        public static T Matching<T>(string s, T defaultValue, string format = null)
        {
            return (T)Matching(typeof(T), s, defaultValue, format);
        }

        public static object Matching(Type type, string s, object defaultValue, string format = null)
        {
            if (s == null)
            {
                return defaultValue;
            }
            else if (type == StringType)
            {
                return s;
            }
            else if (type == IntType && int.TryParse(s, out var i))
            {
                return  i;
            }
            else if (type == DoubleType && double.TryParse(s, out var d))
            {
                return d;
            }
            else if (type == BooleanType && bool.TryParse(s, out var bl))
            {
                return bl;
            }
            else if (type == LongType && long.TryParse(s, out var l))
            {
                return l;
            }
            else if (type == DateTimeType && DateTime.TryParseExact(s, format ?? "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.AssumeLocal, out var dt))
            {
                return dt;
            }
            else if (type == DecimalType && decimal.TryParse(s, out var de))
            {
                return de;
            }
            else if(type == CharType && s != null && s.Length == 1)
            {
                return s.ElementAt(0);
            }
            else if(type == FloatType && float.TryParse(s, out var f))
            {
                return f;
            }
            else if (type == ByteType && byte.TryParse(s, out var by))
            {
                return by;
            }
            else if(type == UintType && uint.TryParse(s, out var ui))
            {
                return ui;
            }
            else if (type == UlongType && ulong.TryParse(s, out var ul))
            {
                return ul;
            }
            else if(type == ShortType && short.TryParse(s, out var sh))
            {
                return sh;
            }
            else if(type == UshortType && ushort.TryParse(s, out var us))
            {
                return us;
            }
            else if(type == SbyteType && sbyte.TryParse(s, out var sb))
            {
                return sb;
            }
            else
            {
                return defaultValue;
            }
        }

        internal static object VariableMatching(Type type, TextVariable variable, string format = null)
        {
            if (type == TextVariableType)
            {
                return variable;
            }
            else if (type == IntVariableType)
            {
                return new IntVariable(variable);
            }
            else if (type == DoubleVariableType)
            {
                return new DoubleVariable(variable);
            }
            else if (type == BoolVariableType)
            {
                return new BoolVariable(variable);
            }
            else if (type == LongVariableType)
            {
                return new LongVariable(variable);
            }
            else if (type == DateTimeVariableType)
            {
                return new DateTimeVariable(variable, format);
            }
            else if (type == DecimalVariableType)
            {
                return new DecimalVariable(variable);
            }
            else
            {
                return Matching(type, variable.Value, null, format);
            }
        }
    }
}
