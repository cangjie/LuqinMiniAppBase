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
    [Route("api/[controller]")]
    [ApiController]
    public class SyncSnsController : ControllerBase
    {
        private readonly Db _context;

        public SyncSnsController(Db context)
        {
            _context = context;
        }

        // GET: api/SyncSns
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SyncSns>>> GetSyncSns()
        {
            var list = await _context.SyncSns.OrderByDescending(l => l.syncsns_id).ToListAsync();
            for (int i = 0; i < list.Count; i++)
            {
                string content = list[i].syncsns_content.Trim();
                int pos = content.IndexOf("http");
                if (pos > 0)
                    content = content.Substring(0, pos);
                list[i].syncsns_content = content;
            }
            return list;
        }
        /*
        // GET: api/SyncSns/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SyncSns>> GetSyncSns(int id)
        {
            var syncSns = await _context.SyncSns.FindAsync(id);

            if (syncSns == null)
            {
                return NotFound();
            }

            return syncSns;
        }

        // PUT: api/SyncSns/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSyncSns(int id, SyncSns syncSns)
        {
            if (id != syncSns.syncsns_id)
            {
                return BadRequest();
            }

            _context.Entry(syncSns).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SyncSnsExists(id))
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

        // POST: api/SyncSns
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SyncSns>> PostSyncSns(SyncSns syncSns)
        {
            _context.SyncSns.Add(syncSns);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSyncSns", new { id = syncSns.syncsns_id }, syncSns);
        }

        // DELETE: api/SyncSns/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSyncSns(int id)
        {
            var syncSns = await _context.SyncSns.FindAsync(id);
            if (syncSns == null)
            {
                return NotFound();
            }

            _context.SyncSns.Remove(syncSns);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SyncSnsExists(int id)
        {
            return _context.SyncSns.Any(e => e.syncsns_id == id);
        }
        */
    }
}
