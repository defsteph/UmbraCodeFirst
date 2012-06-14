using System;
using System.Collections.Generic;
using System.Linq;

namespace UmbraCodeFirst.Extensions
{
    /// <summary>
    /// <para>Extension methods for Types.</para>
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// <para>Checks if a specified type is assignable from any of the types in the provided list.</para>
        /// </summary>
        /// <param name="thisType">The type to check.</param>
        /// <param name="listOfTypes">A list of types to check against.</param>
        public static bool IsAssignableFrom(this Type thisType, IEnumerable<Type> listOfTypes)
        {
            return listOfTypes.Any(t => thisType.IsAssignableFrom(t) || t.IsAssignableFrom(thisType));
        }
    }
}
