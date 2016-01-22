﻿using System;
using System.Diagnostics.CodeAnalysis;
using log4net;

namespace Zipkin.Tracer.SpanCollector
{
    [ExcludeFromCodeCoverage]  //excluded from code coverage since this class is a 1 liner to new up SpanCollector
    public class SpanCollectorBuilder : ISpanCollectorBuilder
    {
        public SpanCollector Build(Uri uri, int maxProcessorBatchSize, ILog logger)
        {
            return SpanCollector.GetInstance(uri, maxProcessorBatchSize, logger);
        }
    }
}
