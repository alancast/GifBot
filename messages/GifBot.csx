using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;

// Needed for web request
using System.Net;
using System.Net.Http;
using System.IO;

// Needed for JSON deserializing
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


[Serializable]
public class GifBot : IDialog<object>
{
    private static readonly HttpClient httpClient = new HttpClient();

    public Task StartAsync(IDialogContext context)
    {
        try
        {
            context.Wait(MessageReceivedAsync);
        }
        catch (OperationCanceledException error)
        {
            return Task.FromCanceled(error.CancellationToken);
        }
        catch (Exception error)
        {
            return Task.FromException(error);
        }

        return Task.CompletedTask;
    }

    public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;

        string text = message.Text;
        text = text.Replace("<at>GifBot</at>", "");
        text = text.Trim();

        // Query GIPHY for the URL of the GIF to respond with
        HttpResponseMessage response = await httpClient.GetAsync("https://api.giphy.com/v1/gifs/search?api_key=" + Environment.GetEnvironmentVariable("GiphyApiKey") + "&limit=9&rating=g&q=" + Uri.EscapeDataString(text));
        var json = await response.Content.ReadAsStringAsync();

        // Select a random GIF URL from the first 9
        Random rnd = new Random();
        JObject o = JObject.Parse(json);
        string link = o.SelectToken("data").Select(z => (string)z["images"]["original"]["url"]).OrderBy(x => rnd.Next()).FirstOrDefault();

        // Create and send the reply message
        var replyMessage = context.MakeMessage();
        replyMessage.Text = String.IsNullOrEmpty(link)
            ? "no GIFs were found for \"" + text + "\""
            : "random GIF for \"" + text + "\" as requested by " + message.From.Name + "\n" + link;
        // TODO:#7
        // Attachment attachment = GetInternetAttachment(link);
        // replyMessage.Attachments = new List<Attachment> { attachment };
        await context.PostAsync(replyMessage);
        context.Wait(MessageReceivedAsync);
    }

    private static Attachment GetInternetAttachment(string url)
    {
        return new Attachment
        {
            Name = "YourGif",
            ContentType = "image/gif",
            ContentUrl = url
        };
    }
}