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
    public class UserStudyProgressController : ControllerBase
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        private readonly UserHelperController _userHelper;

        public UserStudyProgressController(Db context, IConfiguration config)
        {
            _db = context;
            _config = config;
            _settings = Settings.GetSettings(_config);
            _userHelper = new UserHelperController(_db, _config);
        }

        [HttpGet("{mediaId}")]
        public async Task<ActionResult<IEnumerable<UserStudyProgress>>> GetProgress(int mediaId, string sessionKey)
        {
            sessionKey = Util.UrlDecode(sessionKey);
            int userId = _userHelper.CheckToken(sessionKey);
            if (userId == 0)
            {
                return NotFound();
            }
            var list = await _db.UserStudyProgress
                //.Include(m => m.mediaSubTitle)
                //.Include(m => m.mediaSubTitle.media)
                .Where( u => (u.mediaSubTitle.media_id == mediaId && u.user_id == userId))
                .ToListAsync();
            return list;
        }

        [HttpGet("{mediaSubTitleId}")]
        public async Task<ActionResult<UserStudyProgress>> SetProgress(int mediaSubTitleId, int progress, string sessionKey)
        {
            sessionKey = Util.UrlDecode(sessionKey);
            int userId = _userHelper.CheckToken(sessionKey);
            if (userId == 0)
            {
                return NotFound();
            }
            var list = await _db.UserStudyProgress.Where(u => (u.user_id == userId
                && u.media_subtitle_id == mediaSubTitleId)).ToListAsync();
            UserStudyProgress userProgress;
            if (list.Count == 0)
            {
                userProgress = new UserStudyProgress()
                {
                    media_subtitle_id = mediaSubTitleId,
                    user_id = userId,
                    progress = progress,
                    update_date = DateTime.Now
                };
                await _db.AddAsync(userProgress);
                await _db.SaveChangesAsync();
            }
            else
            {
                userProgress = list[0];
                if (userProgress.progress < progress)
                {
                    userProgress.progress = progress;
                    userProgress.update_date = DateTime.Now;
                    _db.Entry(userProgress).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                }
            }
            return userProgress;

        }

        /*

        // GET: api/UserStudyProgress
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserStudyProgress>>> GetUserStudyProgress()
        {
            return await _db.UserStudyProgress.ToListAsync();
        }

        // GET: api/UserStudyProgress/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserStudyProgress>> GetUserStudyProgress(int id)
        {
            var userStudyProgress = await _db.UserStudyProgress.FindAsync(id);

            if (userStudyProgress == null)
            {
                return NotFound();
            }

            return userStudyProgress;
        }

        // PUT: api/UserStudyProgress/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserStudyProgress(int id, UserStudyProgress userStudyProgress)
        {
            if (id != userStudyProgress.id)
            {
                return BadRequest();
            }

            _db.Entry(userStudyProgress).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserStudyProgressExists(id))
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

        // POST: api/UserStudyProgress
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserStudyProgress>> PostUserStudyProgress(UserStudyProgress userStudyProgress)
        {
            _db.UserStudyProgress.Add(userStudyProgress);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetUserStudyProgress", new { id = userStudyProgress.id }, userStudyProgress);
        }

        // DELETE: api/UserStudyProgress/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserStudyProgress(int id)
        {
            var userStudyProgress = await _db.UserStudyProgress.FindAsync(id);
            if (userStudyProgress == null)
            {
                return NotFound();
            }

            _db.UserStudyProgress.Remove(userStudyProgress);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        */

        private bool UserStudyProgressExists(int id)
        {
            return _db.UserStudyProgress.Any(e => e.id == id);
        }
    }
}
