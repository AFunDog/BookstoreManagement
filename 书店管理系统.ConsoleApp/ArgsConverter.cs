using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.ConsoleApp
{
    internal static class ArgsConverter
    {
        public static T Convert<T>(string[] args)
        {
            return (T)Convert(args, [typeof(T)])[0];
        }

        public static (T1, T2) Convert<T1, T2>(string[] args)
        {
            var res = Convert(args, [typeof(T1), typeof(T2)]);
            return ((T1)res[0], (T2)res[1]);
        }

        public static (T1, T2, T3) Convert<T1, T2, T3>(string[] args)
        {
            var res = Convert(args, [typeof(T1), typeof(T2), typeof(T3)]);
            return ((T1)res[0], (T2)res[1], (T3)res[2]);
        }

        private static IList<object> Convert(string[] args, Type[] types)
        {
            if (args.Length < types.Length)
                throw new ArgumentException("不匹配的转换参数个数");
            object[] result = new object[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                result[i] = System.Convert.ChangeType(args[i], types[i]);
            }
            return result;
        }
    }
}
