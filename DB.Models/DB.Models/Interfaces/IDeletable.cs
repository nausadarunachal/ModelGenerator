namespace DB.Models.Interfaces
{
    public interface IDeletable
    {
        bool IsActive { get; set; }
    }
}
