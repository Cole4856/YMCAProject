namespace YMCAProject.Models;

/*
Author: Cole Hansen
Date: 12/9/24
Function: MemberProgram class for generating csv reports
*/
public class MemberProgram
{
    public int MemberId {get; set;}
    public string MemberName {get; set;} = string.Empty;
    public int ProgramId {get; set;}
    public string ClassName {get; set;} = string.Empty;
    public DateTime StartDate {get; set;} 
    public DateTime EndDate {get;set;}

}