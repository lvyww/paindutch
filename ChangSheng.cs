using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Net
{
    public class ChangSheng
    {
        private string token = "";
        private string userName;
        private string password;

        private int articleId = 666;
        private string typeDate = "1970-01-01";

        public ChangSheng()
        {
        }

        public ChangSheng(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
            InitToken();
        }

        private void InitToken()
        {
            string url = "https://cl.tyu.wiki/home/login/" + userName + "/" + password;
            Dictionary<string, object> dictionary = Util.DoPost<string, object>(url);
            if (dictionary.ContainsKey("result"))
            {
                token = (string)dictionary["result"];
            }
        }

        public string GetArticle()
        {
            string url = "https://cl.tyu.wiki/tljMatch/today/" + token + "?isMobile=0";
            Dictionary<string, object> dictionary = Util.DoGet<string, object>(url);
            string title = "";
            string article = "";
            try
            {
                title = ((JObject)dictionary["result"])["article"]["title"].ToString();
                article = ((JObject)dictionary["result"])["article"]["content"].ToString();
                articleId = Convert.ToInt32(((JObject)dictionary["result"])["articleId"].ToString());
                typeDate = ((JObject)dictionary["result"])["holdDate"].ToString();
            }
            catch (Exception)
            {
                return article;
            }

            return title + "\r\n" + article + "\r\n" + "-----第0段-" + title;
        }

        /**
         * @param
         * deleteNum 退格
         * deleteText 回改
         * keyAccuracy 键准
         * keyLength 键数
         * keyMethod 键法
         * keySpeed 击键
         * mistake 错字
         * number 字数
         * repeatNum 选重
         * speed 速度
         * time 用时
         * wordRate 打词率
         */
        public string SendScore(int deleteNum, int deleteText, double keyAccuracy, double keyLength, double keyMethod,
            double keySpeed, int mistake, int number, int repeatNum, double speed, double time, double wordRate)
        {
            string url = "https://cl.tyu.wiki/tljMatch/uploadTljMatchAch/" + token;

            keyAccuracy = Math.Round(keyAccuracy, 2);
            keyLength = Math.Round(keyLength, 2);
            keyMethod = Math.Round(keyMethod, 2);
            keySpeed = Math.Round(keySpeed, 2);
            speed = Math.Round(speed, 2);
            time = Math.Round(time, 3);
            wordRate = Math.Round(wordRate, 2);
            
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"articleId\":").Append(articleId).Append(",");
            sb.Append("\"deleteNum\":").Append(deleteNum).Append(",");
            sb.Append("\"deleteText\":").Append(deleteText).Append(",");
            sb.Append("\"keyAccuracy\":").Append(keyAccuracy).Append(",");
            sb.Append("\"keyLength\":").Append(keyLength).Append(",");
            sb.Append("\"keyMethod\":").Append(keyMethod).Append(",");
            sb.Append("\"keySpeed\":").Append(keySpeed).Append(",");
            sb.Append("\"mistake\":").Append(mistake).Append(",");
            sb.Append("\"mobile\":").Append("false").Append(",");
            sb.Append("\"number\":").Append(number).Append(",");
            sb.Append("\"paragraph\":").Append(0).Append(",");
            sb.Append("\"repeatNum\":").Append(repeatNum).Append(",");
            sb.Append("\"speed\":").Append(speed).Append(",");
            sb.Append("\"time\":").Append(time).Append(",");
            sb.Append("\"typeDate\":").Append("\"").Append(typeDate).Append("\"").Append(",");
            sb.Append("\"wordRate\":").Append(wordRate);
            sb.Append("}");
            string content = sb.ToString();

            string message = "请求异常";
            Dictionary<string, object> dictionary = Util.DoPost<string, object>(url, content);
            if (dictionary.ContainsKey("message"))
            {
                message = (string)dictionary["message"];
            }

            return message;
        }
    }
}