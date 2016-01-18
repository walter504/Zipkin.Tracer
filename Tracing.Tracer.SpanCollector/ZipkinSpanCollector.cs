using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tracing.Tracer.Core;

namespace Tracing.Tracer.SpanCollector
{
    public class ZipkinSpanCollector : ISpanCollector, IDisposable
    {
        private const int MAX_QUEUE_SIZE = 100;
        internal static readonly BlockingCollection<Span> spanQueue;
        internal SpanProcessor spanProcessor;
        private static SpanCollector instance;

        //private readonly ExecutorService executorService;
        //private readonly List<SpanProcessingThread> spanProcessingThreads = new ArrayList<>();
        //private readonly List<ScribeClientProvider> clientProviders = new ArrayList<>();
        //private readonly List<Future<Integer>> futures = new ArrayList<>();
        //private readonly Set<BinaryAnnotation> defaultAnnotations = new HashSet<>();
        //private readonly SpanCollectorMetricsHandler metricsHandler;

        public ZipkinSpanCollector()
        {
            
        }

        public ZipkinSpanCollector(Uri uri, int maxProcessorBatchSize)
        {
            if ( spanQueue == null)
            {
                spanQueue = new BlockingCollection<Span>(MAX_QUEUE_SIZE);
            }

            spanProcessor = new SpanProcessor(uri, spanQueue, maxProcessorBatchSize, logger);
        }

        public void Dispose()
        {

        }
    }
}
