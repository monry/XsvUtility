using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Monry.XsvUtility
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    public class XsvParserTest
    {
        [Test]
        public void シンプルなCSV()
        {
            /*
             * aaa,bbb,ccc
             * ddd,eee,fff
             */
            var rows = XsvParser.Parse(XsvParser.Delimiter.Comma, "aaa,bbb,ccc\nddd,eee,fff");
            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual(3, rows[0].Count);
            Assert.AreEqual(3, rows[1].Count);
            Assert.AreEqual("bbb", rows[0][1]);
            Assert.AreEqual("ccc", rows[0][2]);
            Assert.AreEqual("ddd", rows[1][0]);
            Assert.AreEqual("fff", rows[1][2]);
        }

        [Test]
        public void シンプルなTSV()
        {
            /*
             * aaa<tab>bbb<tab>ccc
             * ddd<tab>eee<tab>fff
             */
            var rows = XsvParser.Parse(XsvParser.Delimiter.Tab, "aaa\tbbb\tccc\nddd\teee\tfff");
            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual(3, rows[0].Count);
            Assert.AreEqual(3, rows[1].Count);
            Assert.AreEqual("bbb", rows[0][1]);
            Assert.AreEqual("ccc", rows[0][2]);
            Assert.AreEqual("ddd", rows[1][0]);
            Assert.AreEqual("fff", rows[1][2]);
        }

        [Test]
        public void 空要素を含むCSV()
        {
            /*
             * aaa,,ccc
             *
             * ,,
             * ,
             *
             */
            var rows = XsvParser.Parse(XsvParser.Delimiter.Comma, "aaa,,ccc\n\n,,\n,\n");
            Assert.AreEqual(4, rows.Count);
            Assert.AreEqual(3, rows[0].Count);
            Assert.AreEqual(1, rows[1].Count);
            Assert.AreEqual(3, rows[2].Count);
            Assert.AreEqual(2, rows[3].Count);
            Assert.AreEqual("aaa", rows[0][0]);
            Assert.AreEqual("", rows[0][1]);
            Assert.AreEqual("", rows[1][0]);
            Assert.AreEqual("", rows[2][0]);
            Assert.AreEqual("", rows[2][2]);
            Assert.AreEqual("", rows[2][0]);
            Assert.AreEqual("", rows[2][1]);
        }

        [Test]
        public void クォーテーションを含むCSV()
        {
            /*
             * aaa,"bbb",ccc
             * ddd,"e""ee",fff
             * g"gg,"hhh""","i"ii"
             */
            // 本来 "i"ii" というカラムは NG だがダブルクォーテーションを無視させることで無理矢理処理させる
            var rows = XsvParser.Parse(XsvParser.Delimiter.Comma, "aaa,\"bbb\",\"\"\"ccc\"\nddd,\"e\"\"ee\",fff\ng\"gg,\"hhh\"\"\",\"i\"ii\"");
            Assert.AreEqual("bbb", rows[0][1]);
            Assert.AreEqual("\"ccc", rows[0][2]);
            Assert.AreEqual("e\"ee", rows[1][1]);
            Assert.AreEqual("g\"gg", rows[2][0]);
            Assert.AreEqual("hhh\"", rows[2][1]);
            Assert.AreEqual("iii", rows[2][2]);
        }

        [Test]
        public void カンマを含むCSV()
        {
            /*
             * aaa,"b,bb",ccc
             * "d"",dd",eee,",fff,"
             */
            var rows = XsvParser.Parse(XsvParser.Delimiter.Comma, "aaa,\"b,bb\",ccc\n\"d\"\",dd\",eee,\",fff,\"");
            Assert.AreEqual("b,bb", rows[0][1]);
            Assert.AreEqual("d\",dd", rows[1][0]);
            Assert.AreEqual(",fff,", rows[1][2]);
        }

        [Test]
        public void 改行を含むCSV()
        {
            /*
             * aaa,"b
             * bb",ccc
             * "
             * ddd",eee,"fff
             * ,"
             */
            var rows = XsvParser.Parse(XsvParser.Delimiter.Comma, "aaa,\"b\nbb\",ccc\n\"\nddd\",eee,\"fff\n\"");
            Assert.AreEqual("b\nbb", rows[0][1]);
            Assert.AreEqual("\nddd", rows[1][0]);
            Assert.AreEqual("fff\n", rows[1][2]);
        }

        [Test]
        public void ヘッダ付きCSV()
        {
            /*
             * aaa,bbb,ccc
             * ddd,eee,fff
             */
            var rows = XsvParser.ParseWithHeader(XsvParser.Delimiter.Comma, "aaa,bbb,ccc\nあああ,いいい,ううう\nかかか,ききき,くくく");
            Assert.AreEqual(2, rows.Count);
            Assert.AreEqual(3, rows[0].Count);
            Assert.AreEqual(3, rows[1].Count);
            Assert.AreEqual("いいい", rows[0]["bbb"]);
            Assert.AreEqual("ううう", rows[0]["ccc"]);
            Assert.AreEqual("かかか", rows[1]["aaa"]);
            Assert.AreEqual("くくく", rows[1]["ccc"]);
        }
    }
}