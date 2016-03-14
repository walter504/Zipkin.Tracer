using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.SpanCollector.Http
{
    public class HttpSpanCollector : ISpanCollector, IDisposable
    {
        private static ILog logger = log4net.LogManager.GetLogger(typeof(HttpSpanCollector));

        private readonly BlockingCollection<Span> queue;
        private readonly List<SpanProcessor> processors = new List<SpanProcessor>();
        private readonly List<Task<int>> tasks = new List<Task<int>>();
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        public HttpSpanCollector(string baseUrl)
            : this(baseUrl, new HttpSpanCollectorConfig())
        {
        }

        public HttpSpanCollector(string baseUrl, HttpSpanCollectorConfig config)
        {
            queue = new BlockingCollection<Span>(config.QueueSize);
            for (int i = 1; i <= config.NrOfThreads; i++)
            {
                var spanProcessor = new SpanProcessor(baseUrl, queue, config);
                processors.Add(spanProcessor);
                tasks.Add(Task.Factory.StartNew<int>(spanProcessor.Execute, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default));
            }
        }

        public void Collect(Span span)
        {
            long start = DateTime.Now.Ticks;
            if (queue.TryAdd(span))
            {
                long end = DateTime.Now.Ticks;
                if (logger.IsDebugEnabled)
                {
                    logger.Debug("Adding span to queue took " + (end - start) / 10000D + "ms.");
                }
            }
            else
            {
                logger.Warn("Queue rejected Span, span not submitted: " + span);
            }
        }

        public void Dispose()
        {
            logger.Info("Stopping SpanProcessor.");
            foreach (var processor in processors)
            {
                processor.Stop();
            }
            foreach (var task in tasks)
            {
                try
                {
                    int spansProcessed = task.Result;
                    logger.Info("SpanProcessor processed " + spansProcessed + "spans.");
                }
                catch (Exception e) {
                    logger.Error("Exception when getting result of SpanProcessor.", e);
                }
            }
            cts.Cancel();
            logger.Info("HttpSpanCollector closed.");
        }
    }
}
