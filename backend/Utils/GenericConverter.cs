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
        /// gauna List<TSource> kazkoki sarasa
        /// ir gauna funkcija kuri pavercia TSource i TTarget
        /// metodas grazina list nauja
        public List<TTarget> ConvertAll(List<TSource> sourceList, Func<TSource, TTarget> converter)
        {
            if (sourceList == null || converter == null)
                throw new ArgumentNullException("Source list or converter function cannot be null.");

            // selects all items from sourceList and applies converter function to each item
            return sourceList.Select(converter).ToList();
        }

        /// Converts a single source entity to a target DTO using a provided selector function.
        public TTarget ConvertOne(TSource source, Func<TSource, TTarget> converter)
        {
            if (source == null || converter == null)
                throw new ArgumentNullException("Source or converter function cannot be null.");

            return converter(source);
        }
    }
}
