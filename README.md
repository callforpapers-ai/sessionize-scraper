# sessionize-scraper
Scraper samples for Sessionize session page

## Overview

This .NET console application extracts event information and Call for Papers (CFP) details from Sessionize event pages. It uses HTML parsing techniques to gather structured data about technology conferences and events, making it easier to track and monitor CFP opportunities.

## Features

- Scrapes event details from Sessionize event pages
- Extracts the following information:
  - Event title and dates
  - Event start and end times
  - Location information
  - Website URL
  - Event description
  - Call for Papers details:
    - Opening and closing dates and times
    - Timezone information
    - CFP description

## Technical Implementation

### Core Functions

#### `Main` Method
- Entry point of the application
- Handles command-line arguments to determine which URL to scrape
- Coordinates the overall scraping process and displays results

#### `GetEventDataAsync` Method
- Makes an HTTP request to the specified Sessionize URL
- Returns the raw HTML content for parsing

#### `ParseEventData` Method
- Takes raw HTML content and extracts event information
- Uses regex patterns and HTML parsing to locate and extract specific data points
- Returns a structured `EventData` object

#### `ParseCallForPapersData` Method
- Specifically targets CFP information within the HTML
- Extracts opening/closing dates, timezone, and description
- Returns a structured `CallForPapersData` object

#### `GetHtmlDocumentFromUrl` Method
- Creates an HTTP client with appropriate headers
- Makes a GET request to the specified URL
- Returns the HTML content as a string

### Data Models

#### `EventData` Class
- Represents core event information
- Properties include Title, Dates, Times, Location, Website, Description

#### `CallForPapersData` Class
- Represents CFP-specific information
- Properties include OpeningDate, ClosingDate, Timezone, Description

## Requirements

- .NET 9.0 SDK or later
- Internet connection for scraping Sessionize pages

## Installation

1. Clone the repository
```bash
git clone https://github.com/callforpapers-ai/sessionize-scraper.git
cd sessionize-scraper
```

2. Build the solution
```bash
dotnet build src/sessionizescraper.sln
```

## Usage

### Running the Application

Run the application with a specific Sessionize event URL:

```bash
cd src/SessionizeScraperConsole
dotnet run -- https://sessionize.com/your-event-name/
```

By default, the application will scrape data from "https://sessionize.com/netcoreconf-barcelona-2025/" if no URL is provided.

### Example Output

```
Event: NetCoreConf Barcelona 2025
Dates: June 21-22, 2025
Start Time: 9:00 AM
End Time: 6:00 PM
Location: Barcelona, Spain
Website: https://netcoreconf.com/barcelona-2025
Description: Join us for the premier .NET developer conference in Barcelona...

Call for Papers:
Opening Date: January 15, 2025 12:00 AM
Closing Date: April 30, 2025 11:59 PM
Timezone: Europe/Madrid
Description: We are looking for speakers on .NET Core, Azure, AI, and more...
```

## Debugging

1. Open the solution in Visual Studio or Visual Studio Code
2. Set breakpoints in the code where you want to inspect variables or flow
3. Press F5 to start debugging
4. For Visual Studio Code, ensure you have the C# extension installed

## Testing

The application can be tested manually by:

1. Running it against known Sessionize event pages
2. Verifying the output matches the actual event details
3. Testing with various URLs to ensure robust parsing

For automated testing, you can create unit tests in a separate project:

```bash
cd src
dotnet new xunit -n SessionizeScraperTests
cd SessionizeScraperTests
dotnet add reference ../SessionizeScraperConsole/SessionizeScraperConsole.csproj
dotnet add package Moq
```

Then create test cases for the parsing logic using sample HTML responses.

## Project Structure

- `src/` - Source code directory
  - `SessionizeScraperConsole/` - Main console application
    - `Program.cs` - Application entry point and main logic
    - `Models/` - Data models for parsed information
  - `sessionizescraper.sln` - Visual Studio solution file

## Common Issues

- **HTTP 403 Forbidden errors**: Sessionize may block requests that don't appear to come from a browser. The code includes appropriate User-Agent headers to mitigate this.
- **HTML structure changes**: If Sessionize updates their site layout, the parsing logic may need updating.

## License

This project is licensed under the Apache License 2.0 - see the LICENSE file for details.

## Contributing

Contributions are welcome! Feel free to open issues or submit pull requests.

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`) 
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
