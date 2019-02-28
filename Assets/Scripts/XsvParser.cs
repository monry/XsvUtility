using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Monry.XsvUtility
{
    public static class XsvParser
    {
        public enum Delimiter
        {
            Comma,
            Tab,
        }

        private const string Comma = ",";
        private const string Tab = "\t";
        private const string LineFeed = "\n";
        private const string CarriageReturn = "\r";
        private const string Quotation = "\"";
        private static IDictionary<Delimiter, string> DelimiterMap { get; } = new Dictionary<Delimiter,string>
        {
            {Delimiter.Comma, Comma},
            {Delimiter.Tab, Tab},
        };

        public static IList<IList<string>> Parse(Delimiter delimiter, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new List<IList<string>>();
            }
            var textEnumerator = StringInfo.GetTextElementEnumerator(text);
            var rows = new List<IList<string>>();
            var columns = new List<string>();
            var column = new StringBuilder();
            var isQuoting = false;

            textEnumerator.MoveNext();
            while (true)
            {
                var current = textEnumerator.GetTextElement();
                var isSpecialCharacter = false;

                // クォーテーション中の場合
                if (isQuoting)
                {
                    // クォーテーションの場合は次の文字をチェックする
                    if (current == Quotation)
                    {
                        if (textEnumerator.MoveNext())
                        {
                            var tmp = textEnumerator.GetTextElement();
                            if (tmp == Quotation)
                            {
                                column.Append(Quotation);
                                isSpecialCharacter = true;
                            }
                            else
                            {
                                if (tmp == LineFeed || tmp == CarriageReturn || tmp == DelimiterMap[delimiter])
                                {
                                    isQuoting = false;
                                }
                                current = tmp;
                            }
                        }
                        else
                        {
                            isQuoting = false;
                            isSpecialCharacter = true;
                        }
                    }
                }

                // クォーテーション中じゃない場合は特殊文字をチェック
                if (!isQuoting)
                {
                    switch (current)
                    {
                        // クォーテーションでありカラム1文字目の場合はクォーテーションと見なす
                        case Quotation when column.Length == 0:
                            isQuoting = true;
                            isSpecialCharacter = true;
                            break;
                        // \n の場合は新規行を作る
                        case LineFeed:
                            columns.Add(column.ToString());
                            rows.Add(columns);
                            column.Clear();
                            columns = new List<string>();
                            isSpecialCharacter = true;
                            break;
                        // \r の場合は次の文字に判断を委ねる
                        case CarriageReturn:
                            isSpecialCharacter = true;
                            break;
                        default:
                            // デリミタの場合はカラム文字列を構築
                            if (current == DelimiterMap[delimiter])
                            {
                                columns.Add(column.ToString());
                                column.Clear();
                                isSpecialCharacter = true;
                            }
                            break;
                    }
                }

                // 特殊文字では無い場合はカラム文字列に追加
                if (!isSpecialCharacter)
                {
                    column.Append(current);
                }

                // 次の文字が存在しない場合は終了
                if (!textEnumerator.MoveNext())
                {
                    break;
                }
            }

            if (column.Length > 0)
            {
                columns.Add(column.ToString());
            }
            if (columns.Any())
            {
                rows.Add(columns);
            }

            return rows;
        }

        public static IList<IDictionary<string, string>> ParseWithHeader(Delimiter delimiter, string text)
        {
            var rows = Parse(delimiter, text);
            if (rows == null || rows.Count == 0)
            {
                return new List<IDictionary<string, string>>();
            }
            var result = new List<IDictionary<string, string>>();
            var header = rows[0];
            var index = 0;
            foreach (var row in rows)
            {
                if (index++ == 0)
                {
                    continue;
                }

                var line = new Dictionary<string, string>();
                for (var i = 0; i < header.Count; i++)
                {
                    line[header[i]] = row[i];
                }
                result.Add(line);
            }

            return result;
        }

        public static string Compose(Delimiter delimiter, IEnumerable<IEnumerable<string>> data, IEnumerable<string> header = null)
        {
            var result = string.Join(LineFeed, data.Select(line => ComposeRow(delimiter, line)));
            if (header != null)
            {
                result = $"{string.Join(DelimiterMap[delimiter], header)}{LineFeed}{result}";
            }
            return result;
        }

        private static string ComposeRow(Delimiter delimiter, IEnumerable<string> data)
        {
            return string.Join(DelimiterMap[delimiter], data.Select(x => ComposeColumn(delimiter, x)));
        }

        private static string ComposeColumn(Delimiter delimiter, string value)
        {
            value = value.Replace(Quotation, $"{Quotation}{Quotation}");
            if (value.Contains(DelimiterMap[delimiter]) || value.Contains(LineFeed) || value.Contains(CarriageReturn) || value.Contains(Quotation))
            {
                value = $"{Quotation}{value}{Quotation}";
            }

            return value;
        }
    }
}