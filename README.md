# CsvEditSharp

CsvEditSharp is a CSV editor that describes read / write settings with C # script.

In the script, write read / write settings using API of "CsvHelper" which is a open source .Net class library.

![Window000](/Documents/Capture000.PNG)

## Framework
- Windows Presentation Foundation (WPF) 
- .Net Framework 4.7.2

## Libraries
- [CsvHelper](https://joshclose.github.io/CsvHelper/)
- [AvalonEdit](http://avalonedit.net/)
- [Microsoft.CodeAnalysis.CSharp.Scripting](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Scripting/)


# Download

- ~~CsvEditSharp is now available in the [Windows Store](https://www.microsoft.com/store/apps/9nblggh4197m).~~
- Download binaries [here](https://github.com/pierre3/CsvEditSharp/releases/) .


# Overview

## sample csv

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

- https://github.com/pierre3/CsvEditSharp/blob/master/Documents/person.csv


## Configuration script

### Automatically generated

In the CSV file to be read for the first time, a script is automatically generated from the column name in the header and the first line of data.

- Enter the information required for automatic generation.

![Window002](/Documents/Capture002.PNG)

- The following configuration script is generated.
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

- The CSV file is read and displayed according to the contents of the generated script.

![Window003](/Documents/Capture004.PNG)

## Script API
Configure using the ICsvEditSharpApi interface in the script.

```cs
public interface ICsvEditSharpApi
{
    Encoding Encoding { get; set; }
    CsvConfiguration CsvConfiguration { get; set; }
    IDictionary<string, ColumnValidation> ColumnValidations { get; }
    ICsvEditSharpApi GetCsvEditSharpApi();
    void RegisterClassMap<T>(Action<ClassMap<T>> propertyMapSetter = null);
    void RegisterClassMapForReading<T>(Action<ClassMap<T>>　propertyMapSetter = null);
    void RegisterClassMapForWriting<T>(Action<ClassMap<T>> propertyMapSetter = null);
    void AddValidation<TType, TMember>(Expression<Func<TType, TMember>> memberSelector,
        Func<TMember, bool> validation, string errorMessage);
    void Query<T>(Func<IEnumerable<T>, IEnumerable<T>> query);
    void Query<T>(Action<IEnumerable<T>> query);
}
```
### GetCsvEditSharpApi Method

```cs
ICsvEditSharpApi GetCsvEditSharpApi();
```
Gets the CSV Edit Sharp API object.

```cs
ICsvEditSharpApi api = GetCsvEditSharpApi();
```

### Encoding Property

```cs
Encoding Encoding { get; set; }
```

Sets a `System.Text.Encoding` object for reading a CSV file.

```cs
api.Encoding = Encoding.GetEncoding("utf-8");
```

### CsvConfiguration Property

```cs
CsvConfiguration CsvConfiguration { get; set; }
```
Sets a configuration for CSV reader and writer, using `CsvHelper.Configuration.CsvConfiguration`.

```cs
api.CsvConfiguration = new CsvConfiguration( CultureInfo.GetCultureInfo("en-US"))
{
    HasHeaderRecord = false,
    AllowComments = true,
    Comment = '#',
    Delimiter = '\t'
    //etc...
};
```

### RegisterClassMap Methods

```cs    
void RegisterClassMap<T>(Action<ClassMap<T>> propertyMapSetter = null);
void RegisterClassMapForReading<T>(Action<ClassMap<T>>　propertyMapSetter = null);
void RegisterClassMapForWriting<T>(Action<ClassMap<T>> propertyMapSetter = null);
```

Set class map settings, using `CsvHelper.CsvConfiguration.CsvClassMap`.

```cs
/* Set the class map of FieldData for reading and writing, and Set the class map of auto mapping  */
api.RegisterClassMap<FieldData>();

/* Set the class map of FieldData for reading */
api.RegisterClassMapForReading<FieldData>(classMap =>
{
    classMap.AutoMap(api.CsvConfiguration);
    classMap.Map(m => m.PocketMoney)
        .TypeConverterOption
		.NumberStyles(NumberStyles.Currency);
});

/* Set the class map of FieldData for writing */
api.RegisterClassMapForWriting<FieldData>(classMap => 
{
    classMap.AutoMap(api.CsvConfiguration);
    classMap.Map(m => m.PocketMoney)
        .TypeConverterOption
		.Format("C");
});

```

### AddValidation Method

```cs
void AddValidation<TType, TMember>(Expression<Func<TType, TMember>> memberSelector,
    Func<TMember, bool> validation, string errorMessage);
```

Sets a validation in column. 

```cs
api.AddValidation<FieldData,DateTime>(
    m => m.Birthday , 
    dt => dt <= DateTime.Now.Date,
    "Cannot enter a future date.");

api.AddValidation<FieldData, double>(
    m => m.PocketMoney , 
    n => (n > 0) && (n < 10000.0),
    "PocketMoney must be in the range $0 to $10000.");

```

![validation01](/Documents/Capture007.PNG)  

### Query Method

```cs
void Query<T>(Func<IEnumerable<T>, IEnumerable<T>> query);
void Query<T>(Action<IEnumerable<T>> query);
```

#### Filter & Sort Data

```cs
api.Query<FieldData>(source => source
    .Where(m => m.Gender == Gender.Female )
    .Where(m => !m.Married )
    .OrderBy(m => m.Age) );
```

![validation01](/Documents/Capture008.PNG) 

#### Update Data

```cs
api.Query<FieldData>( record => record
	.Where( m => m.Gender == Gender.Male )
	.Where( m => !m.Married )
	.ForEach( m =>
	{
		m.Name += " ★";
	})
);
```

![foreach](/Documents/Capture009.PNG)


## An example configuration script

```cs
ICsvEditSharpApi api = GetCsvEditSharpApi();
api.Encoding = Encoding.GetEncoding("utf-8");
api.CsvConfiguration = new CsvConfiguration( CultureInfo.GetCultureInfo("en-US"))
{
	HasHeaderRecord = true
};

/* Definition of record class 
 and set conversion options for auto mapping using attributes .
 */
class FieldData
{
	public string Name { get; set; }
	[Format("d")]
	public DateTime Birthday { get; set; }
	public Gender Gender { get; set; }
	public bool Married { get; set; }
	[NumberStyles(NumberStyles.Any)]
	[Format("C")]
	public decimal? PocketMoney { get; set; }
	public int Age { get; set; }
}

/* Define the enum type of selection item */
enum Gender
{
	Male,
	Female,
	Other
}

int BirthDayToAge(DateTime birthday)
{
	var today = DateTime.Today;
	var age = today.Year - birthday.Year;
	if( birthday.Date > today.AddYears(-age))
	{
		age--;
	}
	return age;
}

/* Manually configure additional class maps. */
api.RegisterClassMap<FieldData>(classMap =>
{
	classMap.AutoMap(api.CsvConfiguration);
    //Calculate age from birthday and set in Age column
	classMap.Map(m => m.Age).ConvertUsing(row => 
	{
		var birthday = row.GetField<DateTime>("Birthday");
		return BirthDayToAge(birthday);
	});
});

/* Add custom validations */
api.AddValidation<FieldData,DateTime>(
	m => m.Birthday, 
	dt => dt.Date <= DateTime.Today,
	"Cannot enter a future date.");

api.AddValidation<FieldData, decimal?>(
    m => m.PocketMoney , 
    n => !n.HasValue || (n > 0m) && (n < 10000.0m),
    "PocketMoney must be in the range $0 to $10000.");

```


![foreach](/Documents/Capture010.PNG)
