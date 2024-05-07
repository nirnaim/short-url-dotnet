using TinyUrlApi.Models;
using TinyUrlApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace TinyUrlApi.Controllers
{
    [ApiController]
    [Route("api/shorten")]
    public class UrlController : ControllerBase
    {
        private readonly UrlService _urlService;
        public UrlController(UrlService urlService)
        {
            _urlService = urlService;
            
        }

        [HttpGet("{shortcode:length(8)}")]
        public async Task<ActionResult> Get(string shortcode)
        {
            var urlMapping = await _urlService.GetAsyncCachedByShortCode(shortcode);
            if (urlMapping == null)
            {
                return NotFound();
            }
            return Redirect(urlMapping.LongUrl);
        }

        [HttpPost]
        public async Task<ActionResult<UrlMapping>> Post([FromBody] UrlMappingRequest urlMappingRequest)
        {
            var urlMapping = await _urlService.CreateOrGetAsync(urlMappingRequest.longUrl);
            if (urlMapping == null)
            {
                return NotFound();
            }
            return urlMapping;
        }
        public class UrlMappingRequest
        {
            public string longUrl { get; set; } = null!;
        }

    }
}
