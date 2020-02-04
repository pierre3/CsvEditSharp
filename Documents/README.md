# CsvEditSharp

CsvEditSharp is a CSV editor. You can describe reading and writing settings in CSharp script.

![CsvEditSharpMain03](https://github.com/pierre3/CsvEditSharp/blob/master/Documents/csvEditSharp_main03.png)

## Download

- ~~CsvEditSharp is now available in the [Windows Store](https://www.microsoft.com/store/apps/9nblggh4197m).~~
- Download binaries [here](https://github.com/pierre3/CsvEditSharp/releases/) .

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
Leona Lyons,10/3/1959,Female,TRUE,"$1,447"
Randal Bass,9/28/1994,Male,FALSE,"$4,243"
Ben Andrews,1/28/1998,Male,TRUE,"$4,949"
Raul Silva,3/20/1991,Male,FALSE,"$2,685"
Kristy Carlson,12/10/1970,Female,TRUE,$557
Doris Dixon,11/1/1960,Female,FALSE,$56
Jody Williams,9/26/1960,Male,TRUE,"$4,413"
Angelina Rodgers,2/28/1993,Female,FALSE,"$3,992"
Pablo Kelley,6/19/1974,Male,FALSE,"$2,817"
Kristen Greene,8/26/1965,Female,TRUE,"$3,739"
Janie Smith,12/22/1959,Female,FALSE,"$3,120"
Julie Frazier,10/9/1964,Female,FALSE,"$3,958"
Israel Pratt,4/20/1959,Male,FALSE,$418
...
```
- [https://github.com/pierre3/CsvEditSharp/blob/master/Documents/person.csv]


### Configuration script
The configuration script template is automatically generated.



```cs
ICsvEditSharpApi api = GetCsvEditSharpApi();
api.Encoding = Encoding.GetEncoding("utf-8");
api.CsvConfiguration = new CsvConfiguration( CultureInfo.GetCultureInfo("en-US"))
{
	HasHeaderRecord = true
};

class FieldData
{
	public string Name { get; set; }
	public DateTime? Birthday { get; set; }
	public string Gender { get; set; }
	public bool Married { get; set; }
	[NumberStyles(NumberStyles.Any)] 
	public decimal? PocketMoney { get; set; }
}

api.RegisterClassMap<FieldData>();
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
