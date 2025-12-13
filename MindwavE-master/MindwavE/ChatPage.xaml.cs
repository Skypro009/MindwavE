using System.Collections.ObjectModel;
using MindwavE.Models;
using MindwavE.Services;

namespace MindwavE;

public partial class ChatPage : ContentPage
{
    private readonly IAiService _aiService;
    public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();

    public ChatPage(IAiService aiService)
    {
        InitializeComponent();
        _aiService = aiService;
        BindingContext = this;

        // Welcome message
        Messages.Add(new ChatMessage("Zdravo! Ja sam MindwavE AI. Kako mogu da ti pomognem danas?", false));
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        var text = MessageEntry.Text;
        if (string.IsNullOrWhiteSpace(text)) return;

        // User message
        Messages.Add(new ChatMessage(text, true));
        MessageEntry.Text = string.Empty;
        
        // Auto scroll to bottom (optional enhancement)
        ChatMessagesView.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: true);

        try
        {
            // AI Response
            var response = await _aiService.GetResponseAsync(text);
            Messages.Add(new ChatMessage(response, false));
            ChatMessagesView.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: true);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Greška", "Ne mogu da dobijem odgovor od AI-a: " + ex.Message, "OK");
        }
    }
}
