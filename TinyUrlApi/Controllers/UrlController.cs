using TinyUrlApi.Models;
using TinyUrlApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace TinyUrlApi.Controllers
{
    [ApiController]
    [Route("api/shorten")]
    public class UrlController : ControllerBase
    {
        private readonly UrlService _urlService;
        public UrlController(UrlService urlService) =>
        _urlService = urlService;

        [HttpGet("{shortcode:length(6)}")]
        public async Task<ActionResult<UrlMapping>> Get(string shortcode)
        {
            var urlMapping = await _urlService.GetAsyncByShortCode(shortcode);
            if (urlMapping == null)
            {
                return NotFound();
            }
            return urlMapping;
        }
        
    }
}
