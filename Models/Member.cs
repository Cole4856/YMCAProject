using System;
using Microsoft.AspNetCore.Identity;

namespace YMCAProject.Models;

public class Member : IdentityUser
{
    public int MemberId { get; set;}
    public string? FirstName {get; set;}
    public string? LastName {get; set;}
    public required string EmailAddress {get; set;}
    public required string Password {get; set;}
    public bool IsActive {get; set;}
    public bool IsMember {get; set;}

}
