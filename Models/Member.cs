using Microsoft.AspNetCore.Identity;

namespace YMCAProject.Models;

public class Member
{
    public int MemberId { get; set;}
    public string FirstName {get; set;} = string.Empty;
    public string LastName {get; set;} = string.Empty!;
    public string Email {get; set;} = null!;
    public string PasswordHash {get; set;} = null!;
    public bool IsActive {get; set;}
    public bool IsMember {get; set;}
    public int FamilyId {get; set;}

}
