﻿using System;
using BenchmarkDotNet.Attributes;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Serilog.Sinks.Async.PerformanceTests
{
    public class LatencyBenchmark
    {
        private const int Count = 10000;

        private readonly LogEvent _evt = new LogEvent(DateTimeOffset.Now, LogEventLevel.Information, null,
            new MessageTemplate(new[] {new TextToken("Hello")}), new LogEventProperty[0]);

        private Logger _syncLogger, _asyncLogger;

        [Setup]
        public void Reset()
        {
            _syncLogger?.Dispose();
            _asyncLogger?.Dispose();

            _syncLogger = new LoggerConfiguration()
                .WriteTo.Sink(new SilentSink())
                .CreateLogger();

            _asyncLogger = new LoggerConfiguration()
                .WriteTo.Async(a => a.Sink(new SilentSink()))
                .CreateLogger();
        }

        [Benchmark(Baseline = true)]
        public void Sync()
        {
            for (var i = 0; i < Count; ++i)
            {
                _syncLogger.Write(_evt);
            }
        }

        [Benchmark]
        public void Async()
        {
            for (var i = 0; i < Count; ++i)
            {
                _asyncLogger.Write(_evt);
            }
        }
    }
}