using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
//using SnowmeetApi.Data;
//using SnowmeetApi.Models;
using System.Threading.Tasks;
//using SnowmeetApi.Models.Users;
using LuqinMiniAppBase.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using static LuqinMiniAppBase.Controllers.TiktokHelperController.TiktokPrepayOrder;

namespace LuqinMiniAppBase.Controllers
{
    [Route("core/[controller]/[action]")]
    [ApiController]
    public class TiktokHelperController : ControllerBase
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        public class TiktokPrepayOrder
        {
            public class OrderStruct
            {
                public string order_id { get; set; }
                public string order_token { get; set; }
            }
            public string err_no { get; set; }
            public OrderStruct data { get; set; }
            
        }

        public class TiktokPaymentCallBack
        {
            public string timestamp { get; set; }
            public string nonce { get; set; }
            public string msg { get; set; }
            public string msg_signature { get; set; }
        }

        

        public TiktokHelperController(Db db, IConfiguration config)
		{
            _db = db;
            _config = config;
            _settings = Settings.GetSettings(_config);
        }

        [HttpPost]
        public async Task<ActionResult<string>> PaymentCallback()
        {
            StreamReader sr = new StreamReader(Request.Body);
            string postStr = await sr.ReadToEndAsync();
            sr.Close();
            TiktokPaymentCallBack callBack = JsonConvert.DeserializeObject<TiktokPaymentCallBack>(postStr);
            if (callBack == null)
            {
                return BadRequest();
            }
            callBack.msg = callBack.msg.Replace(@"\", "");
            SortedSet<string> sl = new SortedSet<string>(StringComparer.Ordinal);
            sl.Add(callBack.timestamp.Trim());
            sl.Add(callBack.msg.Trim());
            sl.Add(callBack.nonce.Trim());
            sl.Add(_settings.tiktokToken.Trim());
            string cyStr = "";
            foreach (string s in sl)
            {
                cyStr += s.Trim();
            }
            string sign = Util.GetSha1(cyStr);
            bool valid = false;
            if (sign.Trim().Equals(callBack.msg_signature.Trim()))
            {
                valid = true;
            }
            string callBackStr = DateTime.Now.ToString() + "\t" + valid.ToString()  +  "\t" + postStr;
            System.IO.File.AppendAllText(Util.workingPath + "/tt_payment.txt", callBackStr + "\r\n" + callBack.msg + "\r\n" + cyStr.Trim() + "\r\n");
            return Ok("{  \"err_no\": 0,  \"err_tips\": \"success\"}");
        }

        [HttpGet]
        public ActionResult<string> GetSha1(string str)
        {
            return Ok(Util.GetSha1(str));
        }

        [HttpGet]
        public ActionResult<TiktokPrepayOrder> PreOrderTest(double amount, string sub, string body)
        {
            string api = "https://" + _settings.tiktokDomain + "/api/apps/ecpay/v1/create_order";
            //api = "https://open-sandbox.douyin.com/api/apps/ecpay/v1/create_order";
            string outTradeNo = Util.GetLongTimeStamp(DateTime.Now);
            //double amount = 1;
            //string sub = "测试商品";
            //string body = "测试商品详细说明";
            amount = Math.Round(amount * 100, 0);
            int validTime = 180;
            string sign = "";
            string extra = "测试测试";
            //string notify = "https://mini.luqinwenda.com/core/TiktokHelper/PaymentCallback";
            string salt = _settings.tiktokSalt;

            //Comparer<string> comparer = System.Collections.Generic.Comparer<string>.Create(StringComparer.Ordinal);

            SortedSet<string> sortParam = new SortedSet<string>(StringComparer.Ordinal);
            
            sortParam.Add(outTradeNo);
            sortParam.Add(amount.ToString());
            sortParam.Add(sub);
            sortParam.Add(body);
            sortParam.Add(validTime.ToString());
            sortParam.Add(extra);
            //sortParam.Add(notify);
            sortParam.Add(salt);

            string cyStr = "";
            foreach (string s in sortParam)
            {
                cyStr += (cyStr.Trim().Equals("") ? "" : "&") + s.Trim();
            }

            sign = Util.GetMd5(cyStr);


            string requestJson = "{ \"app_id\": \"" + _settings.tiktokAppId + "\", \"out_order_no\": \"" + outTradeNo + "\", \"total_amount\": " + amount.ToString()
                + ",  \"subject\": \"" + sub + "\",  \"body\": \"" + body + "\",   \"valid_time\": " + validTime.ToString()
                + " ,  \"sign\": \"" + sign + "\",  \"cp_extra\": \"" + extra + "\" }";
            Console.WriteLine(requestJson);
            Console.WriteLine(cyStr);
            string r = Util.GetWebContent(api, requestJson, "application/json");
            TiktokPrepayOrder rObj = JsonConvert.DeserializeObject<TiktokPrepayOrder>(r);
            return Ok(rObj);
            //Console.WriteLine(r);
        }

        [HttpGet]
        public void SortTest()
        {
            SortedDictionary<string, string> a = new SortedDictionary<string, string>();
            
            a.Add("test", "test");
            a.Add("fuck", "fuck");
            a.Add("shit", "shit");
            a.Add("deep throat", "deep throat");
            a.Add("hand job", "hand job");
            a.Add("three some", "three some");
            foreach (KeyValuePair<string, string> kv in a)
            {
                Console.WriteLine("Key = {0}, Value = {1}",
                kv.Key, kv.Value);
            }

            SortedSet<string> aa = new SortedSet<string>();
            aa.Add("test");
            aa.Add("fuck");
            aa.Add("shit");
            aa.Add("deep throat");
            aa.Add("hand job");
            aa.Add("three some");


            foreach (string t in aa)
            {
                Console.WriteLine(t.ToString());
            }

        }

        


        [HttpGet("{code}")]
        public async Task<ActionResult<string>> Login(string code)
        {
            string postData = "{ \"appid\": \"" + _settings.tiktokAppId.Trim() + "\", \"secret\": \"" + _settings.tiktokAppSecret.Trim() + "\", "
                + " \"code\": \"" + code.Trim() + "\"}";
            Console.WriteLine(postData);
            string loginUrl = "https://" + _settings.tiktokDomain + "/api/apps/v2/jscode2session";
            string retStr = Util.GetWebContent(loginUrl, postData, "application/json");
            Console.WriteLine(retStr);
            Code2Session codeObj = JsonConvert.DeserializeObject<Code2Session>(retStr);
            string sessionKey = codeObj.data.session_key.Trim();
            if (sessionKey.Equals(""))
            {
                return BadRequest();
            }


            MiniSession session = await _db.miniSession.FindAsync(sessionKey);

            if (session != null)
            {
                return sessionKey.Trim();
            }

            session = new MiniSession()
            {
                session_key = codeObj.data.session_key,
                open_id = codeObj.data.openid,
                session_type = "tiktok"
            };
            await _db.miniSession.AddAsync(session);
            await _db.SaveChangesAsync();
            return Ok(session.session_key);
            
        }

        [HttpGet]
        public void RefreshAccessToken()
        {
            GetAccessToken();
        }


        [NonAction]
        public string GetAccessToken()
        {
            string tokenFilePath = $"{Environment.CurrentDirectory}";
            tokenFilePath = tokenFilePath + "/access_token.tiktok";
            string token = "";
            string tokenTime = Util.GetLongTimeStamp(DateTime.Parse("1970-1-1"));
            string nowTime = Util.GetLongTimeStamp(DateTime.Now);
            bool fileExists = false;
            if (System.IO.File.Exists(tokenFilePath))
            {
                fileExists = true;
                using (StreamReader sr = new StreamReader(tokenFilePath))
                {
                    try
                    {
                        token = sr.ReadLine();
                    }
                    catch
                    {

                    }
                    try
                    {
                        tokenTime = sr.ReadLine();
                    }
                    catch
                    {

                    }
                    sr.Close();
                }
                long timeDiff = long.Parse(nowTime) - long.Parse(tokenTime);
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)timeDiff);
                //TimeSpan ts = new TimeSpan()
                if (ts.TotalSeconds > 3600)
                {
                    token = "";
                    if (fileExists)
                    {
                        System.IO.File.Delete(tokenFilePath);
                    }
                }
                else
                {
                    return token.Trim();
                    //return "";
                }
            }
            string getTokenUrl = "https://" + _settings.tiktokDomain + "/api/apps/v2/token";
            string postData = "{ \"appid\": \"" + _settings.tiktokAppId + "\", \"secret\": \"" + _settings.tiktokAppSecret
                + "\",  \"grant_type\": \"client_credential\"}";
            try
            {
                string ret = Util.GetWebContent(getTokenUrl, postData, "application/json");
                
                AccessToken at = JsonConvert.DeserializeObject<AccessToken>(ret);
                if (!at.data.access_token.Trim().Equals(""))
                {
                    System.IO.File.WriteAllText(tokenFilePath, at.data.access_token.Trim() + "\r\n" + nowTime);
                    return at.data.access_token.Trim().Trim();
                    //return "";
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }

        }
        public class Code2Session
        {
            public class DataStruct
            {
                public string openid { get; set; } = "";
                public string session_key { get; set; } = "";
                public string anonymous_openid { get; set; } = "";
                public string unionid { get; set; } = "";
            }

            public int err_no { get; set; }
            public string err_tips { get; set; }
            public DataStruct data { get; set; }
        }

        public class AccessToken
        {
            public struct DataStruct
            {
                public string access_token { get; set; }
                public int expires_in { get; set; }
            }
            public int err_no { get; set; }
            public string err_tips { get; set; }
            public DataStruct data { get; set; }
            

        }
    }
}

