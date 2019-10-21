using System.Collections.Generic;

namespace Serilog.Sinks.Loki.gRPC.Labels
{
    public interface ILogLabelProvider
    {
        IList<LokiLabel> GetLabels();
    }
}