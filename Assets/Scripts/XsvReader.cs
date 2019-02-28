using System;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Monry.XsvUtility
{
    [Serializable]
    public class XsvReader
    {
        [SerializeField, Tooltip("Choose delimiter, CSV->Comma, TSV->Tab")]
        private XsvParser.Delimiter delimiter;

        [SerializeField, Tooltip("CSV or TSV Asset, Most Prioritable")]
        private TextAsset xsvAsset;

        [SerializeField, Tooltip("CSV or TSV File Path, then XsvAsset is null")]
        private string xsvPathInResources;

        [SerializeField, Tooltip("Header Row Skip Flag")]
        private bool withHeader;

        private XsvParser.Delimiter Delimiter => delimiter;

        private TextAsset XsvAsset =>
            xsvAsset == null && !string.IsNullOrEmpty(XsvPathInResources)
                ? xsvAsset = Resources.Load<TextAsset>(XsvPathInResources)
                : xsvAsset;

        private string XsvPathInResources => xsvPathInResources;

        private bool WithHeader => withHeader;

        public XsvReader(XsvParser.Delimiter delimiter, TextAsset xsvAsset, bool withHeader)
        {
            this.delimiter = delimiter;
            this.xsvAsset = xsvAsset;
            this.withHeader = withHeader;
        }

        public XsvReader(XsvParser.Delimiter delimiter, string xsvPathInResources, bool withHeader)
        {
            this.delimiter = delimiter;
            this.xsvPathInResources = xsvPathInResources;
            this.withHeader = withHeader;
        }

        public struct Data<TValue>
        {
            [XsvRow] public IEnumerable<TValue> Rows { get; [UsedImplicitly] set; }

            [SuppressMessage("ReSharper", "UnusedMember.Global")]
            [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
            public static void AOTWorkaround()
            {
                new Data<TValue>();
                new List<Data<TValue>>();
            }
        }

        /// <summary>
        /// Get rows as a grouped Dictionary whose type is list of TValue
        /// </summary>
        /// <typeparam name="TKey">Type of grouping key defined in TValue</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        /// <returns></returns>
        public IDictionary<TKey, IList<TValue>> GetGroupedValueDictionary<TKey, TValue>()
        {
            return ReadAsGroupedValueDictionary<TKey, TValue>(Delimiter, XsvAsset, WithHeader);
        }

        /// <summary>
        /// Get rows as a grouped List whose type is list of string Dictionary
        /// </summary>
        public IList<IList<IDictionary<string, string>>> GetGroupedStringDictionaryList()
        {
            return ReadAsGroupedStringDictionaryList(Delimiter, XsvAsset, WithHeader);
        }

        /// <summary>
        /// Get rows as a Dictionary whose type is TValue
        /// </summary>
        /// <typeparam name="TKey">Type of grouping key defined in TValue</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        public IDictionary<TKey, TValue> GetValueDictionary<TKey, TValue>(bool useFirst = true)
        {
            return ReadAsValueDictionary<TKey, TValue>(Delimiter, XsvAsset, WithHeader, useFirst);
        }

        /// <summary>
        /// Get rows as a Dictionary whose type is string
        /// </summary>
        public IList<IDictionary<string, string>> GetStringDictionaryList(bool useFirst = true)
        {
            return ReadAsStringDictionaryList(Delimiter, XsvAsset, WithHeader, useFirst);
        }

        /// <summary>
        /// Get rows as a grouped List whose type is list of string
        /// </summary>
        public IList<IList<IList<string>>> GetGroupedStringListList()
        {
            return ReadAsGroupedStringListList(Delimiter, XsvAsset, WithHeader);
        }

        /// <summary>
        /// Get rows as a List of TValue
        /// </summary>
        /// <typeparam name="TValue">Type of value</typeparam>
        public IList<TValue> GetValueList<TValue>()
        {
            return ReadAsValueList<TValue>(Delimiter, XsvAsset, WithHeader);
        }

        /// <summary>
        /// Get rows as a List of List of string
        /// </summary>
        public IList<IList<string>> GetStringListList()
        {
            return ReadAsStringListList(Delimiter, XsvAsset, WithHeader);
        }

        /// <summary>
        /// Get rows as a Dictionary whose type is list of TValue
        /// </summary>
        /// <param name="delimiter">Delimiter (Comma|Tab)</param>
        /// <param name="xsvAsset">TextAsset of xsv file</param>
        /// <param name="withHeader">Use first line as header</param>
        /// <typeparam name="TKey">Type of grouping key defined in TValue</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        public static IDictionary<TKey, IList<TValue>> ReadAsGroupedValueDictionary<TKey, TValue>(XsvParser.Delimiter delimiter, TextAsset xsvAsset, bool withHeader = true)
        {
            return ReadXsv<TValue>(delimiter, xsvAsset, withHeader)
                ?.GroupBy(x => (TKey) GetKeyColumn(x))
                .ToDictionary(
                    x => x.Key,
                    x => x.ToList() as IList<TValue>
                );
        }

        /// <summary>
        /// Get rows as a List whose type is list of string Dictionary
        /// </summary>
        /// <param name="delimiter">Delimiter (Comma|Tab)</param>
        /// <param name="xsvAsset">TextAsset of xsv file</param>
        /// <param name="withHeader">Use first line as header</param>
        public static IList<IList<IDictionary<string, string>>> ReadAsGroupedStringDictionaryList(XsvParser.Delimiter delimiter, TextAsset xsvAsset, bool withHeader = true)
        {
            if (withHeader)
            {
                return XsvParser.ParseWithHeader(delimiter, xsvAsset.text)
                    ?.GroupBy(x => x.Values.First())
                    .Select(x => x.ToList() as IList<IDictionary<string, string>>)
                    .ToList();
            }

            return XsvParser.Parse(delimiter, xsvAsset.text)
                ?.GroupBy(x => x.First())
                .Select(
                    x => x
                        .Select(
                            list => list
                                .Select((item, index) => (item, index))
                                .ToDictionary(y => y.index.ToString(), y => y.item) as IDictionary<string, string>
                        )
                        .ToList() as IList<IDictionary<string, string>>
                )
                .ToList();
        }

        /// <summary>
        /// Get rows as a List whose type is list of string
        /// </summary>
        /// <param name="delimiter">Delimiter (Comma|Tab)</param>
        /// <param name="xsvAsset">TextAsset of xsv file</param>
        /// <param name="withHeader">Use first line as header</param>
        public static IList<IList<IList<string>>> ReadAsGroupedStringListList(XsvParser.Delimiter delimiter, TextAsset xsvAsset, bool withHeader = true)
        {
            if (withHeader)
            {
                return XsvParser.ParseWithHeader(delimiter, xsvAsset.text)
                    ?.GroupBy(x => x.Values.First())
                    .Select(x => x.ToList().Select(y => y.Values.ToList() as IList<string>) as IList<IList<string>>)
                    .ToList();
            }

            return XsvParser.Parse(delimiter, xsvAsset.text)
                ?.GroupBy(x => x.First())
                .Select(
                    x => x
                        .Select(
                            list => list
                                .ToList() as IList<string>
                        )
                        .ToList() as IList<IList<string>>
                )
                .ToList();
        }

        /// <summary>
        /// Get rows as a Dictionary whose type is TValue
        /// </summary>
        /// <param name="delimiter">Delimiter (Comma|Tab)</param>
        /// <param name="xsvAsset">TextAsset of xsv file</param>
        /// <param name="withHeader">Use first line as header</param>
        /// <param name="useFirst">Use first match if multiple hits</param>
        /// <typeparam name="TKey">Type of grouping key defined in TValue</typeparam>
        /// <typeparam name="TValue">Type of value</typeparam>
        public static IDictionary<TKey, TValue> ReadAsValueDictionary<TKey, TValue>(XsvParser.Delimiter delimiter, TextAsset xsvAsset, bool withHeader = true, bool useFirst = true)
        {
            return ReadAsGroupedValueDictionary<TKey, TValue>(delimiter, xsvAsset, withHeader)
                .ToDictionary(
                    x => x.Key,
                    x =>
                        useFirst
                            ? x.Value.First()
                            : x.Value.Last()
                );
        }

        /// <summary>
        /// Get rows as a Dictionary whose type is TValue
        /// </summary>
        /// <param name="delimiter">Delimiter (Comma|Tab)</param>
        /// <param name="xsvAsset">TextAsset of xsv file</param>
        /// <param name="withHeader">Use first line as header</param>
        /// <param name="useFirst">Use first match if multiple hits</param>
        public static IList<IDictionary<string, string>> ReadAsStringDictionaryList(XsvParser.Delimiter delimiter, TextAsset xsvAsset, bool withHeader = true, bool useFirst = true)
        {
            return ReadAsGroupedStringDictionaryList(delimiter, xsvAsset, withHeader)
                .Select(
                    x =>
                        useFirst
                            ? x.First()
                            : x.Last()
                )
                .ToList();
        }

        /// <summary>
        /// Get rows as a List whose type is TValue
        /// </summary>
        /// <param name="delimiter">Delimiter (Comma|Tab)</param>
        /// <param name="xsvAsset">TextAsset of xsv file</param>
        /// <param name="withHeader">Use first line as header</param>
        /// <typeparam name="TValue">Type of value</typeparam>
        public static IList<TValue> ReadAsValueList<TValue>(XsvParser.Delimiter delimiter, TextAsset xsvAsset, bool withHeader = true)
        {
            return ReadXsv<TValue>(delimiter, xsvAsset, withHeader)
                .ToList();
        }

        /// <summary>
        /// Get rows as a List whose type is string
        /// </summary>
        /// <param name="delimiter">Delimiter (Comma|Tab)</param>
        /// <param name="xsvAsset">TextAsset of xsv file</param>
        /// <param name="withHeader">Use first line as header</param>
        public static IList<IList<string>> ReadAsStringListList(XsvParser.Delimiter delimiter, TextAsset xsvAsset, bool withHeader = true)
        {
            var data = XsvParser.Parse(delimiter, xsvAsset.text);
            if (withHeader)
            {
                data.RemoveAt(0);
            }

            return data;
        }

        /// <summary>
        /// カラムを解析して主キーを特定する、XsvKeyプロパティ指定で主キーとなる
        /// Setting of Primary key from XsvKey Attribute Property
        /// </summary>
        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        private static object GetKeyColumn<T>(T instance)
        {
            var type = typeof(T);
            var keyFieldList = type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<XsvKeyAttribute>() != null)
                .ToList();
            if (keyFieldList.Any())
            {
                return keyFieldList.First().GetValue(instance);
            }

            var keyPropertyList = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<XsvKeyAttribute>() != null)
                .ToList();
            if (keyPropertyList.Any())
            {
                return keyPropertyList.First().GetValue(instance);
            }

            return null;
        }

        /// <summary>
        /// XsvDeserializeを使ってTValueの型でパース開始
        /// Run XsvDeserialize with options
        /// </summary>
        /// <typeparam name="TValue">Columns(Struct or Class) Type</typeparam>
        private static IEnumerable<TValue> ReadXsv<TValue>(
            XsvParser.Delimiter delimiter,
            TextAsset xsvAsset,
            bool withHeader = true)
        {
            if (xsvAsset == null)
            {
                return null;
            }

            return
                (
                    withHeader
                        ? InternalSerializer.DeserializeWithHeader<Data<TValue>>(delimiter, xsvAsset.text)
                        : InternalSerializer.Deserialize<Data<TValue>>(delimiter, xsvAsset.text)
                )
                .Rows;
        }
    }
}
