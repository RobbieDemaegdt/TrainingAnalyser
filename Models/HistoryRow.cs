namespace Maui2.Models;

public class HistoryRow
{
	public string Age { get; set; } = string.Empty;
	public string Date { get; set; } = string.Empty;
	public string Spd { get; set; } = string.Empty;
	public string Stm { get; set; } = string.Empty;
	public string Nav { get; set; } = string.Empty;
	public string Tec { get; set; } = string.Empty;
	public string Aer { get; set; } = string.Empty;
	public string Intl { get; set; } = string.Empty;
	public string Tot { get; set; } = string.Empty;

	public static List<HistoryRow> FromSnapshots(List<PigeonSnapshot> snapshots)
	{
		var ordered = snapshots.OrderBy(s => s.TotalMonths).ToList();
		var rows = new List<HistoryRow>();

		for (var i = 0; i < ordered.Count; i++)
		{
			var current = ordered[i];
			var previous = i > 0 ? ordered[i - 1] : null;

			rows.Add(new HistoryRow
			{
				Age = $"{current.TotalMonths / 12}y {current.TotalMonths % 12}m",
				Date = current.RecordedAt.ToString("dd/MM/yyyy"),
				Spd = FormatDelta(current.Speed + 1, previous?.Speed),
				Stm = FormatDelta(current.Stamina + 1, previous?.Stamina),
				Nav = FormatDelta(current.Navigation + 1, previous?.Navigation),
				Tec = FormatDelta(current.Technique + 1, previous?.Technique),
				Aer = FormatDelta(current.Aerodynamics + 1, previous?.Aerodynamics),
				Intl = FormatDelta(current.Intelligence + 1, previous?.Intelligence),
				Tot = FormatDelta(current.Total + 6, previous?.Total, totalOffset: true)
			});
		}

		return rows;
	}

	private static string FormatDelta(int displayValue, int? previousRaw, bool totalOffset = false)
	{
		if (previousRaw is null)
			return displayValue.ToString();

		var previousDisplay = previousRaw.Value + (totalOffset ? 6 : 1);
		if (displayValue > previousDisplay)
			return $"{displayValue}\u2191";
		if (displayValue < previousDisplay)
			return $"{displayValue}\u2193";
		return displayValue.ToString();
	}
}
