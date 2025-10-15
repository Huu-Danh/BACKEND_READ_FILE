using BACKEND_READ_FILE.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
namespace BACKEND_READ_FILE.Services
{
    public class WordParserService : IWordParserService
    {
        private readonly ILogger<WordParserService> _logger;
        public WordParserService(ILogger<WordParserService> logger) => _logger = logger;

        public async Task<ParsedTicket> ParseDocxFromFileAsync(string fullPath)
        {
            await using var fs = File.OpenRead(fullPath);
            return await ParseDocxFromStreamAsync(fs, Path.GetFileName(fullPath));
        }

        private List<string> GetAllParagraphTexts(OpenXmlElement element)
        {
            var result = new List<string>();

            foreach (var child in element.Elements())
            {
                switch (child)
                {
                    case Paragraph p:
                        var text = string.Join("", p.Descendants<Text>().Select(t => t.Text)).Trim();
                        if (!string.IsNullOrEmpty(text))
                            result.Add(text);
                        break;
                    default:
                        result.AddRange(GetAllParagraphTexts(child));
                        break;
                }
            }


            return result;
        }

        public async Task<ParsedTicket> ParseDocxFromStreamAsync(Stream docxStream, string fileName)
        {
            using var ms = new MemoryStream();
            await docxStream.CopyToAsync(ms);
            ms.Position = 0;

            var parsed = new ParsedTicket { FileName = fileName };

            using var doc = WordprocessingDocument.Open(ms, false);
            var body = doc.MainDocumentPart?.Document?.Body;
            if (body == null) return parsed;

            var allParagraphs = GetAllParagraphTexts(body).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

            var fullText = string.Join("\n", allParagraphs);

            var bookingMatch = Regex.Match(fullText, @"\b([A-Z0-9]{5,7})\b");
            if (bookingMatch.Success)
                parsed.BookingCode = bookingMatch.Groups[1].Value;

            var phoneMatch = Regex.Match(fullText, @"0\d{9,10}");
            if (phoneMatch.Success)
                parsed.ContactPhone = phoneMatch.Value;

            var emailMatch = Regex.Match(fullText, @"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}");
            if (emailMatch.Success)
                parsed.ContactEmail = emailMatch.Value;

            var dateMatch = Regex.Match(fullText, @"\b\d{1,2}/\d{1,2}/\d{4}\b");
            if (dateMatch.Success)
                parsed.BookingDate = dateMatch.Value;

            var nameMatch = Regex.Match(fullText, @"(Tên|Name)[:\- ]+([A-ZÀ-Ỹ\s,]+)", RegexOptions.IgnoreCase);
            if (nameMatch.Success)
                parsed.BookerName = nameMatch.Groups[2].Value.Trim();

            var passengers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var line in allParagraphs)
            {
                var m = Regex.Match(line, @"^([A-ZÀ-Ỹ\s]+),\s*([A-ZÀ-Ỹ\s]+)$", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    var lastName = m.Groups[1].Value.Trim();
                    var firstName = m.Groups[2].Value.Trim();

                    var fullName = $"{lastName}, {firstName}".Trim();

                    if (!passengers.Contains(fullName))
                    {
                        passengers.Add(fullName);
                        parsed.Passengers.Add(new Passenger
                        {
                            LastName = lastName,
                            FirstName = firstName
                        });
                    }
                }
            }


            for (int i = 0; i < allParagraphs.Count; i++)
            {
                var line = allParagraphs[i];
                if (Regex.IsMatch(line, @"^[A-Z]{2}\d{3,4}$")) // ví dụ VJ864
                {
                    var fi = new FlightInfo
                    {
                        FlightNumber = line,
                        Date = allParagraphs.ElementAtOrDefault(i + 1),
                        FareClass = allParagraphs.ElementAtOrDefault(i + 2),
                        DepartureTimeAndPlace = allParagraphs.ElementAtOrDefault(i + 3),
                        ArrivalTimeAndPlace = allParagraphs.ElementAtOrDefault(i + 4)
                    };
                    parsed.Flights.Add(fi);
                }
            }

            return parsed;
        }

    }
}
