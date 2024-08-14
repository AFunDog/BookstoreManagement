using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 书店管理系统.ConsoleApp
{
    internal static class Output
    {
        public static void Info(string messageFormat, params object[]? args)
        {
            System.Console.WriteLine(messageFormat, args);
        }

        public static void Warning(string messageFormat, params object[]? args)
        {
            var orginColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(messageFormat, args);
            System.Console.ForegroundColor = orginColor;
        }

        public static void Error(string messageFormat, params object[]? args)
        {
            var orginColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine(messageFormat, args);
            System.Console.ForegroundColor = orginColor;
        }
    }
}
