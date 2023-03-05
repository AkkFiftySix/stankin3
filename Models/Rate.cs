using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace stankin3.Models;

public class Rate
{
    [Key]
    [Format("dd.MM.yyyy")]
    public DateTime Date { get; set; }
    [Name("1 USD")]
    public double Value { get; set; }
}