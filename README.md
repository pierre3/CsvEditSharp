# CsvEditSharp

This is the CSV Editor, describe read/write settings in C# script.

![CsvEditSharpMain03](https://github.com/pierre3/CsvEditSharp/blob/master/Documents/csvEditSharp_main03.png)

## Framework
- Windows Presentation Foundation (WPF) 
- .Net Framework 4.6.1

## Liblary
- [CsvHelper](https://joshclose.github.io/CsvHelper/)
- [AvalonEdit](http://avalonedit.net/)
- [Microsoft.CodeAnalysis.CSharp.Scripting](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Scripting/)

## Exsamples

### sample csv
```
Name,Birthday,Gender,Married,PocketMoney
Joe Maldonado,6/4/1955,Male,N,$7028.74
Mae Ross,10/10/1981,Male,Y,$1847.49
Adele Owen,10/8/1993,Female,N,$2615.28
Nina Pope,7/24/1967,Male,Y,$6265.55
Marian Young,12/3/1969,Male,N,$9165.00
Nicholas Cruz,3/16/1971,Male,Y,$8435.71
Effie Moody,7/23/1990,Female,Y,$6883.24
Larry Carson,11/12/1970,Male,N,$723.88
Edna Watson,3/6/1967,Male,Y,$203.56
Bettie Bishop,9/17/1985,Female,N,$5471.29
Verna Fowler,12/8/1972,Male,N,$43.31
Ola Zimmerman,6/5/1961,Female,Y,$1354.57
Rachel Hart,10/3/1975,Female,Y,$8221.86
...
```

### configuration script

```cs
/* CSV Text Encoding */
Encoding = Encoding.GetEncoding("utf-8");

/* Class Declarations */
class Person
{
    public string Name { get; set; }
    public DateTime Birthday { get; set; }
    public Gender Gender { get; set; }
    public bool Married { get; set; }
    public double PocketMoney { get; set; }
}

enum Gender
{
    Male,
    Female
}

/* Set class map settings via CsvHelper */
RegisterClassMap<Person>(classMap =>
{
    classMap.Map(m => m.Name).Name("Name");
    classMap.Map(m => m.Birthday).Name("Birthday")
        .TypeConverterOption("M/d/yyyy");
    classMap.Map(m => m.Gender).Name("Gender");
    classMap.Map(m => m.Married).Name("Married")
        .TypeConverterOption(true,"Y")
        .TypeConverterOption(false,"N");
    
    var culcure = System.Globalization.CultureInfo.GetCultureInfo("en-us");
    classMap.Map(m => m.PocketMoney).Name("PocketMoney")
        .TypeConverterOption("C")
        .TypeConverterOption(NumberStyles.Currency)
        .TypeConverterOption(culcure);
});
```
![CsvEditSharpMain02](https://github.com/pierre3/CsvEditSharp/blob/master/Documents/csvEditSharp_main02.png)

## Script API

### Encoding Property

```cs
Encoding Encoding { get; set; }
```

Sets a `System.Text.Encoding` object for reading a CSV file.

```cs
Encoding = Encoding.GetEncoding("utf-8");
```

### RegisterClassMap Method

```cs    
void RegisterClassMap<T>();
void RegisterClassMap<T>(Action<CsvClassMap<T>> propertyMapSetter);
void RegisterClassMap<T>(Action<CsvClassMap<T>> propertyMapSetter, RegisterClassMapTarget target);
```

Set class map settings, using `CsvHelper.Configuration.CsvClassMap`.

```cs
/* Set default class map settings */
RegisterClassMap<Person>();

/* Set class map settings for CSV reader and writer */
RegisterClassMap<Person>(classMap => {
    classMap.Map(m => m.Name);
    classMap.Map(m => m.Birthday);
    classMap.Map(m => m.Gender);
    classMap.Map(m => m.Married)
        .TypeConverterOption(true,"Y")
        .TypeConverterOption(false,"N");
    classMap.Map(m => m.PocketMoney)
        .TypeConverterOption("C")
        .TypeConverterOption(NumberStyles.Currency);
});

/* Set class map settings for CSV reader only */
RegisterClassMap<Person>(classMap => {
    classMap.Map(m => m.Name);
    classMap.Map(m => m.Birthday);
    classMap.Map(m => m.Gender);
    classMap.Map(m => m.Married)
        .TypeConverterOption(true,"Y")
        .TypeConverterOption(false,"N");
    classMap.Map(m => m.PocketMoney)
        .TypeConverterOption("C")
        .TypeConverterOption(NumberStyles.Currency);
}, RegisterClassMapTarget.Reader);
```

### SetConfiguration Method

```cs
void SetConfiguration(Action<CsvConfiguration> configurationSetter);
```

Sets a configuration for CSV reader and writer, using `CsvHelper.Configuration.CsvConfiguration`

(sample.csv)

```
Joe Maldonado;6/4/1955;Male;N;$7028.74
# comment line
Mae Ross;10/10/1981;Male;Y;$1847.49
Adele Owen;10/8/1993;Female;N;$2615.28
Nina Pope;7/24/1967;Male;Y;$6265.55
Marian Young;12/3/1969;Male;N;$9165.00
```

```cs
SetConfiguration(config =>
{
    config.HasHeaderRecord = false;
    config.AllowComments = true;
    config.Comment = '#';
    config.Delimiter = ';';
    //etc...
});
```

### AddValidation Method

```cs
void AddValidation<TType, TMember>(Expression<Func<TType, TMember>> memberSelector, Func<TMember, bool> validation, string errorMessage);
```

Sets a validation in column. 

```cs
AddValidation<Person,DateTime>(
    m => m.Birthday , 
    dt => dt <= DateTime.Now.Date,
    "Cannot enter a future date.");

AddValidation<Person, double>(
    m => m.PocketMoney , 
    n => (n > 0) && (n < 10000.0),
    "PocketMoney must be in the range $0 to $10000.");

```

![validation01](https://github.com/pierre3/CsvEditSharp/blob/master/Documents/validation01.png)  
![validation02](https://github.com/pierre3/CsvEditSharp/blob/master/Documents/validation02.png)  

### Query Method

```cs
void Query<T>(Func<IEnumerable<T>, IEnumerable<T>> query);
void Query<T>(Action<IEnumerable<T>> query);
```

#### Filter & Sort Data

```cs
Query<Person>(source => source
    .Where(m => m.Gender == Gender.Female )
    .Where(m => m.Married )
    .OrderBy(m => m.PocketMoney) );
```

![query](https://github.com/pierre3/CsvEditSharp/blob/master/Documents/query.png)  

#### Update Data

```cs
Query<Person>( record => record
	.Where( m => m.Gender == Gender.Male )
	.Where( m => m.Married )
	.ForEach( m =>
	{
		m.Name += " *";
		m.PocketMoney = 0;
	})
);
```

![foreach](https://github.com/pierre3/CsvEditSharp/blob/master/Documents/foreach.png)
