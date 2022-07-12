using Serilog.Events;
using Serilog.Formatting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinearWebService.Models
{
    public class LogEventsFormatModel : ITextFormatter
    {
        public static LogEventsFormatModel Formatter { get; } = new LogEventsFormatModel();

        public void Format(LogEvent logEvent, TextWriter output)
        {
            logEvent.Properties.ToList()
                .ForEach(e => output.Write($"{e.Key}={e.Value} "));
        }
    }
}
