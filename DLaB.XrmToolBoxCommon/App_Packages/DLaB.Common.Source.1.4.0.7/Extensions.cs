using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

#if DLAB_UNROOT_COMMON_NAMESPACE
using DLaB.Common.Exceptions;

namespace DLaB.Common
#else

using Source.DLaB.Common.Exceptions;

namespace Source.DLaB.Common
#endif
{
    /// <summary>
    /// Extension Class
    /// </summary>
#if DLAB_PUBLIC
    public static class Extensions
#else
    internal static class Extensions
#endif
    {
        #region Byte[]

        /// <summary>
        /// Unzips the specified zipped bytes using an in-memory GZipStream.
        /// </summary>
        /// <param name="zippedBytes">The zipped bytes to unzip.</param>
        /// <param name="encoding">The Encoding to use to parse the bytes.  Defaults to UTF8.</param>
        /// <returns></returns>
        public static string Unzip(this byte[] zippedBytes, Encoding encoding = null)
        {
            using (var zipped = new MemoryStream(zippedBytes))
            using (var unzipper = new System.IO.Compression.GZipStream(zipped, System.IO.Compression.CompressionMode.Decompress))
            using (var unzippedBytes = new MemoryStream())
            {
                unzipper.CopyTo(unzippedBytes);

                encoding = encoding ?? DLaBConfig.Config.GetEncoding(EncodingUses.Zip, zippedBytes);
                return encoding.GetString(unzippedBytes.ToArray());
            }
        }

        /// <summary>
        /// Zips the specified bytes using an in-memory GZipStream.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static byte[] Zip(this byte[] bytes)
        {
            byte[] compressed;
            using (var ms = new MemoryStream())
            {
                using (var zip = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true))
                {
                    zip.Write(bytes, 0, bytes.Length);
                }
                ms.Position = 0;

                compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);
            }

            return compressed;
        }

        #endregion Byte[]

        #region ConcurrentDictionary<,>

        /// <summary>
        /// Creates a concurrent dictionary from the source.
        /// </summary>
        /// <typeparam name="TKey">The Type of the key.</typeparam>
        /// <typeparam name="TElement">The Type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TKey, TElement>(
            this IEnumerable<TElement> source, Func<TElement, TKey> keySelector)
        {
            return source.ToConcurrentDictionary(keySelector, Instance<TElement>());
        }

        /// <summary>
        /// Creates a concurrent dictionary list from the source.
        /// </summary>
        /// <typeparam name="TSource">The Type of the source.</typeparam>
        /// <typeparam name="TKey">The Type of the key.</typeparam>
        /// <typeparam name="TElement">The Type of the element.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <param name="elementSelector">The element selector.</param>
        /// <returns></returns>
        public static ConcurrentDictionary<TKey, TElement> ToConcurrentDictionary<TSource, TKey, TElement>(
            this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            var concurrentDictionary = new ConcurrentDictionary<TKey, TElement>();
            foreach (var local in source)
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
            var dictionary = new ConcurrentDictionary<TKey, List<TElement>>();
            foreach (var local in source)
            {
                if (!dictionary.TryGetValue(keySelector(local), out var elements))
                {
                    elements = new List<TElement>();
                    dictionary.TryAdd(keySelector(local), elements);
                }
                elements.Add(elementSelector(local));
            }

            return dictionary;
        }

        /// <summary>
        /// Ensures that the value Factory Delegate only gets ran once.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <param name="source"></param>
        /// <param name="lockObj">An object to serve as the lock in a "Double-checked" lock if the value needs
        /// to be added to the dictionary</param>
        /// <param name="key"></param>
        /// <param name="valueFactory"></param>
        /// <returns></returns>
        public static TElement GetOrAddSafe<TKey, TElement>(
            this ConcurrentDictionary<TKey, TElement> source, object lockObj, TKey key,
                    Func<TKey, TElement> valueFactory)
        {
            if (!source.TryGetValue(key, out var value))
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

        #endregion ConcurrentDictionary<,>

        #region ConcurrentQueue<T>

        /// <summary>
        /// Adds the range of items to the end of the queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="range"></param>
        public static void EnqueueRange<T>(this ConcurrentQueue<T> queue, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                queue.Enqueue(item);
            }
        }

        #endregion ConcurrentQueue

        #region DateTime

        /// <summary>
        /// recreates a new instance of the given date time, specified to be UTC
        /// </summary>
        public static DateTime AsUtc(this DateTime date)
        {
            return date.Kind == DateTimeKind.Utc ?
                date :
                new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond, DateTimeKind.Utc);
        }

        /// <summary>
        /// recreates a new instance of the given date time, specified to be UTC
        /// </summary>
        public static DateTime? AsUtc(this DateTime? date)
        {
            return date?.AsUtc();
        }

        /// <summary>
        /// Inclusive IsBetween Check, using DefaultValue for Null
        /// </summary>
        public static bool IsBetween(this DateTime date, DateTime? start, DateTime? end)
        {
            return start.GetValueOrDefault() <= date && date <= end.GetValueOrDefault();
        }

        #endregion Date Time

        #region Dictionary<,>

        /// <summary>
        /// Extension overload of Dictionary.Add to throw a more context specific exception message based on the key
        /// </summary>
        /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
        /// <param name="source"></param>
        /// <param name="key">The key of the element To add.</param>
        /// <param name="value">The value of the element to add.  The value can be null for reference types.</param>
        /// <param name="getDupKeyErrorMessage">Delegate function used to populate the message property of the exception
        /// generated when an element is added to the dictionary whose key already exists.</param>
        public static void Add<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value,
            Func<string> getDupKeyErrorMessage)
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

                throw;
            }
        }

        private static Func<TElement, TElement> Instance<TElement>()
        {
            return x => x;
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
            var dictionary = new Dictionary<TKey, List<TElement>>();
            foreach (var local in source)
            {
                if (!dictionary.TryGetValue(keySelector(local), out var elements))
                {
                    elements = new List<TElement>();
                    dictionary.Add(keySelector(local), elements);
                }
                elements.Add(elementSelector(local));
            }

            return dictionary;
        }

        /// <summary>
        /// Performs a try catch on the dictionary for the key, returning the found value, or the defaultValue if not found
        /// </summary>
        /// <typeparam name="TKey">Type of the Key in the Dictionary</typeparam>
        /// <typeparam name="TValue">Type of the Value in the Dictionary</typeparam>
        /// <param name="source"></param>
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
        /// <param name="source"></param>
        /// <param name="key">The Key to look for</param>
        /// <param name="defaultValue">The default value to return</param>
        /// <returns></returns>
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            return source.TryGetValue(key, out var value) ? value : defaultValue;
        }

        #endregion Dictionary<,>

        #region Dictionary<, HastSet<>>
        /// <summary>
        /// Looks up the list for the given key, adding the value if the list is found, or creating a new list and adding
        /// the value to that list if the list is not found.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key">The key value to lookup and add the value to the list of</param>
        /// <param name="value">Value to add to the list</param>
        public static void AddOrAppend<TKey, TValue>(this Dictionary<TKey, HashSet<TValue>> dict, TKey key, TValue value)
        {
            if (dict.TryGetValue(key, out var values))
            {
                values.Add(value);
            }
            else
            {
                values = new HashSet<TValue>
                    {
                        value
                    };
                dict.Add(key, values);
            }
        }

        /// <summary>
        /// Looks up the list for the given key, adding the value if the list is found, or creating a new list and adding
        /// the value to that list if the list is not found.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key">The key value to lookup and add the value to the list of</param>
        /// <param name="value">Values to add to the list</param>
        public static void AddOrAppend<TKey, TValue>(this Dictionary<TKey, HashSet<TValue>> dict, TKey key, params TValue[] value)
        {
            if (dict.TryGetValue(key, out var values))
            {
                value.ToList().ForEach(g => values.Add(g));
            }
            else
            {
                values = new HashSet<TValue>();
                value.ToList().ForEach(g => values.Add(g));
                dict.Add(key, values);
            }
        }
        #endregion

        #region Dicitonary<,List<>>

        /// <summary>
        /// Looks up the list for the given key, adding the value if the list is found, or creating a new list and adding
        /// the value to that list if the list is not found.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key">The key value to lookup and add the value to the list of</param>
        /// <param name="value">Value to add to the list</param>
        public static void AddOrAppend<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        {
            if (dict.TryGetValue(key, out var values))
            {
                values.Add(value);
            }
            else
            {
                values = new List<TValue> {value};
                dict.Add(key, values);
            }
        }

        /// <summary>
        /// Looks up the list for the given key, adding the value if the list is found, or creating a new list and adding
        /// the value to that list if the list is not found.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key">The key value to lookup and add the value to the list of</param>
        /// <param name="value">Values to add to the list</param>
        public static void AddOrAppend<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, params TValue[] value)
        {
            if (dict.TryGetValue(key, out var values))
            {
                values.AddRange(value);
            }
            else
            {
                values = new List<TValue>();
                values.AddRange(value);
                dict.Add(key, values);
            }
        }

        #endregion Dicitonary<,List<>>

        #region Exception

        /// <summary>
        /// Checks the ToString results of the Exception and adds the stack trace if it isn't there
        /// </summary>
        /// <returns></returns>
        public static string ToStringWithCallStack(this Exception ex)
        {
            // This is not 100% but the worse that happens is a duplicate stack trace.

            var s = ex.ToString();
            if (s.Length >= ex.Message.Length && s.Substring(ex.Message.Length).Contains("   at "))
            {
                return s;
            }

            if (ex.InnerException != null)
            {
                s = string.Format("{0} ---> {1}{2}   --- End of inner exception stack trace ---{2}", s, ex.InnerException.ToStringWithCallStack(), Environment.NewLine);
            }
            
            var stackTrace = ex.StackTrace;
            if (stackTrace != null)
            {
                s += Environment.NewLine + stackTrace;
            }
            
            return s;
        }

        #endregion Exception

        #region Expression<Func<TEntity,TProperty>>

        /// <summary>
        /// Gets the name of the lower case property.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="exp">The exp.</param>
        /// <returns></returns>
        public static string GetLowerCasePropertyName<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> exp)
        {
            return ((MemberExpression)exp.Body).Member.Name.ToLower();
        }

        #endregion Expression<Func<T,TProperty>>

        #region ICollection

        /// <summary>
        /// Equivalent to !collection.Contains().  Purely for readability, especially if you have a negative collection ie. 
        /// if(status != null &amp;&amp; !notToCalcStatuses.Contains(status)) vs if(status != null &amp;&amp; notToCalcStatuses.DoesNotContain(status))
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool DoesNotContain<T>(this ICollection<T> collection, T value)
        {
            return !collection.Contains(value);
        }

        #endregion ICollection

        #region IEnumerable<string>

        /// <summary>
        /// Joins the items in the list to create a csv using string.Join(", ", items)
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static string ToCsv(this IEnumerable<string> items)
        {
            return string.Join(", ", items);
        }

        #endregion IEnumerable<string>

        #region IEnumerable<T>

        /// <summary>
        /// Converts an IEnumerable into Batches
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="batchSize">Size of the batch.</param>
        /// <returns></returns>
        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> collection, int batchSize)
        {
            var nextBatch = new List<T>(batchSize);
            foreach (var item in collection)
            {
                nextBatch.Add(item);
                if (nextBatch.Count != batchSize) continue;

                yield return nextBatch;
                nextBatch = new List<T>(batchSize);
            }

            if (nextBatch.Count > 0)
                yield return nextBatch;
        }

        /// <summary>
        /// Allows a Key Selector to be used to create a HashSet and perform a Distinct without creating an Equality Comparer
        /// See http://stackoverflow.com/questions/1300088/distinct-with-lambda
        /// 
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="source"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        /// <summary>
        /// Executes the given action against every item in the IEnumerable.  Any Exceptions are grouped by Exception Type
        /// and the first exception of each type is written to the StringBuilder along with the number of exceptions of that type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Items to iterate over performing the action on</param>
        /// <param name="sb"></param>
        /// <param name="action">Action to perform on each item</param>
        /// <returns></returns>
        public static bool IterateAndTrackExceptions<T>(this IEnumerable<T> items, StringBuilder sb, Action<T> action)
        {
            var total = 0;
            var errors = new Dictionary<string, Tuple<Exception, int>>();
            foreach (var item in items)
            {
                try
                {
                    action(item);
                }
                catch (Exception ex)
                {
                    var key = ex.GetType().FullName ?? "";
                    if (errors.TryGetValue(key, out var exceptionWithCount))
                    {
                        errors[key] = new Tuple<Exception, int>(exceptionWithCount.Item1, exceptionWithCount.Item2 + 1);
                    }
                    else
                    {
                        errors.Add(key, new Tuple<Exception, int>(ex, 1));
                    }
                }
                finally { total++; }
            }

            // Handle Exceptions
            if (errors.Count == 0)
            {
                sb.AppendLogLine("{0} {1} were processed.  0 exceptions occurred.{2}", total, typeof(T).Name);
            }
            else if (errors.Count == 1)
            {
                sb.AppendLogLine("{0} {1} were processed.  1 type of exception occurred.{2}", total, typeof(T).Name);
            }
            else
            {
                sb.AppendLogLine("{0} {1} were processed.  {2} types of exceptions occurred.{3}", total, typeof(T).Name, errors.Count);
            }

            foreach (var error in errors)
            {
                if (error.Value.Item2 == 1)
                {
                    sb.AppendLogLine("    1 exception of type {0} occurred: {2}{1}", error.Key, error.Value.Item1);
                }
                else
                {
                    sb.AppendLogLine("    {0} exceptions of type {1} occurred.  The first exception of this type was: {3}{2}", error.Value.Item2, error.Key, error.Value.Item1);
                }
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Executes the given action against every item in the IEnumerable.  Any Exceptions are grouped by Exception Type
        /// and the first exception of each type is written to the StringBuilder along with the number of exceptions of that type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">Items to iterate over performing the action on</param>
        /// <param name="action">Action to perform on each item</param>
        /// <param name="exceptionHandler">Contains the item that caused the exception, and the exception</param>
        /// <returns></returns>
        public static bool IterateAndDelegateExceptionHandling<T>(this IEnumerable<T> items, Action<T> action, Action<T, Exception> exceptionHandler)
        {
            var errored = false;
            foreach (var item in items)
            {
                try
                {
                    action(item);
                }
                catch (Exception ex)
                {
                    exceptionHandler(item, ex);
                    errored = true;
                }
            }

            return errored;
        }

        /// <summary>
        /// Creates an object array from the values.
        /// </summary>
        /// <typeparam name="T">The type of the array</typeparam>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static object[] ToObjectArray<T>(this IEnumerable<T> values)
        {
            var array = values.ToArray();
            var objArray = new object[array.Length];
            array.CopyTo(objArray, 0);
            return objArray;
        }

        #endregion IEnumerable<T>

        #region IEnumerable<string>

        /// <summary>
        /// Batches the values into batches with the maximum length less than the max.  Useful when executing a command line that can only be a certain length, but there are a large number of arguments to potentially adds.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="maxLength">The maximum length, with the optional additional padding for each value.</param>
        /// <param name="errorFormat">The error format {0} is the value, {1} is the Max Length.</param>
        /// <param name="padding">The padding.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static List<List<string>> BatchLessThanMaxLength(this IEnumerable<string> values, int maxLength, string errorFormat = "Value \"{0}\" is longer than than max length {1}.", int padding = 0)
        {
            var batches = new List<List<string>>();
            var length = 0;
            var batch = new List<string>();
            foreach (var value in values)
            {
                if (value.Length + length + padding > maxLength)
                {
                    if (length == 0)
                    {
                        throw new Exception(string.Format(errorFormat, value, maxLength));
                    }

                    batches.Add(batch);
                    batch = new List<string>();
                    length = 0;
                }
                batch.Add(value);
                length += value.Length + padding;
            }

            if (batch.Count > 0)
            {
                batches.Add(batch);
            }

            return batches;
        }

        #endregion IEnumerable<string>

        #region IEquatable<T>

        /// <summary>
        /// Checks whether the current value is in the list of values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static bool In<T>(this T value, params T[] values) where T : IEquatable<T>
        {
            return values.Contains(value);
        }

        /// <summary>
        /// Checks whether the current value is in the list of values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static bool In<T>(this T value, IEnumerable<T> values) where T : IEquatable<T>
        {
            return values.Contains(value);
        }

        #endregion IEquatable<T>

        #region IExtensibleDataObject

#if NETCOREAPP == false
        /// <summary>
        /// Serializes the specified obj, returning it's xml serialized value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public static string Serialize(this IExtensibleDataObject obj, bool indent = false)
        {
            var serializer = new NetDataContractSerializer();
            StringWriter sr = null;
            try
            {
                sr = new StringWriter(CultureInfo.InvariantCulture);
                using (var writer = new System.Xml.XmlTextWriter(sr))
                {
                    if (indent)
                    {
                        writer.Formatting = System.Xml.Formatting.Indented;
                        writer.Indentation = 2;
                    }

                    serializer.WriteObject(writer, obj);
                    return sr.ToString();
                }
            }
            finally
            {
                sr?.Dispose();
            }
        }
#endif
        #endregion IExtensibleDataObject

        #region KeyValueConfigurationCollection

        /// <summary>
        /// Converts the KeyValueConfigurationCollection to a NameValueCollection
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static NameValueCollection ToNameValueCollection(this KeyValueConfigurationCollection collection)
        {
            var nvc = new NameValueCollection();
            foreach (KeyValueConfigurationElement element in collection)
            {
                nvc.Set(element.Key, element.Value);
            }

            return nvc;
        }

        #endregion KeyValueConfigurationCollection

        #region MemberInfo

        /// <summary>
        /// Determines whether the Member Info contains all the specified custom attribute types.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public static bool ContainsCustomAttributeTypes(this MemberInfo property, params Type[] attributes)
        {
            if (attributes.Length == 0) { return true; }
            if (property.CustomAttributes.Count() < attributes.Length) { return false;}

            var notFoundAttributes = new HashSet<Type>(attributes);
            foreach (var attributeType in property.CustomAttributes.Select(a => a.AttributeType))
            {
                if (notFoundAttributes.Contains(attributeType))
                {
                    notFoundAttributes.Remove(attributeType);
                }

                if (notFoundAttributes.Count == 0)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion MemberInfo

        #region Object

        /// <summary>
        /// Shortcut for throwing an ArgumentNullException
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        [DebuggerHidden]
        public static void ThrowIfNull(this object data, string name)
        {
            if (data == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        #endregion Object

        #region Queue<T>

        /// <summary>
        /// Adds the range of items to the end of the queue.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queue"></param>
        /// <param name="range"></param>
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                queue.Enqueue(item);
            }
        }

        #endregion Queue

        #region String

        /// <summary>
        /// Determines whether current string contains the specified value.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns></returns>
        public static bool Contains(this string source, string value, StringComparison comparisonType)
        {
            return source.IndexOf(value, comparisonType) >= 0;
        }

        /// <summary>
        /// Determines whether current string contains the specified value, ignoring case.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Deserializes the json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">The text.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="encoding">The encoding.  Defaults to Encoding.UTF8.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public static T DeserializeJson<T>(this string text, DataContractJsonSerializerSettings settings = null, Encoding encoding = null)
        {
            if(text == null){
                throw new ArgumentNullException(nameof(text));
            }

            encoding = encoding ?? DLaBConfig.Config.GetEncoding(EncodingUses.Json, text);
            settings = settings ?? DLaBConfig.Config.GetJsonSerializerSettings(text, encoding);
            using(var reader = new MemoryStream(encoding.GetBytes(text)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T), settings);
                return (T)serializer.ReadObject(reader);
            }
        }

        /// <summary>
        /// The standard string.GetHashCode can result in different domains.  This is a deterministic version of the hash code so it will always return the same int value for the given string.
        /// Warning: it's not safe to use in any situations vulnerable to hash-based attacks!
        /// </summary>
        /// <param name="str"></param>
        /// <remarks>Taken from https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/#a-deterministic-gethashcode-implementation</remarks>
        /// <returns></returns>
        public static int GetDeterministicHashCode(this string str)
        {
            unchecked
            {
                var hash1 = (5381 << 16) + 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
        }

        /// <summary>
        /// Converts a string that is a base64 (UTF8 by default) encoded string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="encoding">Default to UTF8</param>
        /// <returns></returns>
        public static string FromBase64(this string text, Encoding encoding = null)
        {
            if (text == null)
            {
                return null;
            }

            encoding = encoding ?? DLaBConfig.Config.GetEncoding(EncodingUses.Base64, text);
            text = encoding.GetString(Convert.FromBase64String(text));
            var preamble = encoding.GetString(encoding.GetPreamble());
            if (text.StartsWith(preamble))
            {
                text = text.Remove(0, preamble.Length);
            }

            return text;
        }


        /// <summary>
        /// Limits the string to the specified maximum length.  All characters beyond the max are removed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns></returns>
        public static string Limit(this string source, int maxLength)
        {
            return source.Substring(0, Math.Min(source.Length, maxLength));
        }

        /// <summary>
        /// Parses or Convert the string into the give "T" type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strValue">The string value.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException"></exception>
        public static T ParseOrConvertString<T>(this string strValue)
        {
            T value;
            var type = typeof(T);
            if (type.IsEnum)
            {
                // Handle Enums by parsing as integers, and then casting to the enum type
                return (T)(object)strValue.ParseOrConvertString<int>();
            }

            var parse = type.GetMethod("Parse", new[] { typeof(string) });
            try
            {
                if (parse == null)
                {
                    value = (T)Convert.ChangeType(strValue, type);
                }
                else
                {
                    value = (T)parse.Invoke(null, new object[] { strValue });
                }
            }
            catch (Exception ex)
            {
                throw new FormatException($"Unable to convert \"{strValue}\" into type {typeof(T).FullName}.", ex);
            }
            return value;
        }

        /// <summary>
        /// Converts String to a Single Item List
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static List<string> ToSingleItemList(this string source)
        {
            return new List<string> { source };
        }

        #region String.In

        /// <summary>
        /// Returns true if no parameters in the params array are equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">Parameter values to search.</param>
        /// <returns></returns>
        public static bool NotIn(this string value, params object[] values) { return !value.In(values); }

        /// <summary>
        /// Returns true if a  parameter in the params array is equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">Parameter values to search.</param>
        /// <returns></returns>
        public static bool In(this string value, params object[] values)
        {
            return values.Any(obj => obj.ToString() == value);
        }

        /// <summary>
        /// Returns true if no parameters in the params array are equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">Parameter values to search.</param>
        /// <returns></returns>
        public static bool NotIn(this string value, params string[] values) { return !value.In(values); }
        /// <summary>
        /// Returns true if a parameter in the params array is equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">Parameter values to search.</param>
        /// <returns></returns>
        public static bool In(this string value, params string[] values)
        {
            return values.Any(str => str == value);
        }

        /// <summary>
        /// Returns true if no parameters in the params array are equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="values">Parameter values to search.</param>
        /// <returns></returns>
        public static bool NotIn(this string value, StringComparison comparison, params object[] values)
        {
            return !value.In(comparison, values);
        }

        /// <summary>
        /// Returns true if a parameter in the params array is equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="values">Parameter values to search.</param>
        /// <returns></returns>
        public static bool In(this string value, StringComparison comparison, params object[] values)
        {
            return values.Select(obj => obj.ToString()).Any(str => value.Equals(str, comparison));
        }

        /// <summary>
        /// Returns true if no parameters in the params array are equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="values">Parameter values to search.</param>
        /// <returns></returns>
        public static bool NotIn(this string value, StringComparison comparison, params string[] values) { return !value.In(comparison, values); }

        /// <summary>
        /// Returns true if a parameter in the params array is equal to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="values">Parameter values to search.</param>
        /// <returns></returns>
        public static bool In(this string value, StringComparison comparison, params string[] values)
        {
            return values.Any(str => value.Equals(str, comparison));
        }

        #endregion String.In

        /// <summary>
        /// Nullable int parse
        /// </summary>
        /// <param name="stringInt">The string int.</param>
        /// <returns></returns>
        public static int? ParseInt(this string stringInt)
        {
            int? value = null;
            if (stringInt != null && int.TryParse(stringInt, out var i))
            {
                value = i;
            }

            return value;
        }

        #region SubstringByString

        /// <summary>
        /// Returns a the substring after the index of the first occurence of the startString.
        /// Example: "012345678910".SubstringByString("2"); returns "345678910"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startString">The string that marks the start of the substring to be returned.</param>
        /// <param name="comparison">The comparison method for finding the index of the endString.</param>
        /// <returns></returns>
        public static string SubstringByString(this string value, string startString, StringComparison comparison = StringComparison.Ordinal)
        {
            var start = value.IndexOf(startString, comparison);
            return start < 0 ? null : value.Substring(start + startString.Length);
        }

        /// <summary>
        /// Returns a the substring after the index of the first occurence of the startString and ending before the first instance of the end string.
        /// Example: "012345678910".SubstringByString("2", "8"); returns "34567"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startString">The string that marks the start of the substring to be returned.</param>
        /// <param name="endString">The string that marks the end of the substring to be returned.</param>
        /// <param name="comparison">The comparison method for finding the index of the endString.</param>
        /// <returns></returns>
        public static string SubstringByString(this string value, string startString, string endString, StringComparison comparison = StringComparison.Ordinal)
        {
            return value.SubstringByString(startString, endString, out _, comparison);
        }

        /// <summary>
        /// Returns a the substring after the index of the first occurence of the startString and ending before the first instance of the endString.
        /// Example: "012345678910".SubstringByString("2", "8"); returns "34567"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startString">The string that marks the start of the substring to be returned.</param>
        /// <param name="endString">The string that marks the end of the substring to be returned.</param>
        /// <param name="endIndex">The end index of the endString.  Returns -1 if endString is not found.</param>
        /// <param name="comparison">The comparison method for finding the index of the endString.</param>
        /// <returns></returns>
        public static string SubstringByString(this string value, string startString, string endString, out int endIndex, StringComparison comparison = StringComparison.Ordinal)
        {
            var start = value.IndexOf(startString, comparison);
            string result;
            if (start < 0)
            {
                endIndex = -1;
                result = null;
            }
            else
            {
                result = value.SubstringByString(start + startString.Length, endString, out endIndex);
            }
            return result;
        }

        /// <summary>
        /// Returns a the substring starting with the index of the startIndex and ending before the first instance of the end string.
        /// Example: "012345678910".SubstringByString("2", "8"); returns "34567"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index of the substring.</param>
        /// <param name="endString">The string that marks the end of the substring to be returned.</param>
        /// <param name="comparison">The comparison method for finding the index of the endString.</param>
        /// <returns></returns>
        public static string SubstringByString(this string value, int startIndex, string endString, StringComparison comparison = StringComparison.Ordinal)
        {
            return value.SubstringByString(startIndex, endString, out _, comparison);
        }

        /// <summary>
        /// Returns a the substring starting with the index of the startIndex and ending before the first instance of the end string.
        /// Example: "012345678910".SubstringByString("2", "8"); returns "34567"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startIndex">The start index of the substring.</param>
        /// <param name="endString">The string that marks the end of the substring to be returned.</param>
        /// <param name="endIndex">The end index of the endString.  Returns -1 if endString is not found.</param>
        /// <param name="comparison">The comparison method for finding the index of the endString.</param>
        /// <returns></returns>
        public static string SubstringByString(this string value, int startIndex, string endString, out int endIndex, StringComparison comparison = StringComparison.Ordinal)
        {
            var end = value.IndexOf(endString, startIndex, comparison);
            string result;
            if (end < 0)
            {
                endIndex = -1;
                result = null;
            }
            else
            {

                endIndex = end;
                result = value.Substring(startIndex, end - startIndex);
            }
            return result;
        }

        #endregion SubstringByString

        #region SubstringAllByString

        /// <summary>
        /// Loops through the string, retrieving sub strings for the values.  i.e. "_1_2_".SubstringAllByString("_","_") would return a list containing two items, "1" and "2"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="startString">The start string.</param>
        /// <param name="endString">The end string.</param>
        /// <param name="comparison">The comparison.</param>
        /// <param name="splitOptions">The split options.</param>
        /// <returns></returns>
        public static List<string> SubstringAllByString(this string value, string startString, string endString, StringComparison comparison = StringComparison.Ordinal, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            var results = new List<string>();

            while (true)
            {
                var sub = value.SubstringByString(startString, endString, out var index, comparison);
                if (index < 0)
                {
                    break;
                }
                if (!string.IsNullOrEmpty(sub) || splitOptions != StringSplitOptions.RemoveEmptyEntries)
                {
                    results.Add(sub);
                }
                value = value.Substring(index);
            }

            return results;
        }

        #endregion SubstringAllByString

        /// <summary>
        /// Inserts spaces before capital letters.  
        /// If more than two capital letters are in sequence, only the first letter will have a space pre appended
        /// examples:
        /// HelloWorld --> Hello World
        /// HelloWorldXML --> Hello World XML
        /// HelloAWorld --> Hello A World
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SpaceOutCamelCase(this string value)
        {
            // http://stackoverflow.com/questions/272633/add-spaces-before-capital-letters
            return System.Text.RegularExpressions.Regex.Replace(value,
                @"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))|((?<=[\p{Ll}\p{Lu}])\p{Nd})|((?<=\p{Nd})\p{Lu})", " $0");
        }

        /// <summary>
        /// Converts a string to a base64 (UTF8 by default) encoded string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="encoding">Default to UTF8</param>
        /// <param name="includePreamble">if set to <c>true</c> [include preamble] else don't.</param>
        /// <returns></returns>
        public static string ToBase64(this string text, Encoding encoding = null, bool includePreamble = false)
        {
            if (text == null)
            {
                return null;
            }

            encoding = encoding ?? DLaBConfig.Config.GetEncoding(EncodingUses.Base64, text);
            if (includePreamble)
            {
                text = encoding.GetString(encoding.GetPreamble()) + text;
            }

            return Convert.ToBase64String(encoding.GetBytes(text));
        }

        /// <summary>
        /// Zips the specified text using an in-memory GZipStream.
        /// </summary>
        /// <param name="text">The text to be Zipped.</param>
        /// <param name="encoding">The Encoding to be used.  Defaults to ASCII</param>
        /// <returns></returns>
        public static byte[] Zip(this string text, Encoding encoding = null)
        {
            encoding = encoding ?? DLaBConfig.Config.GetEncoding(EncodingUses.Zip, text);
            return encoding.GetBytes(text).Zip();
        }

        #endregion String

        #region StringBuilder

        /// <summary>
        /// Adds the current time "HH:MM:ss.fff - "  to the message, and appends a newline at the end
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="message"></param>
        public static void AppendLogLine(this StringBuilder sb, string message)
        {
            sb.AppendLine(DateTime.Now.ToString("HH:mm:ss.fff") + " - " + message);
        }

        /// <summary>
        /// Adds the current time "HH:MM:ss.fff - "  to the message, and appends a newline at the end.
        /// Also calls String.Format on the message and appends an Environment.NewLine to the end of the args array
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void AppendLogLine(this StringBuilder sb, string message, params object[] args)
        {
            var newArgs = new object[args.Length + 1];
            args.CopyTo(newArgs, 0);
            newArgs[args.Length] = Environment.NewLine;
            sb.AppendLogLine(string.Format(message, newArgs));
        }

            #endregion StringBuilder
    
        #region Type

        /// <summary>
        /// Gets the class level attribute based on type.
        /// </summary>
        /// <remarks>Taken from https://stackoverflow.com/questions/2656189/how-do-i-read-an-attribute-on-a-class-at-runtime </remarks>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TAttribute GetClassAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
        }

        /// <summary>
        /// Searches the current assembly for the first (defaults to public) class that implements the interface
        /// </summary>
        /// <param name="interfaceType">The Interface Type.</param>
        /// <param name="assembly">The Assembly to search, defaults to the Assembly of the given type.</param>
        /// <param name="forceIsPublic">If true only searches public Types.</param>
        /// <returns></returns>
        public static Type GetFirstImplementation(this Type interfaceType, Assembly assembly = null, bool forceIsPublic = true)
        {
            return (assembly ?? interfaceType.Assembly).ExportedTypes.FirstOrDefault(t => t.IsClass
                                                                                          && (!forceIsPublic || t.IsPublic)
                                                                                          && interfaceType.IsAssignableFrom(t));
        }

        #endregion Type

        #region <T>

        /// <summary>
        /// Serializes the value to a json string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The Value to serialize to JSON</param>
        /// <param name="settings">The settings.</param>
        /// <param name="encoding">The encoding.  Defaults to Encoding.UTF8</param>
        /// <param name="formatJson">Formats the outputted JSON</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public static string SerializeToJson<T>(this T value, DataContractJsonSerializerSettings settings = null, Encoding encoding = null, bool formatJson = false)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            encoding = encoding ?? DLaBConfig.Config.GetEncoding(EncodingUses.Json, value);
            settings = settings ?? DLaBConfig.Config.GetJsonSerializerSettings(value, encoding);

            using (var memoryStream = new MemoryStream())
            {
                if (formatJson)
                {
                    using (var writer = JsonReaderWriterFactory.CreateJsonWriter(memoryStream, encoding, true, true, "  "))
                    {
                        var serializer = new DataContractJsonSerializer(typeof(T), settings);
                        serializer.WriteObject(writer, value);
                        writer.Flush();
                        return encoding.GetString(memoryStream.ToArray());
                    }
                }

                new DataContractJsonSerializer(typeof(T), settings).WriteObject(memoryStream, value);
                return encoding.GetString(memoryStream.ToArray());
            }
        }

        #endregion <T>

    }
}
