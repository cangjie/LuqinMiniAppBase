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
    public class QuestionController : ControllerBase
    {
        //private readonly Db _context;

        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        public QuestionController(Db db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _settings = Settings.GetSettings(_config);
        }

        /*

        // GET: api/Question
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestion()
        {
            return await _db.Question.ToListAsync();
        }
        */
        // GET: api/Question/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _db.Question.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }
        /*
        // PUT: api/Question/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(int id, Question question)
        {
            if (id != question.id)
            {
                return BadRequest();
            }

            _db.Entry(question).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
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
        */

        // POST: api/Question
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Question>> PostQuestion(string sessionKey, Question question)
        {
            UserHelperController userHelper = new UserHelperController(_db, _config);
            int userId = userHelper.CheckToken(sessionKey);
            if (userId == 0)
            {
                return NotFound();
            }
            question.user_id = userId;
            _db.Question.Add(question);
            await _db.SaveChangesAsync();
            return CreatedAtAction("GetQuestion", new { id = question.id }, question);
        }
        /*
        // DELETE: api/Question/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _db.Question.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _db.Question.Remove(question);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool QuestionExists(int id)
        {
            return _db.Question.Any(e => e.id == id);
        }
    }
}
