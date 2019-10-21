namespace Serilog.Sinks.Loki.gRPC.Labels
{
    public class LokiLabel
    {

        public LokiLabel(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
        
        public string Key { get; set; }
        
        public string Value { get; set; }
        
    }
}