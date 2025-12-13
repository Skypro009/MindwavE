using System.Collections.ObjectModel;

namespace MindwavE.Models;

public class Appointment
{
    public string Name { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public string Icon { get; set; } = "👤";
    public bool IsFavorite { get; set; } = false;
    public bool IsHighlighted { get; set; } = false;
    public DateTime Date { get; set; } = DateTime.Today;
}
