using System;
using System.Collections.Generic;
using System.Linq;
using Zipkin.Tracer.Core;

namespace Zipkin.Tracer.Http
{
    public class HttpServerRequestAdapter : IServerRequestAdapter
    {
        private readonly IHttpServerRequest serverRequest;
        private readonly ISpanNameProvider spanNameProvider;

        public HttpServerRequestAdapter(IHttpServerRequest serverRequest, ISpanNameProvider spanNameProvider)
        {
            this.serverRequest = serverRequest;
            this.spanNameProvider = spanNameProvider;
        }

        public TraceData GetTraceData()
        {
            string sampled = serverRequest.GetHttpHeaderValue(ZipkinHttpHeaders.Sampled);
            if (sampled != null)
            {
                if (sampled == "0" || sampled.ToLower() == "false")
                {
                    return new TraceData() { Sample = false };
                }
                else
                {
                    string parentSpanId = serverRequest.GetHttpHeaderValue(ZipkinHttpHeaders.ParentSpanId);
                    string traceId = serverRequest.GetHttpHeaderValue(ZipkinHttpHeaders.TraceId);
                    string spanId = serverRequest.GetHttpHeaderValue(ZipkinHttpHeaders.SpanId);

                    if (traceId != null && spanId != null)
                    {
                        return new TraceData()
                        {
                            Sample = true,
                            SpanId = GetSpanId(traceId, spanId, parentSpanId)
                        };
                    }
                }
            }
            return new TraceData();
        }

        public string GetSpanName()
        {
            return spanNameProvider.SpanName(serverRequest);
        }

        public IEnumerable<KeyValueAnnotation> RequestAnnotations()
        {
            return Enumerable.Empty<KeyValueAnnotation>();
        }

        private SpanId GetSpanId(string traceId, string spanId, string parentSpanId)
        {
            if (parentSpanId != null)
            {
                return new SpanId(Util.HexToLong(traceId), Util.HexToLong(spanId), Util.HexToLong(parentSpanId));
            }
            return new SpanId(Util.HexToLong(traceId), Util.HexToLong(spanId), null);
        }
    }
}
