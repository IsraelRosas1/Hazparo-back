namespace Hazparo.Domain.Entities;
public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    
    public Customer Customer { get; set; } = default!;
    public Professional? Professional { get; set; }
    public Address Address { get; set; } = new();

    public DateTime? ScheduledDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public decimal? QuotedPrice { get; set; }
    public decimal? FinalPrice { get; set; }
    public bool? IsPaid { get; set; }

    // Foreign key properties
    public int CustomerId { get; set; }
    public int ProfessionalId { get; set; } 

}