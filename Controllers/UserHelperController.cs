using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using LuqinMiniAppBase.Models;
using Microsoft.Extensions.Configuration;

namespace LuqinMiniAppBase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserHelperController : ControllerBase
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        public UserHelperController(Db db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _settings = Settings.GetSettings(_config);
        }

        [HttpGet]
        public  ActionResult<MiniUser> GetMiniUser(string openId)
        {
            var userList = _db.miniUser
                .Where(u => (u.open_id.Trim().Equals(openId.Trim())
                && (u.original_id.Trim().Equals(_settings.originalId.Trim())))).ToList();
            if (userList.Count > 0)
            {
                return userList[0];
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public ActionResult<int> GetUserId(string unionId)
        {
            int userId = 0;
            var userList = _db.unicUser.Where(u => (u.oa_union_id.Trim().Equals(unionId.Trim())))
                .ToList();
            if (userList.Count > 0)
            {
                userId = userList[0].id;
            }
            return userId;
        }
        [NonAction]
        public int CheckToken(string token)
        {
            int userId = 0;
            var list = _db.token.Where(t => (t.token.Trim().Equals(token.Trim()) && t.state == 1
                && t.original_id.Trim().Equals(_settings.originalId))).ToList();
            if (list != null && list.Count != 0)
            {
                userId = list[0].user_id;
            }
            return userId;
        }

        [NonAction]
        public string GetOpenId(int userId, string originalId)
        {
            string openId = "";
            var userList = _db.miniUser.Where(u => (u.original_id.Trim().Equals(originalId.Trim())
                && u.user_id == userId)).ToList();
            if (userList.Count > 0)
            {
                openId = userList[0].open_id.Trim();
            }
            return openId;
        }
        

    }
}