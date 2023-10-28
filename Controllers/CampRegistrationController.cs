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
    public class CampRegistrationController : ControllerBase
    {
        private readonly Db _context;

        private readonly IConfiguration _config;

        private UserHelperController _userHelper;

        public CampRegistrationController(Db context, IConfiguration config)
        {
            _context = context;
            _userHelper = new UserHelperController(_context, config);
        }

        [HttpPost]
        public ActionResult<CampRegistration> Test([FromQuery] string sessionKey, CampRegistration reg)
        {
            return Ok(reg);
        }

        [HttpPost]
        public async Task<ActionResult<CampRegistration>> NewRegister([FromQuery] string sessionKey, [FromBody]CampRegistration registration)
        {
            sessionKey = Util.UrlDecode(sessionKey);
            int userId =  _userHelper.CheckToken(sessionKey);
            if (userId <= 0)
            {
                return BadRequest();
            }
            registration.user_id = userId;
            await _context.CampRegistration.AddAsync(registration);
            await _context.SaveChangesAsync();
            return Ok(registration);
        }

        [HttpPost]
        public async Task<ActionResult<CampRegistration>> ModRegister([FromQuery] string sessionKey, [FromBody]CampRegistration registration)
        {
            sessionKey = Util.UrlDecode(sessionKey);
            int userId = _userHelper.CheckToken(sessionKey);
            if (userId <= 0)
            {
                return BadRequest();
            }

            //CampRegistration oriReg = await _context.CampRegistration.FindAsync(registration.id);
            if (registration == null || registration.user_id != userId)
            {
                return NotFound();
            }
            //registration = registration;
            _context.Entry(registration).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(registration);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CampRegistration>> GetRegistration(int id, string sessionKey)
        {
            sessionKey = Util.UrlDecode(sessionKey);
            int userId = _userHelper.CheckToken(sessionKey);
            CampRegistration reg = await _context.CampRegistration.FindAsync(id);
            if (reg == null || reg.user_id != userId)
            {
                return NotFound();
            }
            return Ok(reg);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CampRegistration>>> GetList(string sessionKey)
        {
            sessionKey = Util.UrlDecode(sessionKey);
            int userId = _userHelper.CheckToken(sessionKey);
            return Ok(await _context.CampRegistration.Where(c => c.user_id == userId).ToListAsync());
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
