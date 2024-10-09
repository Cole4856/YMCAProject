namespace YMCAProject.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash {get; set;} = null;

    public bool IsActive { get; set; }
    public bool IsMember { get; set; }
}
