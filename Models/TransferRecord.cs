namespace Maui2.Models;

public enum TransferStatus
{
	Active,
	Sold,
	Expired
}

public class TransferRecord
{
	public int TransferId { get; set; }
	public int PigeonId { get; set; }
	public string PigeonName { get; set; } = string.Empty;
	public bool Sex { get; set; }
	public int Years { get; set; }
	public int Months { get; set; }
	public int StartPrice { get; set; }
	public int? CurrentPrice { get; set; }
	public string Seller { get; set; } = string.Empty;
	public string? Buyer { get; set; }
	public int BidCount { get; set; }
	public DateTimeOffset Start { get; set; }
	public DateTimeOffset End { get; set; }
	public int Spd { get; set; }
	public int Stm { get; set; }
	public int Nav { get; set; }
	public int Tec { get; set; }
	public int Aer { get; set; }
	public int Intl { get; set; }
	public int Tot { get; set; }
	public int Frm { get; set; }
	public int Exp { get; set; }
	public int Lib { get; set; }
	public int Nv { get; set; }
	public TransferStatus Status { get; set; }
	public int? SoldPrice { get; set; }
	public string? SoldTo { get; set; }

	public static TransferRecord FromTransferItem(TransferItem item, string resolvedName)
	{
		var pigeon = item.Pigeon;
		var skills = pigeon.Skills;

		return new TransferRecord
		{
			TransferId = item.Id,
			PigeonId = pigeon.Id,
			PigeonName = resolvedName,
			Sex = pigeon.Sex,
			Years = pigeon.Years,
			Months = pigeon.Months,
			StartPrice = item.StartPrice,
			CurrentPrice = item.Price,
			Seller = item.Fancier?.DisplayName ?? "-",
			Buyer = item.Buyer?.DisplayName,
			BidCount = item.Bidders.Count,
			Start = item.Start,
			End = item.End,
			Spd = (skills?.Speed ?? 0) + 1,
			Stm = (skills?.Stamina ?? 0) + 1,
			Nav = (skills?.Navigation ?? 0) + 1,
			Tec = (skills?.Technique ?? 0) + 1,
			Aer = (skills?.Aerodynamics ?? 0) + 1,
			Intl = (skills?.Intelligence ?? 0) + 1,
			Tot = (skills?.Total ?? 0) + 6,
			Frm = (skills?.Form ?? 0) + 1,
			Exp = (skills?.Experience ?? 0) + 1,
			Lib = (skills?.Libido ?? 0) + 1,
			Nv = (skills?.Nightvision ?? 0) + 1,
			Status = TransferStatus.Active
		};
	}
}
