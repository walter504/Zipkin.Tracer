using System;
using System.Collections.Generic;

namespace Zipkin.Tracer.Core
{
    public interface IClientResponseAdapter
    {

        /**
         * Returns a collection of annotations that should be added to span
         * based on response.
         *
         * Can be used to indicate errors when response was not successful.
         *
         * @return Collection of annotations.
         */
        IEnumerable<KeyValueAnnotation> ResponseAnnotations();
    }
}
