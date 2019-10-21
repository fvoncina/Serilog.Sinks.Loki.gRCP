using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Logproto;
using Serilog.Events;
using Serilog.Sinks.Loki.gRPC.Labels;
using Serilog.Sinks.PeriodicBatching;

namespace Serilog.Sinks.Loki.gRPC
{
    internal class LokigRpcClient : PeriodicBatchingSink
    {
        private readonly Pusher.PusherClient _client;
        private readonly ILogLabelProvider _globalLabelsProvider;

        public LokigRpcClient(
            string grpcEndpoint,
            ILogLabelProvider globalLabelsProvider = null,
            int? queueLimit = 2,
            int? batchSizeLimit = 1000,
            TimeSpan? period = null
        ) : this(grpcEndpoint, globalLabelsProvider, queueLimit ?? int.MaxValue, batchSizeLimit ?? 1000,
            period ?? TimeSpan.FromSeconds(2))
        {
        }

        public LokigRpcClient(
            string grpcEndpoint,
            ILogLabelProvider globalLabelsProvider,
            int queueLimit,
            int batchSizeLimit,
            TimeSpan period
        ) : base(batchSizeLimit, period, queueLimit)
        {
            if (string.IsNullOrEmpty(grpcEndpoint))
            {
                throw new ArgumentNullException(nameof(grpcEndpoint));
            }

            _globalLabelsProvider = globalLabelsProvider;
            _client = new Pusher.PusherClient(new Channel(grpcEndpoint, ChannelCredentials.Insecure));
        }

        protected override async Task EmitBatchAsync(IEnumerable<LogEvent> events)
        {
            var req = new PushRequest();
            var logEvents = events as LogEvent[] ?? events.ToArray();
            foreach (var le in logEvents)
            {
                var stream = new Stream();
                req.Streams.Add(stream);
                stream.Labels = GenerateLabels(le);
                stream.Entries.Add(new Entry
                {
                    Line = le.RenderMessage(),
                    Timestamp = new Google.Protobuf.WellKnownTypes.Timestamp
                        {Seconds = le.Timestamp.ToUnixTimeSeconds()}
                });
            }

            await _client.PushAsync(req);
        }

        private string GenerateLabels(LogEvent le)
        {
            var list = new List<string> {$@"level={le.Level.ToString().ToLower().NormalizeLokiLabelValue()}"};
            if (_globalLabelsProvider != null)
            {
                list.AddRange(_globalLabelsProvider.GetLabels()
                    .Select(label => $@"{label.Key}={label.Value.NormalizeLokiLabelValue()}"));
            }

            if (le.Exception != null)
            {
                list.Add($"ExceptionType={le.Exception.GetType().Name.NormalizeLokiLabelValue()}");
                if (le.Exception.Data?.Count != 0)
                {
                    foreach (var key in le.Exception.Data.Keys)
                    {
                        var value = le.Exception.Data[key];
                        if (value != null)
                        {
                            list.Add($"Exception_Data_{key}={value.ToString().NormalizeLokiLabelValue()}");
                        }
                    }
                }
            }

            list.AddRange(le.Properties.Select(x => $@"{x.Key}={x.Value.ToString().NormalizeLokiLabelValue()}"));
            return $"{{{string.Join(",", list)}}}";
        }
    }
}