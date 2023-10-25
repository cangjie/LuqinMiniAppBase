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
    public class CampRegistrationController : ControllerBase
    {
        private readonly Db _context;

        public CampRegistrationController(Db context)
        {
            _context = context;
        }



        /*

        // GET: api/CampRegistration
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampRegistration>>> GetCampRegistration()
        {
            return await _context.CampRegistration.ToListAsync();
        }

        // GET: api/CampRegistration/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CampRegistration>> GetCampRegistration(int id)
        {
            var campRegistration = await _context.CampRegistration.FindAsync(id);

            if (campRegistration == null)
            {
                return NotFound();
            }

            return campRegistration;
        }

        // PUT: api/CampRegistration/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCampRegistration(int id, CampRegistration campRegistration)
        {
            if (id != campRegistration.id)
            {
                return BadRequest();
            }

            _context.Entry(campRegistration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CampRegistrationExists(id))
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

        // POST: api/CampRegistration
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CampRegistration>> PostCampRegistration(CampRegistration campRegistration)
        {
            _context.CampRegistration.Add(campRegistration);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCampRegistration", new { id = campRegistration.id }, campRegistration);
        }

        // DELETE: api/CampRegistration/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCampRegistration(int id)
        {
            var campRegistration = await _context.CampRegistration.FindAsync(id);
            if (campRegistration == null)
            {
                return NotFound();
            }

            _context.CampRegistration.Remove(campRegistration);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool CampRegistrationExists(int id)
        {
            return _context.CampRegistration.Any(e => e.id == id);
        }
    }
}
