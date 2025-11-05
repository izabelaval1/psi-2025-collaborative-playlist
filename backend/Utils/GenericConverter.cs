using System;
using System.Collections.Generic;
using System.Linq;

namespace MyApi.Utils
{
    /// A generic converter utility that maps entities to DTOs.
    /// Demonstrates generic type, generic method, and two type constraints.
    public class GenericConverter<TSource, TTarget>
        // kas leidziama naudoti kaip TSource ir TTarget:
        where TSource : class, new()
        where TTarget : class, new()
    {
        /// Converts a list of source entities to target DTOs using a provided selector function.
        public List<TTarget> ConvertAll(List<TSource> sourceList, Func<TSource, TTarget> converter)
        {
            if (sourceList == null || converter == null)
                throw new ArgumentNullException("Source list or converter function cannot be null.");

            return sourceList.Select(converter).ToList();
        }
    }
}
