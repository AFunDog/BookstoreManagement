﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace 书店管理系统.Core.Structs
{
    public enum ResultType
    {
        Success,
        Warning,
        Error
    }

    public readonly record struct ActionResult(ResultType ResultType, string Message)
    {
        public bool IsSucceed => ResultType == ResultType.Success;

        public static ActionResult Success(string message = "") => new(ResultType.Success, message);

        public static ActionResult Warning(string message) => new(ResultType.Warning, message);

        public static ActionResult Error(string message) => new(ResultType.Error, message);

        public ActionResult Log()
        {
            if (string.IsNullOrEmpty(Message))
                return this;
            switch (ResultType)
            {
                case ResultType.Success:
                    LibrarySystemManager.Logger.Information(Message);
                    break;
                case ResultType.Warning:
                    LibrarySystemManager.Logger.Warning(Message);
                    break;
                case ResultType.Error:
                    LibrarySystemManager.Logger.Error(Message);
                    break;
                default:
                    LibrarySystemManager.Logger.Verbose(Message);
                    break;
            }
            return this;
        }
    }
}
