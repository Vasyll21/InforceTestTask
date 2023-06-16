using Azure.Core;
using InforceTestTask.DataContexts;
using InforceTestTask.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InforceTestTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ShortURLController : Controller
    {
        private readonly AppDBContext _dbContext;

        public ShortURLController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet, AllowAnonymous]
        public async Task<IActionResult> GetShortUrls()
        {
            return Ok(await _dbContext.ShortUrls.ToListAsync());
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetShortUrl([FromRoute] Guid id)
        {
            var employee = await _dbContext.ShortUrls.FirstOrDefaultAsync(item => item.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShortUrl(ShortUrlDto url)
        {
            if(!Uri.TryCreate(url.Url, UriKind.Absolute, out var uri))
                return BadRequest("Invalid URL");

            var existUrl = await _dbContext.ShortUrls.FirstOrDefaultAsync(item => item.Url == url.Url);

            if (existUrl != null)
                return BadRequest("Url is already exist");

            var user = await _dbContext.Users.FirstOrDefaultAsync(item => item.Username == url.Username);

            if (user == null)
                return BadRequest("User not found");

            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPRSTUVWXYZ1234567890@asdzxcguj";
            var randomStr = new string(Enumerable.Repeat(chars, 8).Select(x => x[random.Next(x.Length)]).ToArray());

            var sUrl = new ShortUrl()
            {
                Id = Guid.NewGuid(),
                Url = url.Url,
                CreationDate = DateTime.Now,
                Short = randomStr,
                User = user
            };

            await _dbContext.ShortUrls.AddAsync(sUrl);
            await _dbContext.SaveChangesAsync();

            return Ok(sUrl);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateShortUrl([FromRoute] Guid id, UpdateShortUrlRequest request)
        {
            var url = await _dbContext.ShortUrls.FindAsync(id);
            if (url != null)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(item => item.Username == request.Username);

                if (user == null)
                    return BadRequest("Undefined user");
                else
                {
                    if (user == url.User || user.Role == "Admin")
                    {
                        url.Url = request.Url;
                        url.Short = request.Short;

                        await _dbContext.SaveChangesAsync();
                        return Ok(url);
                    }
                    else
                        return BadRequest("User not have permission");
                }
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteShortUrl([FromRoute] Guid id, UserCredentials userCred)
        {
            var url = await _dbContext.ShortUrls.FindAsync(id);
            if (url != null)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(item => item.Username == userCred.Username);

                if (user == null)
                    return BadRequest("Undefined user");
                else
                {
                    if (user == url.User || user.Role == "Admin")
                    {
                        _dbContext.ShortUrls.Remove(url);
                        await _dbContext.SaveChangesAsync();

                        return Ok(url);
                    }
                    else
                        return BadRequest("User not have permission");
                }
            }
            return NotFound();
        }
    }
}
