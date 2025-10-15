using BACKEND_READ_FILE.Models;

namespace BACKEND_READ_FILE.Services
{
    public interface IWordParserService
    {
        Task<ParsedTicket> ParseDocxFromFileAsync(string fullPath);
        Task<ParsedTicket> ParseDocxFromStreamAsync(Stream docxStream, string fileName);
    }

}
