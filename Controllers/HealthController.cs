using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuqinMiniAppBase;
using LuqinMiniAppBase.Models;

namespace LuqinMiniAppBase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly Db _db;

        public HealthController(Db context)
        {
            _db = context;
        }

        // GET: api/Health
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Health>>> GetHealth()
        {
            return await _db.Health.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Health>> PostHealth(Health health)
        {
            var healthList = await _db.Health.Where(h => h.name.Trim().Equals(health.name)).ToListAsync();
            if (healthList.Count > 0)
            {
                return BadRequest();
            }

            await _db.AddAsync(health);
            await _db.SaveChangesAsync();
            return Ok(health);

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Health>>> GetByName(string name)
        {
            name = Util.UrlDecode(name);
            var hList = await _db.Health.Where(h => h.name.Trim().Equals(name.Trim())).ToListAsync();
            return Ok(hList);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Health>>> GetAll()
        {
            return await _db.Health.ToListAsync();
        }

        /*
        // GET: api/Health/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Health>> GetHealth(int id)
        {
            var health = await _context.Health.FindAsync(id);

            if (health == null)
            {
                return NotFound();
            }

            return health;
        }

        // PUT: api/Health/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHealth(int id, Health health)
        {
            if (id != health.id)
            {
                return BadRequest();
            }

            _context.Entry(health).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HealthExists(id))
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

        // POST: api/Health
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Health>> PostHealth(Health health)
        {
            _context.Health.Add(health);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHealth", new { id = health.id }, health);
        }

        // DELETE: api/Health/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHealth(int id)
        {
            var health = await _context.Health.FindAsync(id);
            if (health == null)
            {
                return NotFound();
            }

            _context.Health.Remove(health);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool HealthExists(int id)
        {
            return _db.Health.Any(e => e.id == id);
        }
    }
}
