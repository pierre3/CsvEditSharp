ICsvEditSharpApi api = GetCsvEditSharpApi();
api.Encoding = Encoding.GetEncoding("utf-8");
api.CsvConfiguration = new CsvConfiguration( CultureInfo.GetCultureInfo("en-US"))
{
	HasHeaderRecord = true
};

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

api.RegisterClassMap<FieldData>(classMap =>
{
	classMap.AutoMap(api.CsvConfiguration);
	classMap.Map(m => m.Age).ConvertUsing(row => 
	{
		var birthday = row.GetField<DateTime>("Birthday");
		return BirthDayToAge(birthday);
	});
});


api.AddValidation<FieldData,DateTime>(
	m => m.Birthday, 
	dt => dt.Date <= DateTime.Today,
	"Cannot enter a future date.");

api.AddValidation<FieldData, decimal?>(
    m => m.PocketMoney , 
    n => !n.HasValue || (n > 0m) && (n < 10000.0m),
    "PocketMoney must be in the range $0 to $10000.");
