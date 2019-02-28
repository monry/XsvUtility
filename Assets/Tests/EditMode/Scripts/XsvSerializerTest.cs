using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Monry.XsvUtility.Fixture;
using NUnit.Framework;

namespace Monry.XsvUtility
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class XsvSerializerTest
    {
        private const string FixtureTextTsv = "00a49b71a0c32ba7b88b027fd36f6c1a\t937617\n02dad8b60165dd49995b7048a4d6aaf1\t1685691\n04345a169b0367393c937fc50296d22c\t440117\n04ea5eaf96ee42ccd83b9e6a76b93dd0\t27842";
        private const string FixtureTextCsv = "00a49b71a0c32ba7b88b027fd36f6c1a,937617\n02dad8b60165dd49995b7048a4d6aaf1,1685691\n04345a169b0367393c937fc50296d22c,440117\n04ea5eaf96ee42ccd83b9e6a76b93dd0,27842";
        private const string FixtureTextTsvWithHeader = "hash\tsize\n00a49b71a0c32ba7b88b027fd36f6c1a\t937617\n02dad8b60165dd49995b7048a4d6aaf1\t1685691\n04345a169b0367393c937fc50296d22c\t440117\n04ea5eaf96ee42ccd83b9e6a76b93dd0\t27842";
        private const string FixtureTextCsvWithHeader = "hash,size\n00a49b71a0c32ba7b88b027fd36f6c1a,937617\n02dad8b60165dd49995b7048a4d6aaf1,1685691\n04345a169b0367393c937fc50296d22c,440117\n04ea5eaf96ee42ccd83b9e6a76b93dd0,27842";
        private const string FixtureTextTsvWithHeaderSwapped = "size\thash\n937617\t00a49b71a0c32ba7b88b027fd36f6c1a\n1685691\t02dad8b60165dd49995b7048a4d6aaf1\n440117\t04345a169b0367393c937fc50296d22c\n27842\t04ea5eaf96ee42ccd83b9e6a76b93dd0";
        private const string FixtureTextSimpleEnumTsv = "Foo\t10\nbar\t20\n";
        private const string FixtureTextFlagEnumTsv = "Foo,baz\t30\nbar, Quz\t40\n";
        private const string FixtureTextSimpleEnumTsvThrowable = "Foo\t10\nHoge\t20\n";

        private static DataIndexed FixtureInstanceIndexed { get; } = new DataIndexed
        {
            ItemList = new List<ItemIndexed>
            {
                new ItemIndexed {Hash = "00a49b71a0c32ba7b88b027fd36f6c1a", Size = 937617},
                new ItemIndexed {Hash = "02dad8b60165dd49995b7048a4d6aaf1", Size = 1685691},
                new ItemIndexed {Hash = "04345a169b0367393c937fc50296d22c", Size = 440117},
                new ItemIndexed {Hash = "04ea5eaf96ee42ccd83b9e6a76b93dd0", Size = 27842},
            },
        };

        private static DataNamed FixtureInstanceNamed { get; } = new DataNamed
        {
            ItemList = new List<ItemNamed>
            {
                new ItemNamed {Hash = "00a49b71a0c32ba7b88b027fd36f6c1a", Size = 937617},
                new ItemNamed {Hash = "02dad8b60165dd49995b7048a4d6aaf1", Size = 1685691},
                new ItemNamed {Hash = "04345a169b0367393c937fc50296d22c", Size = 440117},
                new ItemNamed {Hash = "04ea5eaf96ee42ccd83b9e6a76b93dd0", Size = 27842},
            },
        };

        private static DataSimpleEnum FixtureInstanceSimpleEnum { get; } = new DataSimpleEnum
        {
            ItemList = new List<ItemSimpleEnum>
            {
                new ItemSimpleEnum {SimpleEnum = SimpleEnum.Foo, Size = 10},
                new ItemSimpleEnum {SimpleEnum = SimpleEnum.Bar, Size = 20},
            }
        };

        private static DataFlagEnum FixtureInstanceFlagEnum { get; } = new DataFlagEnum
        {
            ItemList = new List<ItemFlagEnum>
            {
                new ItemFlagEnum {FlagEnum = FlagEnum.Foo | FlagEnum.Baz, Size = 30},
                new ItemFlagEnum {FlagEnum = FlagEnum.Bar | FlagEnum.Quz, Size = 40},
            }
        };

        [Test]
        public void TSVをDeserialize()
        {
            var data = TsvSerializer.Deserialize<DataIndexed>(FixtureTextTsv);
            Assert.NotNull(data.ItemList);
            Assert.AreEqual(4, data.ItemList.Count());
            Assert.AreEqual("02dad8b60165dd49995b7048a4d6aaf1", data.ItemList.ToList()[1].Hash);
            Assert.AreEqual(27842, data.ItemList.ToList()[3].Size);
        }

        [Test]
        public void CSVをDeserialize()
        {
            var data = CsvSerializer.Deserialize<DataIndexed>(FixtureTextCsv);
            Assert.NotNull(data.ItemList);
            Assert.AreEqual(4, data.ItemList.Count());
            Assert.AreEqual("02dad8b60165dd49995b7048a4d6aaf1", data.ItemList.ToList()[1].Hash);
            Assert.AreEqual(27842, data.ItemList.ToList()[3].Size);
        }

        [Test]
        public void ヘッダ付きTSVをDeserialize()
        {
            var data = TsvSerializer.DeserializeWithHeader<DataNamed>(FixtureTextTsvWithHeader);
            Assert.NotNull(data.ItemList);
            Assert.AreEqual(4, data.ItemList.Count());
            Assert.AreEqual("02dad8b60165dd49995b7048a4d6aaf1", data.ItemList.ToList()[1].Hash);
            Assert.AreEqual(27842, data.ItemList.ToList()[3].Size);
        }

        [Test]
        public void ヘッダ付きCSVをDeserialize()
        {
            var data = CsvSerializer.DeserializeWithHeader<DataNamed>(FixtureTextCsvWithHeader);
            Assert.NotNull(data.ItemList);
            Assert.AreEqual(4, data.ItemList.Count());
            Assert.AreEqual("02dad8b60165dd49995b7048a4d6aaf1", data.ItemList.ToList()[1].Hash);
            Assert.AreEqual(27842, data.ItemList.ToList()[3].Size);
        }

        [Test]
        public void TSVにSerialize()
        {
            Assert.AreEqual(FixtureTextTsv, TsvSerializer.Serialize(FixtureInstanceIndexed));
        }

        [Test]
        public void CSVにSerialize()
        {
            Assert.AreEqual(FixtureTextCsv, CsvSerializer.Serialize(FixtureInstanceIndexed));
        }

        [Test]
        public void ヘッダ付きTSVにSerialize()
        {
            Assert.AreEqual(FixtureTextTsvWithHeader, TsvSerializer.Serialize(FixtureInstanceNamed, new[] {"hash", "size"}));
            Assert.AreEqual(FixtureTextTsvWithHeaderSwapped, TsvSerializer.Serialize(FixtureInstanceNamed, new[] {"size", "hash"}));
        }

        [Test]
        public void ヘッダ付きCSVにSerialize()
        {
            Assert.AreEqual(FixtureTextCsvWithHeader, CsvSerializer.Serialize(FixtureInstanceNamed, new[] {"hash", "size"}));
        }

        [Test]
        public void SimpleEnumを含むTSVをDeserialize()
        {
            var data = TsvSerializer.Deserialize<DataSimpleEnum>(FixtureTextSimpleEnumTsv);
            Assert.NotNull(data);
            Assert.AreEqual(SimpleEnum.Foo, data.ItemList.ToList()[0].SimpleEnum);
            Assert.AreEqual(SimpleEnum.Bar, data.ItemList.ToList()[1].SimpleEnum);
        }

        [Test]
        public void FlagEnumを含むTSVをDeserialize()
        {
            var data = TsvSerializer.Deserialize<DataFlagEnum>(FixtureTextFlagEnumTsv);
            Assert.NotNull(data);
            Assert.AreEqual(FlagEnum.Foo | FlagEnum.Baz, data.ItemList.ToList()[0].FlagEnum);
            Assert.AreEqual(FlagEnum.Bar | FlagEnum.Quz, data.ItemList.ToList()[1].FlagEnum);
        }

        [Test]
        public void 未定義のEnum値を含むTSVをDeserialize()
        {
            Assert.Throws<ArgumentException>(() => TsvSerializer.Deserialize<DataSimpleEnum>(FixtureTextSimpleEnumTsvThrowable));
        }

        [Test]
        public void Enumを用いたSerialize()
        {
            Assert
                .AreEqual(
                    FixtureTextSimpleEnumTsv
                        .ToLower()
                        .TrimEnd('\n')
                        .Replace(" ", string.Empty),
                    TsvSerializer.Serialize(FixtureInstanceSimpleEnum)
                        .ToLower()
                        .TrimEnd('\n')
                        .Replace(" ", string.Empty)
                );
            Assert
                .AreEqual(
                    FixtureTextFlagEnumTsv
                        .ToLower()
                        .TrimEnd('\n')
                        .Replace(" ", string.Empty),
                    TsvSerializer.Serialize(FixtureInstanceFlagEnum)
                        .ToLower()
                        .TrimEnd('\n')
                        .Replace(" ", string.Empty)
                );
        }
    }
}
