using Serilog.Events;
using Serilog.Formatting;

namespace LinearUpdateDashboard.Models
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
