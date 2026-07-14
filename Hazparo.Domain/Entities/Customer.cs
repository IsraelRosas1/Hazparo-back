
namespace Hazparo.Domain.Entities;

public class Customer
{
    public int Id {get;set;}
    public string Name{get;set;} = default!;
    public string? ContactEmail {get; set;}
    public string? ContactNumber {get; set;}

    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public string? Biography {get; set;}

    public Address? HomeAddress {get;set;} = default!;
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    // public ICollection<Review> Reviews {get;set;} = new List<Review>();

}