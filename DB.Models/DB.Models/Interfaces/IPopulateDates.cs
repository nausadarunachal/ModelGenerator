using System;

namespace DB.Models.Interfaces
{
    public interface IPopulateDates
    {
        DateTime CreatedDate { get; set; }
        DateTime ModifiedDate { get; set; }
        void PopulateDates();
    }
}
