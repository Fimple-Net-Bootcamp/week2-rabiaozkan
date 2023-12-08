using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Space_Weather.Models;
using Space_Weather.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace Space_Weather.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly SpaceWeatherContext _context;

        public WeatherController(SpaceWeatherContext context)
        {
            _context = context;
        }

        // GET: api/v1/weather/{planetId}
        // Sayfalama, filtreleme ve sıralama özellikleri eklendi
        [HttpGet("{planetId}")]
        public async Task<ActionResult<IEnumerable<Weather>>> GetWeatherByPlanet(
            int planetId,
            DateTime? date,
            string sort = "date",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.Weathers.Where(w => w.PlanetId == planetId);

            // Filtering
            if (date.HasValue)
            {
                query = query.Where(w => w.Date.Date == date.Value.Date);
            }

            // Sorting
            query = sort switch
            {
                "date_desc" => query.OrderByDescending(w => w.Date),
                _ => query.OrderBy(w => w.Date), // Default sorting
            };

            // Pagination
            var weathers = await query.Skip((pageNumber - 1) * pageSize)
                                      .Take(pageSize)
                                      .ToListAsync();

            if (!weathers.Any())
            {
                return NotFound();
            }

            return weathers;
        }


        // POST: api/v1/weather
        [HttpPost]
        public async Task<ActionResult<Weather>> CreateWeather([FromBody] Weather weather)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Weathers.Add(weather);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWeatherByPlanet), new { planetId = weather.PlanetId }, weather);
        }

        // PUT: api/v1/weather/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeather(int id, [FromBody] Weather weather)
        {
            if (id != weather.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(weather).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Weathers.Any(w => w.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // PATCH: api/v1/weather/{id}
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateWeather(int id, [FromBody] JsonPatchDocument<Weather> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            var weather = await _context.Weathers.FindAsync(id);
            if (weather == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(weather, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState, error =>
            {
                var errorMessage = error.ErrorMessage;
                ModelState.AddModelError(string.Empty, errorMessage);
            });



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }



        // DELETE: api/v1/weather/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeather(int id)
        {
            var weather = await _context.Weathers.FindAsync(id);
            if (weather == null)
            {
                return NotFound();
            }

            _context.Weathers.Remove(weather);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
