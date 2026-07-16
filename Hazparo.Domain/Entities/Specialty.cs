namespace Hazparo.Domain.Entities;

public class Specialty
{
    public int Id{get;set;}
    public string Title {get;set;} =default!;
    public string Description {get;set;} = default!;

    public string? IconUrl{get;set;}

    public ICollection<Professional> Professionals {get;set;} = new List<Professional>();
}