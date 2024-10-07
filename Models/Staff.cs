namespace YMCAProject.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string Fname { get; set; } = null!;

    public string Lname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsAdmin { get; set; }
}
