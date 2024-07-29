using System;
using System.Collections.Generic;
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
            catch (Exception)
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