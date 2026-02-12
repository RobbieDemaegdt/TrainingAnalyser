using System.Text.Json.Serialization;

namespace Maui2.Models;

public class PigeonSkills
{
	public int Form { get; set; }
	public int Experience { get; set; }
	public int Speed { get; set; }
	public int Technique { get; set; }
	public int Stamina { get; set; }
	public int Aerodynamics { get; set; }
	public int Intelligence { get; set; }
	public int Libido { get; set; }
	public int Nightvision { get; set; }
	public int Navigation { get; set; }
	public int Total { get; set; }
}

public class Pigeon
{
	public int Id { get; set; }
	public bool Sex { get; set; }
	public int FirstNameId { get; set; }
	public int LastNameId { get; set; }
	public DateTime BirthDateTime { get; set; }
	public int Energy { get; set; }
	public int Breed { get; set; }
	public int TotalMonths { get; set; }
	public int AgeType { get; set; }
	public int Years { get; set; }
	public int Months { get; set; }
	public bool Certificate { get; set; }
	public string? Disease { get; set; }
	public bool Flying { get; set; }
	public int Premium { get; set; }
	public PigeonSkills? Skills { get; set; }
	public Dictionary<string, bool>? Training { get; set; }
}
