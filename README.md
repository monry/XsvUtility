# XsvUtility

XsvUtility has serializer and deserializer for CSV, TSV.

You can deserialize any C# class instance from CSV, TSV like as JsonUtility.
Also you can serialize any instance to CSV, TSV.

## Installation

Specify repository URL `git://github.com/monry/upm.XsvUtility.git` with key `tv.monry.xsvutility` into `Packages/manifest.json` like below.

```javascript
{
  "dependencies": {
    // ...
    "tv.monry.xsvutility": "git://github.com/monry/upm.XsvUtility.git",
    // ...
  }
}
```

## Usages

### Setup

You must annotate with C# attributes to serialize/deserialize class, struct.

```csharp
class Foo
{
    [XsvRow]
    public IEnumerable<Bar> Bars { get; set; }
}

class Bar
{
    [XsvColumn(0)]
    public string Baz { get; set; }
    [XsvColumn(1)]
    public int Quz { get; set; }
}
```

### Deserialize

Call `Deserialize<T>()` method to deserialize CSV, TSV to instance of `<T>`.

```csharp
var csvText = @"aaa,100
bbb,200";
var foo = Monry.XsvUtility.CsvSerializer.Deserialize<Foo>(csvText);
foo.Bars.ToList()[0].Baz; // aaa
foo.Bars.ToList()[1].Quz; // 200
```

### Serialize

Call `Serialize<T>()` method to serialize instance of `<T>` to CSV, TSV.

```csharp
var foo = new Foo
{
    Bars = new List<Bar>
    {
        new Bar {Baz = "aaa", Quz = 100},
        new Bar {Baz = "bbb", Quz = 200},
    }
};
var tsvText = Monry.XsvUtility.TsvSerializer.Serialize(foo); // aaa,100\nbbb,200
```

### With header line

You can serialize/deserialize with header line like below.

```csv
name,size
aaa,100
bbb,200
```

Use string parameter constructor of `XsvColumn` instead of int parameter constructor.

```csharp
class Bar
{
    [XsvColumn("name")]
    public string Baz { get; set; }
    [XsvColumn("size")]
    public int Quz { get; set; }
}
```

To deserialize CSV or TSV with header line what to call `DeserializeWithHeader<T>()` method.
Deserializer will detect header column automatically.

To serialize CSV or TSV with header line what to specify `IEnumerable<string>` to 2nd argument when call `Serialize<T>`.

## License

[MIT License](LICENSE.txt)
