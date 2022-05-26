using LuqinMiniAppBase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using System.Data;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LuqinMiniAppBase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PromoteController : ControllerBase
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        private readonly UserHelperController _userHelper;

        public PromoteController(Db context, IConfiguration config)
        {
            _db = context;
            _config = config;
            _settings = Settings.GetSettings(_config);
            _userHelper = new UserHelperController(_db, _config);
        }

        [HttpGet]
        public ActionResult<IEnumerable<PromoteResult>> GetPersonalPromoteResult(string sessionKey)
        {
      
            sessionKey = Util.UrlDecode(sessionKey);
            int userId = _userHelper.CheckToken(sessionKey);
            if (userId == 0)
            {
                return NotFound();
            }
            var promoteList = (from p in _db.promote
                               where p.promote_user_id == userId
                               group p by new
                               {
                                   date = p.create_date.Date,
                                   userId = p.promote_user_id
                               }
                     into g
                               orderby g.Key.date descending
                               select new { g.Key, followNum = g.Count() }).ToList();
            var scanList = (from s in _db.QrCodeScanLog
                            where s.poster_user_id == userId
                            group s by new
                            {
                                date = s.create_date.Date,
                                userId = s.poster_user_id
                            } into g
                            orderby g.Key.date descending
                            select new { g.Key, scanNum = g.Count() }).ToList();



            ArrayList list = new ArrayList();
            for (int i = 0; i < promoteList.Count; i++)
            {
                PromoteResult r = new PromoteResult()
                {
                    date = promoteList[i].Key.date,
                    userId = promoteList[i].Key.userId,
                    scanNum = 0,
                    followNum = promoteList[i].followNum
                };
                list.Add(r);
            }

            for (int i = 0; i < scanList.Count; i++)
            {
                bool find = false;
                for (int j = 0; j < list.Count; j++)
                {
                    PromoteResult r = (PromoteResult)list[j];
                    if (r.date == scanList[i].Key.date && r.userId == scanList[i].Key.userId)
                    {
                        find = true;
                        r.scanNum = scanList[i].scanNum;
                    }
                }
                if (!find)
                {
                    PromoteResult r = new PromoteResult()
                    {
                        date = scanList[i].Key.date,
                        userId = scanList[i].Key.userId,
                        followNum = 0,
                        scanNum = scanList[i].scanNum
                    };
                    list.Add(r);
                }
            }
            PromoteResult[] retList = new PromoteResult[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                retList[i] = (PromoteResult)list[i];
            }

            return retList;

            //return NoContent();
        }



        [HttpGet]
        public ActionResult<IEnumerable<PromoteResult>> GetAll()
        {

            var promoteList = (from p in _db.promote
                     group p by new
                     {
                         date = p.create_date.Date,
                         userId = p.promote_user_id
                     }
                     into g orderby g.Key.date descending select new { g.Key, followNum = g.Count() }).ToList();
            var scanList = (from s in _db.QrCodeScanLog
                           group s by new
                           {
                               date = s.create_date.Date,
                               userId = s.poster_user_id
                           } into g
                           orderby g.Key.date descending
                           select new { g.Key, scanNum = g.Count() }).ToList();

            

            ArrayList list = new ArrayList();
            for (int i = 0; i < promoteList.Count; i++)
            {
                PromoteResult r = new PromoteResult()
                {
                    date = promoteList[i].Key.date,
                    userId = promoteList[i].Key.userId,
                    scanNum = 0,
                    followNum = promoteList[i].followNum
                };
                list.Add(r);
            }

            for (int i = 0; i < scanList.Count; i++)
            {
                bool find = false;
                for (int j = 0; j < list.Count; j++)
                {
                    PromoteResult r = (PromoteResult)list[j];
                    if (r.date == scanList[i].Key.date && r.userId == scanList[i].Key.userId)
                    {
                        find = true;
                        r.scanNum = scanList[i].scanNum;
                    }
                }
                if (!find)
                {
                    PromoteResult r = new PromoteResult()
                    {
                        date = scanList[i].Key.date,
                        userId = scanList[i].Key.userId,
                        followNum = 0,
                        scanNum = scanList[i].scanNum
                    };
                    list.Add(r);
                }
            }
            PromoteResult[] retList = new PromoteResult[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                retList[i] = (PromoteResult)list[i];
            }

            return retList;

            

        }

        
       
        
    }
}
