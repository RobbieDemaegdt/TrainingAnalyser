namespace Maui2.Models;

public class PigeonSnapshot
{
	public int TotalMonths { get; set; }
	public DateTime RecordedAt { get; set; }
	public int Speed { get; set; }
	public int Stamina { get; set; }
	public int Navigation { get; set; }
	public int Technique { get; set; }
	public int Aerodynamics { get; set; }
	public int Intelligence { get; set; }
	public int Total { get; set; }
	public int Form { get; set; }
	public int Experience { get; set; }
	public int Libido { get; set; }
	public int Nightvision { get; set; }

	public static PigeonSnapshot FromPigeon(Pigeon pigeon)
	{
		var skills = pigeon.Skills;
		return new PigeonSnapshot
		{
			TotalMonths = pigeon.TotalMonths,
			RecordedAt = DateTime.Now,
			Speed = skills?.Speed ?? 0,
			Stamina = skills?.Stamina ?? 0,
			Navigation = skills?.Navigation ?? 0,
			Technique = skills?.Technique ?? 0,
			Aerodynamics = skills?.Aerodynamics ?? 0,
			Intelligence = skills?.Intelligence ?? 0,
			Total = skills?.Total ?? 0,
			Form = skills?.Form ?? 0,
			Experience = skills?.Experience ?? 0,
			Libido = skills?.Libido ?? 0,
			Nightvision = skills?.Nightvision ?? 0
		};
	}

	public bool HasSameStats(PigeonSnapshot other)
	{
		return Speed == other.Speed
			&& Stamina == other.Stamina
			&& Navigation == other.Navigation
			&& Technique == other.Technique
			&& Aerodynamics == other.Aerodynamics
			&& Intelligence == other.Intelligence
			&& Form == other.Form
			&& Experience == other.Experience
			&& Libido == other.Libido
			&& Nightvision == other.Nightvision;
	}
}
