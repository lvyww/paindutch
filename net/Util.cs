using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Net
{
    public class Util
    {
        public static Dictionary<TKey, TValue> DoPost<TKey, TValue>(string url)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            try
            {
                string res = Sender.Post(url, "");
                dictionary = StrToDict<TKey, TValue>(res);
            }
            catch (Exception )
            {
            }

            return dictionary;
        }

        public static Dictionary<TKey, TValue> DoPost<TKey, TValue>(string url, string content)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            try
            {
                string res = Sender.Post(url, content);
                dictionary = StrToDict<TKey, TValue>(res);
            }
            catch (Exception )
            {
            }

            return dictionary;
        }

        public static Dictionary<string, object> DoPost(string url, Dictionary<string, string> headers,
            Dictionary<string, string> data)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            string res = "";
            try
            {
                res = Sender.Post(url, headers, data);
                dictionary = StrToDict<string, object>(res);
            }
            catch (Exception)
            {
            }

            return dictionary;
        }

        public static Dictionary<string, string> DoPostAddHeaders(string url, Dictionary<string, string> data)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string res = "";
            try
            {
                res = Sender.PostAddHeaders(url, data);
                string[] strings = res.Split('|');

                // 搞返回
                string pattern = @"formReturn\('([^']+)";
                Match match = Regex.Match(res, pattern);
                if (match.Success)
                {
                    string json = match.Groups[1].Value;
                    dictionary = StrToDict<string, string>(json);
                }


                // 取cookie
                string cookie = "";
                string[] headers = strings[strings.Length - 1].Split('\n');
                foreach (string line in headers)
                {
                    if (line.StartsWith("Set-Cookie:"))
                    {
                        cookie = line.Replace("Set-Cookie:", "").Trim();
                    }
                }

                dictionary["cookie"] = cookie;
            }
            catch (Exception)
            {
            }

            return dictionary;
        }

        public static Dictionary<TKey, TValue> DoGet<TKey, TValue>(string url)
        {
            Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
            try
            {
                string res = Sender.Get(url);
                dictionary = StrToDict<TKey, TValue>(res);
            }
            catch (Exception)
            {
            }

            return dictionary;
        }

        public static Dictionary<TKey, TValue> StrToDict<TKey, TValue>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);
        }

        public static string DictToStr<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
        {
            return JsonConvert.SerializeObject(dictionary);
        }
    }
}