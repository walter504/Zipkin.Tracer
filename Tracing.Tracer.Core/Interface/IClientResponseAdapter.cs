using System;
using System.Collections.ObjectModel;

namespace Tracing.Tracer.Core
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
        Collection<KeyValueAnnotation> ResponseAnnotations { get; set; }
    }
}
