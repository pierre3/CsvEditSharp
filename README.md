# CsvEditSharp

This is the CSV Editor, describes a read/write setting in C# script.

## Framework
- Windows Presentation Foundation (WPF) 
- .Net Framework 4.6.1

## Liblary
- [CsvHelper](https://joshclose.github.io/CsvHelper/)
- [AvalonEdit](http://avalonedit.net/)
- [Microsoft.CodeAnalysis.CSharp.Scripting](https://www.nuget.org/packages/Microsoft.CodeAnalysis.CSharp.Scripting/)

![CsvEditSharpMain01](http://link)
![CsvEditSharpMain02](http://link)

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

/* Class Mapping Settings via CsvHelper ClassMap */
RegisterClassMap<Person>(classMap =>
{
    classMap.Map(m => m.Name).Name("Name");
    classMap.Map(m => m.Birthday).Name("Birthday")
        .TypeConverterOption("M/d/yyyy");
    classMap.Map(m => m.Gender).Name("Gender");
    classMap.Map(m => m.Married).Name("Married")
        .TypeConverterOption(true,"N")
        .TypeConverterOption(false,"Y");
    
    var culcure = System.Globalization.CultureInfo.GetCultureInfo("en-us");
    classMap.Map(m => m.PocketMoney).Name("PocketMoney")
        .TypeConverterOption("C")
        .TypeConverterOption(NumberStyles.Currency)
        .TypeConverterOption(culcure);
});
```

