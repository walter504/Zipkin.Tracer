using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.SpanCollector.Http
{
    public class HttpSpanCollectorConfig
    {
        public const int DEFAULT_QUEUE_SIZE = 1000;
        public const int DEFAULT_BATCH_SIZE = 50;
        public const int DEFAULT_NR_OF_THREADS = 1;
        public const int DEFAULT_CONNECT_TIMEOUT = 10 * 1000;
        public const int DEFAULT_READ_WRITE_TIMEOUT = 60 * 1000;

        private int queueSize;
        private int batchSize;
        private int nrOfThreads;
        private int connectTimeout;
        private int readWriteTimeout;

        public HttpSpanCollectorConfig()
        {
            this.queueSize = DEFAULT_QUEUE_SIZE;
            this.batchSize = DEFAULT_BATCH_SIZE;
            this.nrOfThreads = DEFAULT_NR_OF_THREADS;
            this.connectTimeout = DEFAULT_CONNECT_TIMEOUT;
            this.readWriteTimeout = DEFAULT_READ_WRITE_TIMEOUT;
        }

        /// <summary>
        /// Size of the queue that is used as buffer between producers of spans and the thread(s) that submit the spans to collector.
        /// </summary>
        public int QueueSize
        {
            get
            {
                return queueSize;
            }
            set
            {
                if (queueSize <= 0) throw new ArgumentException("QueueSize must be positive");
                queueSize = value;
            }
        }

        /// <summary>
        /// The batch size is the maximum number of spans that is submitted at once to the collector.
        /// </summary>
        public int BatchSize
        {
            get
            {
                return batchSize;
            }
            set
            {
                if (batchSize <= 0) throw new ArgumentException("BatchSize must be positive");
                batchSize = value;
            }
        }

        /// <summary>
        /// Number of parallel threads for submitting spans to collector.
        /// </summary>
        public int NrOfThreads
        {
            get
            {
                return nrOfThreads;
            }
            set
            {
                if (nrOfThreads <= 0) throw new ArgumentException("NrOfThreads must be positive");
                nrOfThreads = value;
            }
        }

        /// <summary>
        /// Timeout of http connection. (ms)
        /// </summary>
        public int ConnectTimeout
        {
            get
            {
                return connectTimeout;
            }
            set
            {
                if (connectTimeout <= 0) throw new ArgumentException("ConnectTimeout must be positive");
                connectTimeout = value;
            }
        }

        /// <summary>
        /// Timeout of read/write http stream. (ms)
        /// </summary>
        public int ReadWriteTimeout
        {
            get
            {
                return readWriteTimeout;
            }
            set
            {
                if (readWriteTimeout <= 0) throw new ArgumentException("ReadWriteTimeout must be positive");
                readWriteTimeout = value;
            }
        }
    }
}
