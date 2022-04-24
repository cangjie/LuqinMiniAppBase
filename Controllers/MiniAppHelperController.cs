using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using LuqinMiniAppBase.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LuqinMiniAppBase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MiniAppHelperController : ControllerBase
    {

        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        public MiniAppHelperController(Db db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _settings = Settings.GetSettings(_config);
        }

        [HttpGet]
        public ActionResult<string> GetHomePageArcitle()
        {
            return Util.GetWebContent("http://weixin.luqinwenda.com/subscribe/api/officialaccountapi/getdraft?offset=0");
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<MiniUser>> Login(string code)
        {
            string appId = _settings.appId;
            string appSecret = _settings.appSecret;
            string checkUrl = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appId.Trim()
                + "&secret=" + appSecret.Trim() + "&js_code=" + code.Trim()
                + "&grant_type=authorization_code";
            string jsonResult = Util.GetWebContent(checkUrl);
            Code2Session sessionObj = JsonConvert.DeserializeObject<Code2Session>(jsonResult);
            var miniUser = new MiniUser()
            {
                id = 0
            };
            if (sessionObj.errcode.ToString().Equals(""))
            {
                UserHelperController userHelper = new UserHelperController(_db, _config);
                string unionId = sessionObj.unionid.Trim();
                if (!unionId.Trim().Equals(""))
                {
                    int userId = userHelper.GetUserId(unionId.Trim()).Value;
                    //New Unic User
                    if (userId == 0)
                    {
                        UnicUser unicUser = new UnicUser()
                        {
                            id = 0,
                            oa_union_id = unionId.Trim(),
                            sessionKey = ""
                        };
                        await _db.unicUser.AddAsync(unicUser);
                        try
                        {
                            await _db.SaveChangesAsync();
                            if (unicUser.id > 0)
                            {
                                string openId = sessionObj.openid.Trim();
                                miniUser = userHelper.GetMiniUser(openId.Trim()).Value;
                                if (miniUser != null)
                                {
                                    if (miniUser.user_id != userId)
                                    {
                                        miniUser.user_id = userId;
                                        _db.Entry(miniUser);
                                        try
                                        {
                                            await _db.SaveChangesAsync();
                                        }
                                        catch
                                        {

                                        }

                                    }
                                    miniUser.sessionKey = sessionObj.session_key.Trim();
                                }
                                else
                                {
                                    miniUser = new MiniUser()
                                    {
                                        id = 0,
                                        user_id = userId,
                                        open_id = openId.Trim(),
                                        original_id = _settings.originalId.Trim(),
                                        sessionKey = sessionObj.session_key.Trim()

                                    };
                                    await _db.miniUser.AddAsync(miniUser);
                                    try
                                    {
                                        await _db.SaveChangesAsync();
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            
                        }
                        catch
                        {

                        }
                    }
                    //Old Unic User
                    else
                    {
                        var unicUser = await _db.unicUser.FindAsync(userId);
                        var miniUserList = _db.miniUser
                            .Where(u => (u.user_id == userId
                            && u.original_id.Trim().Equals(_settings.originalId.Trim())))
                            .ToList();
                        if (miniUserList.Count == 0)
                        {
                            miniUser = new MiniUser()
                            {
                                id = 0,
                                user_id = userId,
                                open_id = sessionObj.openid.Trim(),
                                original_id = _settings.originalId.Trim(),
                                sessionKey = sessionObj.session_key.Trim()

                            };
                            await _db.miniUser.AddAsync(miniUser);
                            try
                            {
                                await _db.SaveChangesAsync();
                            }
                            catch
                            {

                            }
                        }
                        else
                        {
                            miniUser = miniUserList[0];
                            if (miniUser.user_id != userId)
                            {
                                miniUser.user_id = userId;
                                _db.Entry(miniUser);
                                try
                                {
                                    await _db.SaveChangesAsync();
                                }
                                catch
                                {

                                }
                            }
                            miniUser.sessionKey = sessionObj.session_key.Trim();
                        }
                    }
                    
                    //set token
                    var tokenList = _db.token.Where(t => (t.state == 1
                        && t.user_id == miniUser.user_id)).ToList();
                    for (int i = 0; i < tokenList.Count; i++)
                    {
                        tokenList[i].state = 0;
                        _db.Entry(tokenList[i]);
                        try
                        {
                            await _db.SaveChangesAsync();
                        }
                        catch
                        {

                        }
                    }
                    Token token = new Token()
                    {
                        id = 0,
                        token = miniUser.sessionKey.Trim(),
                        open_id = miniUser.open_id.Trim(),
                        original_id = _settings.originalId.Trim(),
                        user_id = miniUser.user_id,
                        expire_timestamp = 0,
                        state = 1
                    };
                    await _db.token.AddAsync(token);
                    try
                    {
                        await _db.SaveChangesAsync();
                    }
                    catch
                    {

                    }
                    miniUser.open_id = "";
                    return miniUser;
                }
                else
                {
                    return NotFound();
                }
                
            }
            return NoContent();
        }


        public class Code2Session
        {
            public string openid { get; set; } = "";
            public string session_key { get; set; } = "";
            public string unionid { get; set; } = "";
            public string errcode { get; set; } = "";
            public string errmsg { get; set; } = "";
        }
    }
}