namespace YMCAProject.Models;

/*
Author: Kylie Trousil
Date: 10/9/24
Function: staff class to be used for programs page
*/
public partial class Staff
{
    public int StaffId { get; set; }

    public string fname { get; set; } = null!;

    public string lname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool is_active { get; set; }

    public bool is_admin { get; set; }
    public string PasswordHash {get; set;} = null!;
}
