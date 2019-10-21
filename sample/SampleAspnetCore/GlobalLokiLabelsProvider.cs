using System.Collections.Generic;
using Serilog.Sinks.Loki.gRPC.Labels;

namespace SampleAspnetCore
{
    public class GlobalLokiLabelsProvider : ILogLabelProvider
    {
        public IList<LokiLabel> GetLabels()
        {
            return new[]
            {
                new LokiLabel("app", "SampleAspnetCore")
            };
        }
    }
}