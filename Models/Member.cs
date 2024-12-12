using Microsoft.AspNetCore.Identity;

namespace YMCAProject.Models;

/*
Author: Cole Hansen
Date: 11/13/24
Function: member class for loading admin dashboard
*/
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
