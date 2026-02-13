namespace Maui2.Models;

public class TransferPigeon
{
	public int Id { get; set; }
	public bool Sex { get; set; }
	public int FirstNameId { get; set; }
	public int LastNameId { get; set; }
	public DateTime BirthDateTime { get; set; }
	public int Breed { get; set; }
	public int TotalMonths { get; set; }
	public int AgeType { get; set; }
	public int Years { get; set; }
	public int Months { get; set; }
	public bool Certificate { get; set; }
	public string? Disease { get; set; }
	public int? FancierId { get; set; }
	public FancierInfo? Fancier { get; set; }
	public TransferPigeon? ParentCock { get; set; }
	public TransferPigeon? ParentHen { get; set; }
	public PigeonSkills? Skills { get; set; }
}

public class TransferItem
{
	public int Id { get; set; }
	public DateTimeOffset Start { get; set; }
	public DateTimeOffset End { get; set; }
	public int StartPrice { get; set; }
	public int? Price { get; set; }
	public TransferPigeon Pigeon { get; set; } = null!;
	public FancierInfo? Fancier { get; set; }
	public FancierInfo? Buyer { get; set; }
	public List<int> Bidders { get; set; } = [];
}
