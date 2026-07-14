using System.Threading.Tasks.Dataflow;

namespace Hazparo.Domain.Entities;

public class Professional
{
    public int Id {get; set;}
    public string Name {get;set;} = default!;
    public string ContactEmail {get;set;} = default!;
    public string ContactNumber {get;set;} = default!;

    public Address HomeAddress {get;set;} = default!;


    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
}

