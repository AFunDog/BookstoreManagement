using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using MessagePack.Formatters;

namespace 书店管理系统.Core.Others.MessagePackFormatters
{
    public sealed class CultureInfoFormatter : IMessagePackFormatter<CultureInfo>
    {
        public CultureInfo Deserialize(
            ref MessagePackReader reader,
            MessagePackSerializerOptions options
        )
        {
            var cultureName = reader.ReadString() ?? string.Empty;
            return new CultureInfo(cultureName);
        }

        public void Serialize(
            ref MessagePackWriter writer,
            CultureInfo value,
            MessagePackSerializerOptions options
        )
        {
            writer.Write(value.Name);
        }
    }
}
