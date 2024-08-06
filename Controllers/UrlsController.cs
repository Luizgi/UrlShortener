using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UrlsController : ControllerBase
    {
        private readonly UrlContext _context;

        public UrlsController(UrlContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Url>> ShortenUrl([FromBody] Url url)
        {
            url.ShortUrl = GenerateShortUrl();
            url.CreatedAt = DateTime.UtcNow;

            _context.Urls.Add(url);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUrl), new { id = url.Id }, url);
        }
        
        private string GenerateShortUrl()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s=> s[random.Next(s.Length)]).ToArray());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Url>> GetUrl(int id)
        {
            var url = await _context.Urls.FindAsync(id);

            if(url == null)
            {
                return NotFound();
            }

            return url;
        }
    }
}