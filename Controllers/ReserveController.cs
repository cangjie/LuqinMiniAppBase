using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LuqinMiniAppBase;
using LuqinMiniAppBase.Models;
using Microsoft.Extensions.Configuration;

namespace LuqinMiniAppBase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReserveController : ControllerBase
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly UserHelperController _userHelper;

        public ReserveController(Db db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _userHelper = new UserHelperController(_db, _config);
        }

        [HttpGet("{actId}")]
        public async Task<ActionResult<Reserve>> Reserve(int actId, string name,
            string cell, string sessionKey)
        {
            sessionKey = Util.UrlDecode(sessionKey);
            int userId = _userHelper.CheckToken(sessionKey);
            if (userId <= 0)
            {
                return BadRequest();
            }

            var reserveList = await _db.Reserve.Where(r => (r.act_id == actId
            && (r.user_id == userId || r.filled_cell.Trim().Equals(cell.Trim())))).ToListAsync();
            if (reserveList != null && reserveList.Count > 0)
            {
                return NotFound();
            }

            Reserve reserve = new Reserve()
            {
                act_id = actId,
                user_id = userId,
                filled_cell = Util.UrlDecode(cell),
                filled_name = Util.UrlDecode(name),
                create_date = DateTime.Now
            };
            await _db.Reserve.AddAsync(reserve);
            await _db.SaveChangesAsync();
            return Ok(reserve);
        }



        /*
        // GET: api/Reserve
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserve>>> GetReserve()
        {
            return await _context.Reserve.ToListAsync();
        }

        // GET: api/Reserve/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reserve>> GetReserve(int id)
        {
            var reserve = await _context.Reserve.FindAsync(id);

            if (reserve == null)
            {
                return NotFound();
            }

            return reserve;
        }

        // PUT: api/Reserve/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReserve(int id, Reserve reserve)
        {
            if (id != reserve.id)
            {
                return BadRequest();
            }

            _context.Entry(reserve).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReserveExists(id))
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

        // POST: api/Reserve
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Reserve>> PostReserve(Reserve reserve)
        {
            _context.Reserve.Add(reserve);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReserve", new { id = reserve.id }, reserve);
        }

        // DELETE: api/Reserve/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserve(int id)
        {
            var reserve = await _context.Reserve.FindAsync(id);
            if (reserve == null)
            {
                return NotFound();
            }

            _context.Reserve.Remove(reserve);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        */

        private bool ReserveExists(int id)
        {
            return _db.Reserve.Any(e => e.id == id);
        }
    }
}
