namespace YMCAProject.Models;

public partial class Staff
{
    public int staff_id { get; set; }

    public string fname { get; set; } = null!;

    public string lname { get; set; } = null!;

    public string email { get; set; } = null!;

    public bool is_active { get; set; }

    public bool is_admin { get; set; }
    public string PasswordHash {get; set;} = null!;
}
