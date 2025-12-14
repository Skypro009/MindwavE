using System.Collections.ObjectModel;
using MindwavE.Models;

namespace MindwavE;

public partial class AppointmentsPage : ContentPage
{
    public ObservableCollection<Appointment> Appointments { get; set; }
    public DateTime SelectedDate { get; set; } = DateTime.Today;
    public int CurrentMonth { get; set; }
    public int CurrentYear { get; set; }

    public AppointmentsPage()
    {
        InitializeComponent();

        CurrentMonth = DateTime.Today.Month;
        CurrentYear = DateTime.Today.Year;

        // Initialize sample appointments
        Appointments = new ObservableCollection<Appointment>
        {
            new Appointment { Name = "Ordinacija 1", Icon = "👤", IsFavorite = false, Date = DateTime.Today },
            new Appointment { Name = "Ordinacija 2", Time = "10:00", Icon = "⭐", IsFavorite = true, Date = DateTime.Today.AddDays(2) },
            new Appointment { Name = "Ordinacija 4", Time = "14:30", Icon = "⭐", IsFavorite = true, IsHighlighted = true, Date = DateTime.Today.AddDays(4) },
            new Appointment { Name = "Ordinacija 3", Icon = "👤", IsFavorite = false, Date = DateTime.Today.AddDays(7) }
        };

        BindingContext = this;
        UpdateCalendarDisplay();
    }

    private void UpdateCalendarDisplay()
    {
        MonthLabel.Text = new DateTime(CurrentYear, CurrentMonth, 1).ToString("MMM");
        YearLabel.Text = CurrentYear.ToString();
        GenerateCalendarDays();
    }

    private void GenerateCalendarDays()
    {
        CalendarGrid.Children.Clear();

        // Add day headers
        string[] dayHeaders = { "Ne", "Po", "Ut", "Sr", "Če", "Pe", "Su" };
        for (int i = 0; i < 7; i++)
        {
            var header = new Label
            {
                Text = dayHeaders[i],
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.FromArgb("#919191"),
                FontSize = 14
            };
            CalendarGrid.Add(header, i, 0);
        }

        // Get first day of month and total days
        var firstDayOfMonth = new DateTime(CurrentYear, CurrentMonth, 1);
        int daysInMonth = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
        int startColumn = (int)firstDayOfMonth.DayOfWeek;

        // Add day numbers
        int row = 1;
        int column = startColumn;

        for (int day = 1; day <= daysInMonth; day++)
        {
            var currentDate = new DateTime(CurrentYear, CurrentMonth, day);
            bool hasAppointment = Appointments.Any(a => a.Date.Date == currentDate.Date);
            bool isSelected = SelectedDate.Date == currentDate.Date;
            bool isToday = currentDate.Date == DateTime.Today;

            View dayView;

            if (isSelected || hasAppointment)
            {
                var frame = new Frame
                {
                    Padding = 0,
                    CornerRadius = 18,
                    BackgroundColor = isSelected ? Color.FromArgb("#9F8FEF") : Color.FromArgb("#6EE7B7"),
                    HasShadow = false,
                    HeightRequest = 36,
                    WidthRequest = 36,
                    Content = new Label
                    {
                        Text = day.ToString(),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = isSelected ? Colors.White : Color.FromArgb("#064E3B"),
                        FontAttributes = FontAttributes.Bold
                    }
                };
                frame.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => OnDaySelected(currentDate))
                });
                dayView = frame;
            }
            else
            {
                var label = new Label
                {
                    Text = day.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = isToday ? Color.FromArgb("#9F8FEF") : Color.FromArgb("#212121"),
                    FontAttributes = isToday ? FontAttributes.Bold : FontAttributes.None
                };
                label.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() => OnDaySelected(currentDate))
                });
                dayView = label;
            }

            CalendarGrid.Add(dayView, column, row);

            column++;
            if (column > 6)
            {
                column = 0;
                row++;
            }
        }
    }

    private void OnDaySelected(DateTime date)
    {
        SelectedDate = date;
        UpdateCalendarDisplay();
        
        // Show appointments for selected date
        var appointmentsOnDate = Appointments.Where(a => a.Date.Date == date.Date).ToList();
        if (appointmentsOnDate.Any())
        {
            var names = string.Join("\n", appointmentsOnDate.Select(a => $"• {a.Name} - {a.Time}"));
            DisplayAlert($"Termini za {date:dd.MM.yyyy}", names, "OK");
        }
    }

    private void OnPreviousMonth(object sender, EventArgs e)
    {
        CurrentMonth--;
        if (CurrentMonth < 1)
        {
            CurrentMonth = 12;
            CurrentYear--;
        }
        UpdateCalendarDisplay();
    }

    private void OnNextMonth(object sender, EventArgs e)
    {
        CurrentMonth++;
        if (CurrentMonth > 12)
        {
            CurrentMonth = 1;
            CurrentYear++;
        }
        UpdateCalendarDisplay();
    }

    private async void OnAppointmentTapped(object sender, EventArgs e)
    {
        Appointment? apt = null;
        
        if (sender is Frame frame)
        {
            apt = frame.BindingContext as Appointment;
        }
        
        if (apt == null) return;

        string action = await DisplayActionSheet(
            $"Termin: {apt.Name}",
            "Zatvori",
            "Obriši",
            "Izmeni vreme",
            "Pogledaj detalje");

        switch (action)
        {
            case "Obriši":
                bool confirm = await DisplayAlert("Potvrda", $"Da li sigurno želiš da obrišeš termin '{apt.Name}'?", "Da", "Ne");
                if (confirm)
                {
                    Appointments.Remove(apt);
                    UpdateCalendarDisplay();
                }
                break;
                
            case "Izmeni vreme":
                string? newTime = await DisplayPromptAsync("Novo vreme", "Unesi vreme (npr. 14:30):", initialValue: apt.Time);
                if (!string.IsNullOrEmpty(newTime))
                {
                    apt.Time = newTime;
                    // Force refresh
                    var index = Appointments.IndexOf(apt);
                    Appointments.Remove(apt);
                    Appointments.Insert(index, apt);
                }
                break;
                
            case "Pogledaj detalje":
                await DisplayAlert("Detalji termina", 
                    $"Ordinacija: {apt.Name}\n" +
                    $"Vreme: {(string.IsNullOrEmpty(apt.Time) ? "Nije zakazano" : apt.Time)}\n" +
                    $"Datum: {apt.Date:dd.MM.yyyy}\n" +
                    $"Omiljeno: {(apt.IsFavorite ? "Da ⭐" : "Ne")}", 
                    "OK");
                break;
        }
    }

    private async void OnAddAppointment(object sender, EventArgs e)
    {
        string? name = await DisplayPromptAsync("Novi termin", "Unesi ime ordinacije:");
        if (string.IsNullOrEmpty(name)) return;

        string? time = await DisplayPromptAsync("Vreme termina", "Unesi vreme (npr. 10:00):", placeholder: "HH:mm");
        
        bool isFavorite = await DisplayAlert("Omiljeno", "Da li ovu ordinaciju dodajemo u omiljene?", "Da ⭐", "Ne");

        Appointments.Add(new Appointment
        {
            Name = name,
            Date = SelectedDate,
            Time = time ?? "",
            Icon = isFavorite ? "⭐" : "👤",
            IsFavorite = isFavorite
        });
        
        UpdateCalendarDisplay();
        await DisplayAlert("Uspeh", $"Termin '{name}' je uspešno dodat za {SelectedDate:dd.MM.yyyy}!", "OK");
    }
}
