using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AgentHub.Entities.Utilities
{
    /// <summary>
    /// ReflectionExtensions class.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Gets properties of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="binding">The binding.</param>
        /// <param name="options">The options.</param>
        /// <returns>
        /// List of properties
        /// </returns>
        public static IEnumerable<PropertyInfo> GetProperties<T>(BindingFlags binding, PropertyReflectionOptions options = PropertyReflectionOptions.All)
        {
            var properties = typeof(T).GetProperties(binding);
            var all = (options & PropertyReflectionOptions.All) != 0;
            var ignoreIndexer = (options & PropertyReflectionOptions.IgnoreIndexer) != 0;

            foreach (var property in properties)
            {
                if (!all)
                {
                    if (ignoreIndexer && IsIndexer(property))
                    {
                        continue;
                    }
                    //
                    // Vinh: Comment these lines to avoid missing fields exception when dealing with uudt types
                    //Ignore for now to accept field candidate.Candidates.cn_photo
                    //if (ignoreIndexer && !(property.PropertyType == typeof(string)) && IsEnumerable(property))
                    //{
                    //    continue;
                    //}
                }

                yield return property;
            }
        }

        /// <summary>
        /// Check if property is indexer
        /// </summary>
        private static bool IsIndexer(PropertyInfo property)
        {
            var parameters = property.GetIndexParameters();

            return parameters.Length > 0;
        }

        /// <summary>
        /// Check if property implements IEnumerable
        /// </summary>
        private static bool IsEnumerable(PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces().Any(x => x == typeof(System.Collections.IEnumerable));
        }
    }

    /// <summary>
    /// The property reflection options
    /// </summary>
    [Flags]
    public enum PropertyReflectionOptions
    {
        /// <summary>
        /// Take all.
        /// </summary>
        All = 0,

        /// <summary>
        /// Ignores indexer properties.
        /// </summary>
        IgnoreIndexer = 1,

        /// <summary>
        /// Ignores all other IEnumerable properties
        /// except strings.
        /// </summary>
        IgnoreEnumerable = 2
    }
}
