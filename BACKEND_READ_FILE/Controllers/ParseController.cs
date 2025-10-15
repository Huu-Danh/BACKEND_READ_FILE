using BACKEND_READ_FILE.Models;
using BACKEND_READ_FILE.Services;
using Microsoft.AspNetCore.Mvc;

namespace BACKEND_READ_FILE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParseController : ControllerBase
    {
        private readonly IWordParserService _parser;
        public ParseController(IWordParserService parser) => _parser = parser;

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Chưa chọn file .docx");

            await using var stream = file.OpenReadStream();
            var result = await _parser.ParseDocxFromStreamAsync(stream, file.FileName);
            return Ok(result);
        }

        [HttpPost("by-path")]
        public async Task<IActionResult> ParseByPath([FromBody] ParsePathRequest req)
        {
            if (!System.IO.File.Exists(req.Path))
                return NotFound("Không tìm thấy file!");

            var result = await _parser.ParseDocxFromFileAsync(req.Path);
            return Ok(result);
        }
    }
}
