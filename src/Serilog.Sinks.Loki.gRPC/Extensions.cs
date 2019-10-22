using System;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Sinks.Loki.gRPC.Labels;

namespace Serilog.Sinks.Loki.gRPC
{
    public static class Extensions
    {
        /// <summary>
        /// Adds Grafana Loki gRPC sink.
        /// </summary>
        /// <param name="sinkLoggerConfiguration"></param>
        /// <param name="grpcEndpoint">host:port for gRPC</param>
        /// <param name="labelProvider">Custom ILogLabelProvider implementation</param>
        /// <param name="restrictedToMinimumLevel">Minimum level for events passing through the sink</param>
        /// <param name="stackTraceAsLabel">Indicates if exception stacktrace should be sent as a label</param>
        /// <returns></returns>
        public static LoggerConfiguration LokigRPC(
            this LoggerSinkConfiguration sinkLoggerConfiguration,
            string grpcEndpoint, ILogLabelProvider labelProvider = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            bool stackTraceAsLabel = false)
        {
            return sinkLoggerConfiguration.Sink(
                new LokigRpcClient(
                    grpcEndpoint,
                    labelProvider,
                    period: TimeSpan.FromSeconds(2),
                    queueLimit: int.MaxValue,
                    stackTraceAsLabel: stackTraceAsLabel
                ), restrictedToMinimumLevel
            );
        }

        /// <summary>
        /// Adds Grafana Loki gRPC sink.
        /// </summary>
        /// <param name="sinkLoggerConfiguration"></param>
        /// <param name="grpcEndpoint">host:port for gRPC</param>
        /// <param name="labelProvider">Custom ILogLabelProvider implementation</param>
        /// <param name="period">The time to wait between checking for event batches. Default value is 2 seconds.</param>
        /// <param name="queueLimit">The maximum number of events stored in the queue in memory, waiting to be posted over the network. Default value is infinitely.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch. Default value is 1000.</param>
        /// <param name="restrictedToMinimumLevel">Minimum level for events passing through the sink</param>
        /// <param name="stackTraceAsLabel">Indicates if exception stacktrace should be sent as a label</param>
        /// <returns></returns>
        public static LoggerConfiguration LokigRPC(
            this LoggerSinkConfiguration sinkLoggerConfiguration,
            string grpcEndpoint,
            ILogLabelProvider labelProvider,
            TimeSpan period,
            int queueLimit,
            int batchSizeLimit,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            bool stackTraceAsLabel = false
        )
        {
            return sinkLoggerConfiguration.Sink(
                new LokigRpcClient(
                    grpcEndpoint,
                    labelProvider,
                    queueLimit,
                    batchSizeLimit,
                    period,
                    stackTraceAsLabel
                ), restrictedToMinimumLevel
            );
        }

        public static string NormalizeLokiLabelValue(this string value)
        {
            return string.Create(value.Length + 2, value, (Span<char> chars, string r) =>
            {
                var span = r.AsSpan();
                var j = 1;

                chars[0] = (char)34;

                foreach (char v in span)
                {
                    chars[j++] = v switch
                    {
                        (char)92 => (char)47,
                        (char)34 => (char)32,
                        (char)123 => (char)40,
                        (char)125 => (char)41,
                        (char)44 => (char)32,
                        (char)10 => (char)32,
                        (char)13 => (char)32,
                        _ => v,
                    };
                }

                chars[j] = (char)34;
            });
        }
    }
}