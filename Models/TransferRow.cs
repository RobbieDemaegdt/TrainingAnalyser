namespace Maui2.Models;

public class TransferRow
{
	public int TransferId { get; set; }
	public string Id { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public string Sex { get; set; } = string.Empty;
	public string Age { get; set; } = string.Empty;
	public string Price { get; set; } = string.Empty;
	public string Seller { get; set; } = string.Empty;
	public string Buyer { get; set; } = string.Empty;
	public string Bids { get; set; } = string.Empty;
	public string TimeLeft { get; set; } = string.Empty;
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

	public static TransferRow FromRecord(TransferRecord record)
	{
		string timeLeft;
		if (record.Status != TransferStatus.Active)
		{
			timeLeft = record.Status == TransferStatus.Sold ? "Sold" : "Expired";
		}
		else
		{
			var remaining = record.End - DateTimeOffset.UtcNow;
			if (remaining.TotalSeconds <= 0)
				timeLeft = "Ended";
			else if (remaining.TotalDays >= 1)
				timeLeft = $"{(int)remaining.TotalDays}d {remaining.Hours}h";
			else if (remaining.TotalHours >= 1)
				timeLeft = $"{(int)remaining.TotalHours}h {remaining.Minutes}m";
			else
				timeLeft = $"{(int)remaining.TotalMinutes}m";
		}

		var displayPrice = record.Status == TransferStatus.Sold
			? (record.SoldPrice ?? record.CurrentPrice ?? record.StartPrice).ToString()
			: (record.CurrentPrice ?? record.StartPrice).ToString();

		var displayBuyer = record.Status == TransferStatus.Sold
			? record.SoldTo ?? record.Buyer ?? "-"
			: record.Buyer ?? "-";

		return new TransferRow
		{
			TransferId = record.TransferId,
			Id = record.PigeonId.ToString(),
			Name = record.PigeonName,
			Sex = record.Sex ? "\u2642" : "\u2640",
			Age = $"{record.Years}y {record.Months}m",
			Price = displayPrice,
			Seller = record.Seller,
			Buyer = displayBuyer,
			Bids = record.BidCount.ToString(),
			TimeLeft = timeLeft,
			Spd = FormatStat(record.Spd),
			Stm = FormatStat(record.Stm),
			Nav = FormatStat(record.Nav),
			Tec = FormatStat(record.Tec),
			Aer = FormatStat(record.Aer),
			Intl = FormatStat(record.Intl),
			Tot = FormatStat(record.Tot),
			Frm = FormatStat(record.Frm),
			Exp = FormatStat(record.Exp),
			Lib = FormatStat(record.Lib),
			Nv = FormatStat(record.Nv)
		};
	}

	private static string FormatStat(int value) => value > 0 ? value.ToString() : "-";
}
