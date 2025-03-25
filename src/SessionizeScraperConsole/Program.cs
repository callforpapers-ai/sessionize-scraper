using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

await ScrapeSessionizeAsync("https://sessionize.com/netcoreconf-barcelona-2025/");

async Task ScrapeSessionizeAsync(string url)
{
    Console.WriteLine($"Scraping event data from {url}");

    var eventData = await FetchEventDataAsync(url);
    if (eventData == null)
    {
        Console.WriteLine("Failed to retrieve event data.");
        return;
    }

    DisplayEventInformation(eventData);
}

async Task<EventData?> FetchEventDataAsync(string url)
{
    try
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Add("User-Agent", "SessionizeScraper/1.0");

        HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        content = WebUtility.HtmlDecode(content);

        return ParseEventData(content);
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Error fetching data: {ex.Message}");
        return null;
    }
}

EventData ParseEventData(string content)
{
    var leftColumn = ExtractSection(content, "id=\"left-column\"");
    var rightColumn = ExtractSection(content, "id=\"right-column\"");

    return new EventData
    {
        Title = ExtractContent(leftColumn, "<h4>", "</h4>"),
        EventDate = ExtractContentExtra(leftColumn, "event date", "<h2 class=\"no-margins\">", "</h2>"),
        EventStartsAt = ExtractContentExtra(leftColumn, "event starts", "<h2 class=\"no-margins\">", "</h2>"),
        EventEndsAt = ExtractContentExtra(leftColumn, "event ends", "<h2 class=\"no-margins\">", "</h2>"),
        LocationLine1 = ExtractContentExtra(leftColumn, "location", "block\">", "</span>"),
        LocationLine2 = ExtractNestedContent(leftColumn, "location", "</span>", "<span class=\"block\">", "</span>"),
        Website = ExtractContentExtra(leftColumn, "website", "<a href=\"", "\""),
        Description = ExtractContent(leftColumn, "<hr class=\"m-t-none\" />", "</div>"),

        CallForPapers = new CallForPapersData
        {
            StartDate = ExtractContentExtra(rightColumn, "<div class=\"col-sm-6 m-b-sm\">", "<h2 class=\"no-margins\">", "</h2>"),
            StartTime = ExtractContentExtra(rightColumn, "<div class=\"col-sm-6 m-b-sm\">", "Call opens at", "<"),
            EndDate = ExtractNestedContent(rightColumn, "Call opens at", "Call closes at", "<h2 class=\"no-margins\">", "</h2>"),
            EndTime = ExtractContent(rightColumn, "Call closes at ", "<"),
            Timezone = ExtractContentExtra(rightColumn, "Call closes in", "<strong>", "</strong>"),
            Description = ExtractContentExtra(rightColumn, "<div class=\"col-md-12\">", "<hr class=\"m-t-none\" />", "</div>")
        }
    };
}

void DisplayEventInformation(EventData eventData)
{
    Console.WriteLine("=== Event Information ===");
    Console.WriteLine($"Title: {eventData.Title}");
    Console.WriteLine($"Date: {eventData.EventDate}");
    Console.WriteLine($"Starts: {eventData.EventStartsAt}");
    Console.WriteLine($"Ends: {eventData.EventEndsAt}");
    Console.WriteLine($"Location: {eventData.LocationLine1}, {eventData.LocationLine2}");
    Console.WriteLine($"Website: {eventData.Website}");
    Console.WriteLine($"Description: {eventData.Description.Trim()}");

    Console.WriteLine("\n=== Call for Papers ===");
    Console.WriteLine($"Opens: {eventData.CallForPapers.StartDate} at {eventData.CallForPapers.StartTime.Trim()}");
    Console.WriteLine($"Closes: {eventData.CallForPapers.EndDate} at {eventData.CallForPapers.EndTime.Trim()}");
    Console.WriteLine($"Timezone: {eventData.CallForPapers.Timezone}");
    Console.WriteLine($"CFP Description: {eventData.CallForPapers.Description.Trim()}");
}

string ExtractSection(string content, string sectionIdentifier)
{
    var parts = content.Split(sectionIdentifier, StringSplitOptions.None);
    return parts.Length > 1 ? parts[1].Trim() : string.Empty;
}

string ExtractContent(string content, string startMarker, string endMarker)
{
    var startIndex = content.IndexOf(startMarker);
    if (startIndex == -1) return string.Empty;

    startIndex += startMarker.Length;
    var endIndex = content.IndexOf(endMarker, startIndex);
    if (endIndex == -1) return string.Empty;

    return content.Substring(startIndex, endIndex - startIndex).Trim();
}

string ExtractContentExtra(string content, string contextMarker, string startMarker, string endMarker)
{
    var contextIndex = content.IndexOf(contextMarker);
    if (contextIndex == -1) return string.Empty;

    var substring = content.Substring(contextIndex);
    return ExtractContent(substring, startMarker, endMarker);
}

string ExtractNestedContent(string content, string firstMarker, string secondMarker, string startMarker, string endMarker)
{
    var firstIndex = content.IndexOf(firstMarker);
    if (firstIndex == -1) return string.Empty;

    var secondIndex = content.IndexOf(secondMarker, firstIndex + firstMarker.Length);
    if (secondIndex == -1) return string.Empty;

    var substring = content.Substring(secondIndex + secondMarker.Length);
    return ExtractContent(substring, startMarker, endMarker);
}

public class EventData
{
    public string Title { get; set; } = string.Empty;
    public string EventDate { get; set; } = string.Empty;
    public string EventStartsAt { get; set; } = string.Empty;
    public string EventEndsAt { get; set; } = string.Empty;
    public string LocationLine1 { get; set; } = string.Empty;
    public string LocationLine2 { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CallForPapersData CallForPapers { get; set; } = new();
}

public class CallForPapersData
{
    public string StartDate { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
