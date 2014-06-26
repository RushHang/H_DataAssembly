using System;

namespace DataLibraries
{
    public static class TryPasrse
    {
        #region object对象类型转换
        public static object TryInt(object s)
        {
            return Int32.Parse(s == null ? "" : s.ToString());
        }

        public static object TryShort(object s)
        {
            return Int16.Parse(s == null ? "" : s.ToString());
        }

        public static object TryLong(object s)
        {
            return Int64.Parse(s == null ? "" : s.ToString());
        }

        public static object TryUint(object s)
        {
            return UInt32.Parse(s == null ? "" : s.ToString());
        }

        public static object TryUShort(object s)
        {
            return UInt16.Parse(s == null ? "" : s.ToString());
        }

        public static object TryULong(object s)
        {
            return UInt64.Parse(s == null ? "" : s.ToString());
        }

        public static object TryBool(object s)
        {
            return Boolean.Parse(s == null ? "" : s.ToString());
        }

        public static object TryString(object s)
        {
            return s==null?"":s.ToString();
        }

        public static object TryDateTime(object s)
        {
            return DateTime.Parse(s == null ? "" : s.ToString());
        }

        public static object Trybyte(object s)
        {
            return byte.Parse(s == null ? "" : s.ToString());
        }

        public static object TryDecimal(object s)
        {
            return Decimal.Parse(s == null ? "" : s.ToString());
        }

        public static object TryDouble(object s)
        {
            return Double.Parse(s == null ? "" : s.ToString());
        }

        public static object TrySByte(object s)
        {
            return SByte.Parse(s == null ? "" : s.ToString());
        }

        public static object Tryfloat(object s)
        {
            return float.Parse(s == null ? "" : s.ToString());
        }

        public static object TryBytes(object s)
        {
            return (byte[])s;
        }
        #endregion

        #region string对象类型转换
        public static Int32 TryInt(string s)
        {
            return Int32.Parse(s);
        }

        public static Int16 TryShort(string s)
        {
            return Int16.Parse(s);
        }

        public static Int64 TryLong(string s)
        {
            return Int64.Parse(s);
        }

        public static UInt32 TryUint(string s)
        {
            return UInt32.Parse(s);
        }

        public static UInt16 TryUShort(string s)
        {
            return UInt16.Parse(s);
        }

        public static UInt64 TryULong(string s)
        {
            return UInt64.Parse(s);
        }

        public static Boolean TryBool(string s)
        {
            return Boolean.Parse(s);
        }

        public static string TryString(string s)
        {
            return s.ToString();
        }

        public static DateTime TryDateTime(string s)
        {
            return DateTime.Parse(s);
        }

        public static byte Trybyte(string s)
        {
            return byte.Parse(s);
        }

        public static Decimal TryDecimal(string s)
        {
            return Decimal.Parse(s);
        }

        public static Double TryDouble(string s)
        {
            return Double.Parse(s);
        }

        public static SByte TrySByte(string s)
        {
            return SByte.Parse(s);
        }

        public static float Tryfloat(string s)
        {
            return float.Parse(s);
        }
        #endregion

        #region string对象类型强制转换
        public static Int32 toInt(string s)
        {
            return Convert.ToInt32(s);
        }

        public static Int16 toShort(string s)
        {
            return Convert.ToInt16(s);
        }

        public static Int64 toLong(string s)
        {
            return Convert.ToInt64(s);
        }

        public static UInt32 toUint(string s)
        {
            return Convert.ToUInt32(s);
        }

        public static UInt16 toUShort(string s)
        {
            return Convert.ToUInt16(s);
        }

        public static UInt64 toULong(string s)
        {
            return Convert.ToUInt64(s);
        }

        public static Boolean toBool(string s)
        {
            return Convert.ToBoolean(s);
        }

        public static string toString(string s)
        {
            return Convert.ToString(s);
        }

        public static DateTime toDateTime(string s)
        {
            return Convert.ToDateTime(s);
        }

        public static byte tobyte(string s)
        {
            return Convert.ToByte(s);
        }

        public static Decimal toDecimal(string s)
        {
            return Convert.ToDecimal(s);
        }

        public static Double toDouble(string s)
        {
            return Convert.ToDouble(s);
        }

        public static SByte toSByte(string s)
        {
            return Convert.ToSByte(s);
        }

        public static float tofloat(string s)
        {
            try
            {
                float f = (float)Convert.ToSingle(s);
                return f;
            }
            catch (FormatException)
            {
                return (float)0.00;
            }
        }
        #endregion
    }
}
