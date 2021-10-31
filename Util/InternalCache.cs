using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Managed.Util
{
    static class InternalCache
    {
        internal static readonly Type AutoManagedComponentsAttributeType = typeof(AutoManagedComponentsAttribute);
        internal static readonly Type NotComponentAttributeType = typeof(NotComponentAttribute);
        internal static readonly Type ComponentAttributeType = typeof(ComponentAttribute);
        internal static readonly Type LinkComponentAttributeType = typeof(LinkComponentAttribute);
        internal static readonly Type ConfigurationAttributeType = typeof(ConfigurationAttribute);
        internal static readonly Type AutowiredAttributeType = typeof(AutowiredAttribute); 
        internal static readonly Type StaticComponentAttributeType = typeof(StaticComponentAttribute);
        internal static readonly Type VariableAttributeType = typeof(VariableAttribute);
        internal static readonly Type TextVariableType = typeof(TextVariable);
        internal static readonly Type IntVariableType = typeof(IntVariable);
        internal static readonly Type LongVariableType = typeof(LongVariable);
        internal static readonly Type BoolVariableType = typeof(BoolVariable);
        internal static readonly Type DateTimeVariableType = typeof(DateTimeVariable);
        internal static readonly Type DoubleVariableType = typeof(DoubleVariable);
        internal static readonly Type DecimalVariableType = typeof(DecimalVariable);
        internal static readonly Type VoidType = typeof(void);
        internal static readonly Type ObjectType = typeof(object);
        internal static readonly Type StringType = typeof(string);
        internal static readonly Type CharType = typeof(char);
        internal static readonly Type LongType = typeof(long);
        internal static readonly Type UlongType = typeof(ulong);
        internal static readonly Type IntType = typeof(int);
        internal static readonly Type UintType = typeof(uint);
        internal static readonly Type ShortType = typeof(short);
        internal static readonly Type UshortType = typeof(ushort);
        internal static readonly Type SbyteType = typeof(sbyte);
        internal static readonly Type ByteType = typeof(byte);
        internal static readonly Type DoubleType = typeof(double);
        internal static readonly Type FloatType = typeof(float);
        internal static readonly Type DecimalType = typeof(decimal);
        internal static readonly Type DateTimeType = typeof(DateTime);
        internal static readonly Type BooleanType = typeof(bool);
    }
}
