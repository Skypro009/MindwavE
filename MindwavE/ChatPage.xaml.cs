using System.Collections.ObjectModel;
using MindwavE.Models;
using MindwavE.Services;
using Supabase.Gotrue;

namespace MindwavE;

public partial class ChatPage : ContentPage
{
    private readonly ChatService _chatService;
    private readonly GeminiService _geminiService;
    public ObservableCollection<ChatMessage> Messages { get; set; } = new();

    public ChatPage(ChatService chatService, GeminiService geminiService)
    {
        InitializeComponent();
        _chatService = chatService;
        _geminiService = geminiService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMessages();
    }

    private async Task LoadMessages()
    {
        try
        {
            var history = await _chatService.GetMessagesAsync();
            Messages.Clear();
            foreach (var msg in history)
            {
                Messages.Add(msg);
            }
            
            // Scroll to bottom
            if (Messages.Count > 0)
            {
                ChatMessagesView.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: false);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load chat history: {ex.Message}", "OK");
        }
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        var text = MessageEntry.Text;
        if (string.IsNullOrWhiteSpace(text)) return;

        MessageEntry.Text = string.Empty;

        // Add User Message
        var userMsg = new ChatMessage { Content = text, Role = "user", CreatedAt = DateTime.UtcNow };
        Messages.Add(userMsg);

        try
        {
            // Show typing indicator
            TypingIndicatorLayout.IsVisible = true;

            // Save User Message
            await _chatService.SaveMessageAsync(text, "user");

            var aiResponseText = await _geminiService.GetCompletionAsync(text);
            
            // Add AI Prefix to the response text for display
            var formattedResponse = $"AI: {aiResponseText}";

            // Add AI Message
            var aiMsg = new ChatMessage { Content = formattedResponse, Role = "assistant", CreatedAt = DateTime.UtcNow };
            Messages.Add(aiMsg);
            
            // Save AI Message
            await _chatService.SaveMessageAsync(formattedResponse, "assistant"); // 'assistant' is standard role for AI

            ChatMessagesView.ScrollTo(aiMsg, position: ScrollToPosition.End, animate: true);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to process message: {ex.Message}", "OK");
        }
        finally
        {
            // Hide typing indicator
            TypingIndicatorLayout.IsVisible = false;
        }
    }
}
