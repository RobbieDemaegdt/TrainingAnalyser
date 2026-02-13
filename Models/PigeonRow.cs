namespace Maui2.Models;

public class PigeonRow
{
	public string Name { get; set; } = string.Empty;
	public string Sex { get; set; } = string.Empty;
	public string Age { get; set; } = string.Empty;
	public string Spd { get; set; } = string.Empty;
	public string Stm { get; set; } = string.Empty;
	public string Nav { get; set; } = string.Empty;
	public string Tec { get; set; } = string.Empty;
	public string Aer { get; set; } = string.Empty;
	public string Intl { get; set; } = string.Empty;
	public string Tot { get; set; } = string.Empty;
	public string Frm { get; set; } = string.Empty;
	public string Exp { get; set; } = string.Empty;
	public string Lib { get; set; } = string.Empty;
	public string Nv { get; set; } = string.Empty;

	public static PigeonRow FromPigeon(Pigeon pigeon, string resolvedName)
	{
		var skills = pigeon.Skills;
		var training = pigeon.Training;

		var skillCount = 6;

		return new PigeonRow
		{
			Name = resolvedName,
			Sex = pigeon.Sex ? "\u2642" : "\u2640",
			Age = $"{pigeon.Years}y {pigeon.Months}m",
			Spd = FormatSkill((skills?.Speed ?? 0) + 1, training, "speed"),
			Stm = FormatSkill((skills?.Stamina ?? 0) + 1, training, "stamina"),
			Nav = FormatSkill((skills?.Navigation ?? 0) + 1, training, "navigation"),
			Tec = FormatSkill((skills?.Technique ?? 0) + 1, training, "technique"),
			Aer = FormatSkill((skills?.Aerodynamics ?? 0) + 1, training, "aerodynamics"),
			Intl = FormatSkill((skills?.Intelligence ?? 0) + 1, training, "intelligence"),
			Tot = ((skills?.Total ?? 0) + skillCount).ToString(),
			Frm = ((skills?.Form ?? 0) + 1).ToString(),
			Exp = ((skills?.Experience ?? 0) + 1).ToString(),
			Lib = ((skills?.Libido ?? 0) + 1).ToString(),
			Nv = ((skills?.Nightvision ?? 0) + 1).ToString()
		};
	}

	private static string FormatSkill(int value, Dictionary<string, bool>? training, string skillKey)
	{
		if (training is not null && training.TryGetValue(skillKey, out var improved))
			return improved ? $"{value}\u2191" : $"{value}\u2193";
		return value.ToString();
	}
}
