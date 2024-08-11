using System;
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

        public ActionResult TryReportResult(string executeInfo, IProgress<ActionResult>? progress = null, ILogger? logger = null)
        {
            const string messageTemplate = "{executeInfo} : {@resultType} {message}";
            progress?.Report(this);
            switch (this.ResultType)
            {
                case ResultType.Success:
                    logger?.Information(messageTemplate, executeInfo, this.ResultType, this.Message);
                    break;
                case ResultType.Warning:
                    logger?.Warning(messageTemplate, executeInfo, this.ResultType, this.Message);
                    break;
                case ResultType.Error:
                    logger?.Error(messageTemplate, executeInfo, this.ResultType, this.Message);
                    break;
                default:
                    break;
            }
            return this;
        }

        public static ActionResult Success(string message = "") => new(ResultType.Success, message);

        public static ActionResult Warning(string message) => new(ResultType.Warning, message);

        public static ActionResult Error(string message) => new(ResultType.Error, message);
    }
}
