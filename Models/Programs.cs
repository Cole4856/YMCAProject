namespace YMCAProject.Models;

public class Programs
{
    public int ProgramId { get; set; }
    public string ClassName { get; set; } = null!;
    public string? ClassDescription { get; set; }
    public double PriceMember { get; set; }
    public double PriceNonmember { get; set; }
    public int Capacity { get; set; }
    public int SpotsLeft { get; set; }
    public int StaffId { get; set; }
    public string Days { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int Status {get; set; }

}
