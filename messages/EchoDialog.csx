using System;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Net.Http;

// Needed for web request
using System.Net;
using System.IO;

// Needed for JSON deserializing
using System.Collections.Generic;
using System.Web.Script.Serialization;

// Classes for GiphyJson deserialization
class GiphyImages
{
    public string url { get; set; }
}
class GiphyData
{
    public Dictionary<string, GiphyImages> images { get; set; }
}
class GiphyJson
{
    public List<GiphyData> data { get; set;}
}

// For more information about this template visit http://aka.ms/azurebots-csharp-basic
[Serializable]
public class EchoDialog : IDialog<object>
{
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

        // Get html response for gif query
        string html = string.Empty;
        string url = @"https://api.giphy.com/v1/gifs/search?api_key=" + Environment.GetEnvironmentVariable("GiphyApiKey") + "&limit=9&rating=g&q=" + Uri.EscapeDataString(text);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.AutomaticDecompression = DecompressionMethods.GZip;
        
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            html = reader.ReadToEnd();
        }
        
        // Pick randomly from the list and get the url for the gif
        Random rnd = new Random();
        JavaScriptSerializer json = new JavaScriptSerializer();
        var JSONObj = json.Deserialize<GiphyJson>(html);
        string link = JSONObj.data[rnd.Next(1, 10)].images["original"].url;
        
        // Create and send the reply message
        var replyMessage = context.MakeMessage();
        replyMessage.Text = "random GIF for \"" + text + "\" as requested by " + message.From.Name + "\n" + link;
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