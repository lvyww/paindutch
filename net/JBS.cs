using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Net
{
    public class JBS
    {
        private string inputMethod = "";

        private string title = "";
        private string wordNum = "";

        private Dictionary<string, string> headers = new Dictionary<string, string>
        {
            { "cookie", "" },
            {
                "user-agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36"
            },
            { "x-requested-with", "XMLHttpRequest" }
        };

        public JBS()
        {
        }

        public JBS(string userName, string password)
        {
            Init(userName, password);
        }

        public JBS(string userName, string password, string inputMethod)
        {
            this.inputMethod = inputMethod;
            Init(userName, password);
        }

        private void Init(string userName, string password)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["u_truename"] = userName;
            data["u_password"] = password;

            string url = "https://www.jsxiaoshi.com/Home/User/login";
            Dictionary<string, string> dictionary = Util.DoPostAddHeaders(url, data);
            if (dictionary.ContainsKey("msg"))
            {
                if ("登录成功".Equals(dictionary["msg"]))
                {
                    headers["cookie"] = dictionary["cookie"];
                }
            }
        }

        public string GetArticle()
        {
            string url = "https://www.jsxiaoshi.com/index.php/Home/Common/getJbSaiWen";
            Dictionary<string, object> response = Util.DoPost(url, headers, new Dictionary<string, string>());

            string article = "";
            try
            {
                JObject msg = (JObject)response["msg"];
                wordNum = msg["6"].ToString();
                title = msg["a_name"].ToString();
                article = msg["a_content"].ToString();
            }
            catch (Exception e)
            {
                return article;
            }

            return title + "\r\n" + article + "\r\n" + "-----第100000段-" + title;
        }

        /**
         * @param
         * speed 速度
         * keystrokes 击键
         * maChang 码长
         * typingTime 时间  格式："04:39.266"
         * huiGai 回改
         * huiChe 回车
         * jianShu 键数
         * jianZhun 键准  格式："96.73%"
         * daCi 打词率  格式："63.40%"
         * wrongNum 错字数  
         * inputMethod 输入法
         */
        public string SendScore(
            string speed,
            string keystrokes,
            string maChang,
            string typingTime,
            string huiGai,
            string huiChe,
            string jianShu,
            string jianZhun,
            string daCi,
            string wrongNum
        )
        {
            string url = "https://www.jsxiaoshi.com/index.php/Home/Rank/uploadResult";
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["textTitle"] = title;
            data["speed"] = speed;
            data["keystrokes"] = keystrokes;
            data["maChang"] = maChang;
            data["wordNum"] = wordNum;
            data["typingTime"] = typingTime;
            data["huiGai"] = huiGai;
            data["huiChe"] = huiChe;
            data["jianShu"] = jianShu;
            data["jianZhun"] = jianZhun;
            data["repeatNum"] = "0";
            data["daCi"] = daCi;
            data["wrongNum"] = wrongNum;
            data["inputMethod"] = inputMethod;
            data["challengeFlag"] = "0";
            data["challengeWinner"] = "";
            data["isFirstSubmit"] = "1";

            Dictionary<string, object> response = Util.DoPost(url, headers, data);
            string msg = "请求异常";
            if (response.ContainsKey("msg"))
            {
                msg = (string)response["msg"];
            }

            return msg;
        }

        public string SendScore(
            double speed,
            double keystrokes,
            double maChang,
            TimeSpan typingTime,
            int huiGai,
            int huiChe,
            int jianShu,
            double jianZhun,
            double daCi,
            int wrongNum,
            string imeName
        )
        {
            string url = "https://www.jsxiaoshi.com/index.php/Home/Rank/uploadResult";
            Dictionary<string, string> data = new Dictionary<string, string>();
            data["textTitle"] = title;
            data["speed"] = speed.ToString("F2");
            data["keystrokes"] = keystrokes.ToString("F2");
            data["maChang"] = maChang.ToString("F2");
            data["wordNum"] = wordNum;

            string t = typingTime.ToString();
            int semi = t.LastIndexOf(":");
            if (t.Length > semi + 7)
                t = t.Substring(0, semi + 7);

            if (t.Length > 3 && t.Substring(0, 3) == "00:")
                t = t.Substring(3);


            data["typingTime"] = t;
            data["huiGai"] = huiGai.ToString("F0");
            data["huiChe"] = huiChe.ToString("F0");
            data["jianShu"] = jianShu.ToString("F0");
            data["jianZhun"] = jianZhun.ToString("P2");
            data["repeatNum"] = "0";
            data["daCi"] = daCi.ToString("P2");
            data["wrongNum"] = wrongNum.ToString();
            data["inputMethod"] = imeName;
            data["challengeFlag"] = "0";
            data["challengeWinner"] = "";
            data["isFirstSubmit"] = "1";

            Dictionary<string, object> response = Util.DoPost(url, headers, data);
            string msg = "请求异常";
            if (response.ContainsKey("msg"))
            {
                msg = (string)response["msg"];
            }

            return msg;
        }
    }
}