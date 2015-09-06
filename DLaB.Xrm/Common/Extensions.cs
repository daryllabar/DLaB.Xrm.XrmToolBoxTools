using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using DLaB.Xrm.Common.Exceptions;

namespace DLaB.Xrm.Common
{
    /// <summary>
    /// Container for Extension Methods
    /// </summary>
    public static class Extensions
    {
        #region ConcurrentDictionary<,>

        /// <summary>
        /// Ensures that the value Factory Delegate only gets ran once.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="lockObj">An object to serve as the lock in a "Double-checked" lock if the value needs
        /// to be added to the dictionary</param>
        /// <param name="key">The key.</param>
        /// <param name="valueFactory">The value factory.</param>
        /// <returns></returns>
        public static TElement GetOrAddSafe<TKey, TElement>(
            this ConcurrentDictionary<TKey, TElement> source, object lockObj, TKey key,
                    Func<TKey, TElement> valueFactory)
        {
            TElement value;
            if (!source.TryGetValue(key, out value))
            {
                lock (lockObj)
                {
                    if (!source.TryGetValue(key, out value))
                    {
                        value = valueFactory(key);
                        source.TryAdd(key, value);
                    }
                }
            }
            return value;
        }

        #endregion // ConcurrentDictionary<,>

        #region Dictionary<,>

        /// <summary>
        /// Extension overload of Dictionary.Add to throw a more context specific exception message based on the key
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The key of the element To add.</param>
        /// <param name="value">The value of the element to add.  The value can be null for reference types.</param>
        /// <param name="getDupKeyErrorMessage">Delegate function used to populate the message property of the exception
        /// generated when an element is added to the dictionary whose key already exists.</param>
        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value,
            Func<String> getDupKeyErrorMessage)
        {
            try
            {
                source.Add(key, value);
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "An item with the same key has already been added.")
                {
                    throw new DictionaryDuplicateKeyException(getDupKeyErrorMessage(), ex);
                }
                else
                {
                    throw;
                }
            }
        }

        private static Func<TElement, TElement> Instance<TElement>()
        {
            return delegate(TElement x) { return x; };
        }

        /// <summary>
        /// Performs a try catch on the dictionary for the key, returning the found value, or using the default type of TValue if not found
        /// </summary>
        /// <typeparam name="TKey">Type of the Key in the Dictionary</typeparam>
        /// <typeparam name="TValue">Type of the Value in the Dictionary</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The Key to look for</param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
        {
            return source.GetValue(key, default(TValue));
        }

        /// <summary>
        /// Performs a try catch on the dictionary for the key, returning the found value, or the defaultValue if not found
        /// </summary>
        /// <typeparam name="TKey">Type of the Key in the Dictionary</typeparam>
        /// <typeparam name="TValue">Type of the Value in the Dictionary</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="key">The Key to look for</param>
        /// <param name="defaultValue">The default value to return</param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            TValue value;
            if (source.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        #endregion // Dictionary<,>

        #region Dicitonary<,List<>>

        /// <summary>
        /// If the key is not present in the dictionary list, the value will be added to a new list, then added to the dictionary.
        /// It the key is present in the dictionary list, the value will be appended to the currently existing list.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dict">The dict.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void AddOrAppend<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        {
            List<TValue> values;
            if (!dict.TryGetValue(key, out values))
            {
                values.Add(value);
            }
            else
            {
                values = new List<TValue>();
                values.Add(value);
                dict.Add(key, values);
            }
        }

        #endregion // Dicitonary<,List<>>

        #region IEnumerable<T>

        /// <summary>
        /// Creates a concurrent dictionary from the source.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TKey, TElement>(
            this IEnumerable<TElement> source, Func<TElement, TKey> keySelector)
        {
            return source.ToConcurrentDictionary(keySelector, Instance<TElement>());
        }

        /// <summary>
        /// Creates a concurrent dictionary from the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="elementSelector">The element selector.</param>
        /// <returns></returns>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            ConcurrentDictionary<TKey, TElement> concurrentDictionary = new ConcurrentDictionary<TKey, TElement>();
            foreach (TSource local in source)
            {
                concurrentDictionary.TryAdd(keySelector(local), elementSelector(local));
            }

            return concurrentDictionary;
        }

        /// <summary>
        /// Creates a concurrent dictionary list from the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static ConcurrentDictionary<TKey, List<TSource>> ToConcurrentDictionaryList<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.ToConcurrentDictionaryList(keySelector, Instance<TSource>());
        }

        /// <summary>
        /// Creates a concurrent dictionary list from the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="elementSelector">The element selector.</param>
        /// <returns></returns>
        public static ConcurrentDictionary<TKey, List<TElement>> ToConcurrentDictionaryList<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            ConcurrentDictionary<TKey, List<TElement>> dictionary = new ConcurrentDictionary<TKey, List<TElement>>();
            List<TElement> elements;
            foreach (TSource local in source)
            {
                if (!dictionary.TryGetValue(keySelector(local), out elements))
                {
                    elements = new List<TElement>();
                    dictionary.TryAdd(keySelector(local), elements);
                }
                elements.Add(elementSelector(local));
            }

            return dictionary;
        }

        /// <summary>
        /// Creates a dictionary list from the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static Dictionary<TKey, List<TSource>> ToDictionaryList<TSource, TKey>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.ToDictionaryList(keySelector, Instance<TSource>());
        }

        /// <summary>
        /// Creates a dictionary list from the source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TElement">The type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="elementSelector">The element selector.</param>
        /// <returns></returns>
        public static Dictionary<TKey, List<TElement>> ToDictionaryList<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            Dictionary<TKey, List<TElement>> dictionary = new Dictionary<TKey, List<TElement>>();
            List<TElement> elements;
            foreach (TSource local in source)
            {
                if (!dictionary.TryGetValue(keySelector(local), out elements))
                {
                    elements = new List<TElement>();
                    dictionary.Add(keySelector(local), elements);
                }
                elements.Add(elementSelector(local));
            }

            return dictionary;
        }

        /// <summary>
        /// Creates an object array from the values.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object[] ToObjectArray<T>(this IEnumerable<T> values)
        {
            T[] array = values.ToArray();
            object[] objArray = new object[array.Length];
            array.CopyTo(objArray, 0);
            return objArray;
        }

        #endregion // IEnumerable<T>

        #region String

        #region String.In

        /// <summary>
        /// Returns true if no parameters in the params array are equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">Paramter values to search.</param>
        /// <returns></returns>
        public static bool NotIn(this string value, params Object[] values) { return !value.In(values); }
        /// <summary>
        /// Returns true if a  parameter in the params array is equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">Paramter values to search.</param>
        /// <returns></returns>
        public static bool In(this string value, params Object[] values)
        {
            bool result = false;
            foreach (Object obj in values)
            {
                if (obj.ToString() == value)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns true if no parameters in the params array are equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">Paramter values to search.</param>
        /// <returns></returns>
        public static bool NotIn(this string value, params String[] values) { return !value.In(values); }
        /// <summary>
        /// Returns true if a parameter in the params array is equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">Paramter values to search.</param>
        /// <returns></returns>
        public static bool In(this string value, params String[] values)
        {
            bool result = false;
            foreach (String str in values)
            {
                if (str == value)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns true if no parameters in the params array are equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="values">Paramter values to search.</param>
        /// <returns></returns>
        public static bool NotIn(this string value, StringComparison comparison, params Object[] values) { return !value.In(comparison, values); }
        /// <summary>
        /// Returns true if a parameter in the params array is equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="values">Paramter values to search.</param>
        /// <returns></returns>
        public static bool In(this string value, StringComparison comparison, params Object[] values)
        {
            bool result = false;
            string str;
            foreach (Object obj in values)
            {
                str = obj.ToString();
                if (str.Equals(str, comparison))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns true if no parameters in the params array are equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="values">Paramter values to search.</param>
        /// <returns></returns>
        public static bool NotIn(this string value, StringComparison comparison, params String[] values) { return !value.In(comparison, values); }
        /// <summary>
        /// Returns true if a parameter in the params array is equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="values">Paramter values to search.</param>
        /// <returns></returns>
        public static bool In(this string value, StringComparison comparison, params String[] values)
        {
            bool result = false;
            foreach (String str in values)
            {
                if (str.Equals(str, comparison))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        #endregion // String.In

        /// <summary>
        /// Returns a the substring after the index of the first occurence of the startstring.
        /// Example: "012345678910".SubstringByString("2"); returns "345678910"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startString">The string that marks the start of the substring to be returned.</param>
        /// <returns></returns>
        public static string SubstringByString(this string value, string startString)
        {
            int start = value.IndexOf(startString);
            if (start < 0) { return null; }
            return value.Substring(start + startString.Length);
        }

        /// <summary>
        /// Returns a the substring after the index of the first occurence of the startstring and ending before the first instance of the end string.
        /// Example: "012345678910".SubstringByString("2", "8"); returns "34567"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startString">The string that marks the start of the substring to be returned.</param>
        /// <param name="endString">The string that marks the end of the substring to be returned.</param>
        /// <returns></returns>
        public static string SubstringByString(this string value, string startString, string endString)
        {
            int start = value.IndexOf(startString);
            if (start < 0) { return null; }
            return SubstringByString(value, value.IndexOf(startString) + startString.Length, endString);
        }

        /// <summary>
        /// Returns a the substring starting with the index of the startIndex and ending before the first instance of the end string.
        /// Example: "012345678910".SubstringByString("2", "8"); returns "34567"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index of the substring.</param>
        /// <param name="endString">The string that marks the end of the substring to be returned.</param>
        /// <returns></returns>
        public static string SubstringByString(this string value, int startIndex, string endString)
        {
            int end = value.IndexOf(endString, startIndex);
            if (end < 0) { return value.Substring(startIndex); }

            return value.Substring(startIndex, end - startIndex);
        }

        #endregion // String
    }
}
