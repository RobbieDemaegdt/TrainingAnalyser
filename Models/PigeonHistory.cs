namespace Maui2.Models;

public class PigeonHistory
{
	public int PigeonId { get; set; }
	public string Name { get; set; } = string.Empty;
	public List<PigeonSnapshot> Snapshots { get; set; } = [];

	public override string ToString() => Name;
}
