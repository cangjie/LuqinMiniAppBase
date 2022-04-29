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
    public class QrCodeScanLogsController : ControllerBase
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        private readonly UserHelperController _userHelper;

        public QrCodeScanLogsController(Db context, IConfiguration config)
        {
            _db = context;
            _config = config;
            _settings = Settings.GetSettings(_config);
            _userHelper = new UserHelperController(_db, _config);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<int>> LogScan(int id, string sessionKey)
        {
            sessionKey = Util.UrlDecode(sessionKey.Trim());
            int userId = _userHelper.CheckToken(sessionKey);
            if (userId <= 0)
            {
                return NotFound();
            }
            var mUserList = await _db.miniUser
                .Where(m => (m.original_id.Trim().Equals(_settings.originalId.Trim()) && m.user_id == userId))
                .ToListAsync();
            if (mUserList == null || mUserList.Count == 0)
            {
                return NotFound();
            }
            QrCodeScanLog log = new QrCodeScanLog()
            { 
                id = 0,
                poster_user_id = id,
                scan_user_id = userId,
                original_id = _settings.originalId.Trim(),
                open_id = mUserList[0].open_id,
                deal = 0
            };
            _db.QrCodeScanLog.Add(log);
            await _db.SaveChangesAsync();
            return log.id;
        }

        /*
        // GET: api/QrCodeScanLogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QrCodeScanLog>>> GetQrCodeScanLog()
        {
            return await _db.QrCodeScanLog.ToListAsync();
        }

        // GET: api/QrCodeScanLogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QrCodeScanLog>> GetQrCodeScanLog(int id)
        {
            var qrCodeScanLog = await _db.QrCodeScanLog.FindAsync(id);

            if (qrCodeScanLog == null)
            {
                return NotFound();
            }

            return qrCodeScanLog;
        }

        // PUT: api/QrCodeScanLogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQrCodeScanLog(int id, QrCodeScanLog qrCodeScanLog)
        {
            if (id != qrCodeScanLog.id)
            {
                return BadRequest();
            }

            _db.Entry(qrCodeScanLog).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QrCodeScanLogExists(id))
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

        // POST: api/QrCodeScanLogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<QrCodeScanLog>> PostQrCodeScanLog(QrCodeScanLog qrCodeScanLog)
        {
            _db.QrCodeScanLog.Add(qrCodeScanLog);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetQrCodeScanLog", new { id = qrCodeScanLog.id }, qrCodeScanLog);
        }

        // DELETE: api/QrCodeScanLogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQrCodeScanLog(int id)
        {
            var qrCodeScanLog = await _db.QrCodeScanLog.FindAsync(id);
            if (qrCodeScanLog == null)
            {
                return NotFound();
            }

            _db.QrCodeScanLog.Remove(qrCodeScanLog);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        */
        private bool QrCodeScanLogExists(int id)
        {
            return _db.QrCodeScanLog.Any(e => e.id == id);
        }
    }
}
