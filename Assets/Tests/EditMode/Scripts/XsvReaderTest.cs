using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;

namespace Monry.XsvUtility
{
    public class XsvReaderTest
    {
        private struct FixtureColumn
        {
            [XsvColumn(0, "id"), XsvKey] public int Id { get; [UsedImplicitly] set; }
            [XsvColumn(1, "name")] public string Name { get; [UsedImplicitly] set; }
            [XsvColumn(2, "place")] public string Place { get; [UsedImplicitly] set; }
        }

        private const string FixtureCsv = "10,Bob,State of Connecticut\n100,Michael,\"Manhattan Borough\nNew York County\"\n";
        private const string FixtureCsvWithHeader = "id,name,place\n10,Bob,State of Connecticut\n100,Michael,\"Manhattan Borough\nNew York County\"\n";
        private const string FixtureCsvHasSameId = "10,Bob,State of Connecticut\n100,Michael,\"Manhattan Borough\nNew York County\"\n10,Tom,Seattle";
        private const string FixtureCsvHasSameIdWithHeader = "id,name,place\n10,Bob,State of Connecticut\n100,Michael,\"Manhattan Borough\nNew York County\"\n10,Tom,Seattle";

        [Test]
        public void ヘッダなしIntインデックスリストのリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsv),
                        false
                    )
                    .GetStringListList();
            Assert.AreEqual("10", data[0][0]);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[1][2]);
        }

        [Test]
        public void ヘッダありIntインデックスリストのリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvWithHeader),
                        true
                    )
                    .GetStringDictionaryList();
            Assert.AreEqual("10", data[0]["id"]);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[1]["place"]);
        }

        [Test]
        public void ヘッダなしStringインデックス辞書のリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsv),
                        false
                    )
                    .GetStringDictionaryList();
            Assert.AreEqual("10", data[0]["0"]);
            Assert.AreEqual("Michael", data[1]["1"]);
        }

        [Test]
        public void ヘッダありStringインデックス辞書のリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvWithHeader),
                        true
                    )
                    .GetStringDictionaryList();
            Assert.AreEqual("10", data[0]["id"]);
            Assert.AreEqual("Michael", data[1]["name"]);
        }

        [Test]
        public void ヘッダなしリストグループのリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameId),
                        false
                    )
                    .GetGroupedStringListList();
            Assert.AreEqual("10", data[0][0][0]);
            Assert.AreEqual("Michael", data[1][0][1]);
            Assert.AreEqual("Seattle", data[0][1][2]);
        }

        [Test]
        public void ヘッダなしStringインデックス辞書グループのリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameId),
                        false
                    )
                    .GetGroupedStringDictionaryList();
            Assert.AreEqual("10", data[0][0]["0"]);
            Assert.AreEqual("Michael", data[1][0]["1"]);
            Assert.AreEqual("Seattle", data[0][1]["2"]);
        }

        [Test]
        public void ヘッダありStringキー辞書グループのリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameIdWithHeader),
                        true
                    )
                    .GetGroupedStringDictionaryList();
            Assert.AreEqual("10", data[0][0]["id"]);
            Assert.AreEqual("Michael", data[1][0]["name"]);
            Assert.AreEqual("Seattle", data[0][1]["place"]);
        }

        [Test]
        public void ヘッダなし構造体のリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsv),
                        false
                    )
                    .GetValueList<FixtureColumn>();
            Assert.AreEqual(10, data[0].Id);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[1].Place);
        }

        [Test]
        public void ヘッダあり構造体のリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvWithHeader),
                        true
                    )
                    .GetValueList<FixtureColumn>();
            Assert.AreEqual(10, data[0].Id);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[1].Place);
        }

        [Test]
        public void ヘッダなし構造体の辞書_先頭()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameId),
                        false
                    )
                    .GetValueDictionary<int, FixtureColumn>();
            Assert.AreEqual(10, data[10].Id);
            Assert.AreEqual("Bob", data[10].Name);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[100].Place);
        }

        [Test]
        public void ヘッダあり構造体の辞書_先頭()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameIdWithHeader),
                        true
                    )
                    .GetValueDictionary<int, FixtureColumn>();
            Assert.AreEqual(10, data[10].Id);
            Assert.AreEqual("Bob", data[10].Name);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[100].Place);
        }

        [Test]
        public void ヘッダなし構造体の辞書_末尾()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameId),
                        false
                    )
                    .GetValueDictionary<int, FixtureColumn>(false);
            Assert.AreEqual(10, data[10].Id);
            Assert.AreEqual("Tom", data[10].Name);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[100].Place);
        }

        [Test]
        public void ヘッダあり構造体の辞書_末尾()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameIdWithHeader),
                        true
                    )
                    .GetValueDictionary<int, FixtureColumn>(false);
            Assert.AreEqual(10, data[10].Id);
            Assert.AreEqual("Tom", data[10].Name);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[100].Place);
        }

        [Test]
        public void ヘッダなし構造体グループのリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameId),
                        false
                    )
                    .GetGroupedValueDictionary<int, FixtureColumn>();
            Assert.AreEqual(10, data[10][0].Id);
            Assert.AreEqual("Michael", data[100][0].Name);
            Assert.AreEqual("Seattle", data[10][1].Place);
        }

        [Test]
        public void ヘッダあり構造体グループのリスト()
        {
            var data =
                new XsvReader(
                        XsvParser.Delimiter.Comma,
                        new TextAsset(FixtureCsvHasSameIdWithHeader),
                        true
                    )
                    .GetGroupedValueDictionary<int, FixtureColumn>();
            Assert.AreEqual(10, data[10][0].Id);
            Assert.AreEqual("Michael", data[100][0].Name);
            Assert.AreEqual("Seattle", data[10][1].Place);
        }

        [Test]
        public void ヘッダなしIntインデックスリストのリスト_static()
        {
            var data =
                XsvReader.ReadAsStringListList(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsv),
                    false
                );
            Assert.AreEqual("10", data[0][0]);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[1][2]);
        }

        [Test]
        public void ヘッダありIntインデックスリストのリスト_static()
        {
            var data =
                XsvReader.ReadAsStringDictionaryList(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvWithHeader)
                );
            Assert.AreEqual("10", data[0]["id"]);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[1]["place"]);
        }

        [Test]
        public void ヘッダなしStringインデックス辞書のリスト_static()
        {
            var data =
                XsvReader.ReadAsStringDictionaryList(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsv),
                    false
                );
            Assert.AreEqual("10", data[0]["0"]);
            Assert.AreEqual("Michael", data[1]["1"]);
        }

        [Test]
        public void ヘッダありStringインデックス辞書のリスト_static()
        {
            var data =
                XsvReader.ReadAsStringDictionaryList(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvWithHeader)
                );
            Assert.AreEqual("10", data[0]["id"]);
            Assert.AreEqual("Michael", data[1]["name"]);
        }

        [Test]
        public void ヘッダなしリストグループのリスト_static()
        {
            var data =
                XsvReader.ReadAsGroupedStringListList(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameId),
                    false
                );
            Assert.AreEqual("10", data[0][0][0]);
            Assert.AreEqual("Michael", data[1][0][1]);
            Assert.AreEqual("Seattle", data[0][1][2]);
        }

        [Test]
        public void ヘッダなしStringインデックス辞書グループのリスト_static()
        {
            var data =
                XsvReader.ReadAsGroupedStringDictionaryList(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameId),
                    false
                );
            Assert.AreEqual("10", data[0][0]["0"]);
            Assert.AreEqual("Michael", data[1][0]["1"]);
            Assert.AreEqual("Seattle", data[0][1]["2"]);
        }

        [Test]
        public void ヘッダありStringキー辞書グループのリスト_static()
        {
            var data =
                XsvReader.ReadAsGroupedStringDictionaryList(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameIdWithHeader)
                );
            Assert.AreEqual("10", data[0][0]["id"]);
            Assert.AreEqual("Michael", data[1][0]["name"]);
            Assert.AreEqual("Seattle", data[0][1]["place"]);
        }

        [Test]
        public void ヘッダなし構造体のリスト_static()
        {
            var data =
                XsvReader.ReadAsValueList<FixtureColumn>(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsv),
                    false
                );
            Assert.AreEqual(10, data[0].Id);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[1].Place);
        }

        [Test]
        public void ヘッダあり構造体のリスト_static()
        {
            var data =
                XsvReader.ReadAsValueList<FixtureColumn>(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvWithHeader)
                );
            Assert.AreEqual(10, data[0].Id);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[1].Place);
        }

        [Test]
        public void ヘッダなし構造体の辞書_先頭_static()
        {
            var data =
                XsvReader.ReadAsValueDictionary<int, FixtureColumn>(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameId),
                    false
                );
            Assert.AreEqual(10, data[10].Id);
            Assert.AreEqual("Bob", data[10].Name);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[100].Place);
        }

        [Test]
        public void ヘッダあり構造体の辞書_先頭_static()
        {
            var data =
                XsvReader.ReadAsValueDictionary<int, FixtureColumn>(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameIdWithHeader)
                );
            Assert.AreEqual(10, data[10].Id);
            Assert.AreEqual("Bob", data[10].Name);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[100].Place);
        }

        [Test]
        public void ヘッダなし構造体の辞書_末尾_static()
        {
            var data =
                XsvReader.ReadAsValueDictionary<int, FixtureColumn>(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameId),
                    false,
                    false
                );
            Assert.AreEqual(10, data[10].Id);
            Assert.AreEqual("Tom", data[10].Name);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[100].Place);
        }

        [Test]
        public void ヘッダあり構造体の辞書_末尾_static()
        {
            var data =
                XsvReader.ReadAsValueDictionary<int, FixtureColumn>(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameIdWithHeader),
                    true,
                    false
                );
            Assert.AreEqual(10, data[10].Id);
            Assert.AreEqual("Tom", data[10].Name);
            Assert.AreEqual("Manhattan Borough\nNew York County", data[100].Place);
        }

        [Test]
        public void ヘッダなし構造体グループのリスト_static()
        {
            var data =
                XsvReader.ReadAsGroupedValueDictionary<int, FixtureColumn>(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameId),
                    false
                );
            Assert.AreEqual(10, data[10][0].Id);
            Assert.AreEqual("Michael", data[100][0].Name);
            Assert.AreEqual("Seattle", data[10][1].Place);
        }

        [Test]
        public void ヘッダあり構造体グループのリスト_static()
        {
            var data =
                XsvReader.ReadAsGroupedValueDictionary<int, FixtureColumn>(
                    XsvParser.Delimiter.Comma,
                    new TextAsset(FixtureCsvHasSameIdWithHeader)
                );
            Assert.AreEqual(10, data[10][0].Id);
            Assert.AreEqual("Michael", data[100][0].Name);
            Assert.AreEqual("Seattle", data[10][1].Place);
        }
    }
}
