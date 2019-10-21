# Serilog.Sinks.Loki.gRpc

This is a [gRPC](https://grpc.io/) Serilog Sink for Grafana's [Loki Log Aggregator](https://grafana.com/loki). 

[![Actions Status](https://github.com/fvoncina/Serilog.Sinks.Loki.gRPC/workflows/Package/badge.svg)](https://github.com/fvoncina/Serilog.Sinks.Loki.gRPC/actions) 

## Installation

The Serilog.Sinks.Loki.gRPC NuGet [package can be found here](https://www.nuget.org/packages/Serilog.Sinks.Loki.gRPC). Alternatively you can install it via one of the following commands below:

NuGet command:
```bash
Install-Package Serilog.Sinks.Loki.gRPC
```
.NET Core CLI:
```bash
dotnet add package Serilog.Sinks.Loki.gRPC
```

## Usage
```csharp
Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.LokigRPC("localhost:9095")
        .CreateLogger();

var exception = new {Message = ex.Message, StackTrace = ex.StackTrace};
Log.Error(exception);

var position = new { Latitude = 25, Longitude = 134 };
var elapsedMs = 34;
Log.Information("Message processed {@Position} in {Elapsed:000} ms.", position, elapsedMs);

Log.CloseAndFlush();
```

## Inspiration
- [Serilog.Sinks.Loki](https://github.com/JosephWoodward/Serilog-Sinks-Loki)

