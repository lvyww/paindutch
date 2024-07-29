using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Net
{
    public class Sender
    {
        private static HttpClient client = new HttpClient();
        private static int WaitingTime = 5000;

        public static void SetWaitingTime(int milliseconds) => Sender.WaitingTime = milliseconds;

        public static string Get(string url)
        {
            url = Sender.FixUrl(url);
            Task<HttpResponseMessage> async = Sender.client.GetAsync(url);
            try
            {
                return Sender.RunTask(async);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Post(string url, string content)
        {
            url = Sender.FixUrl(url);
            StreamContent content1 = new StreamContent((Stream)new MemoryStream(Encoding.UTF8.GetBytes(content)));
            content1.Headers.Add("Content-Type", "application/json;charset=utf8");
            Task<HttpResponseMessage> task = Sender.client.PostAsync(url, (HttpContent)content1);
            try
            {
                return Sender.RunTask(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Post(string url, Dictionary<string, string> content)
        {
            url = Sender.FixUrl(url);
            Task<HttpResponseMessage> task = Sender.client.PostAsync(url,
                (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)content));
            try
            {
                return Sender.RunTask(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Put(string url)
        {
            url = Sender.FixUrl(url);
            Task<HttpResponseMessage> task = Sender.client.PutAsync(url, (HttpContent)null);
            try
            {
                return Sender.RunTask(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Put(string url, Dictionary<string, string> content)
        {
            url = Sender.FixUrl(url);
            Task<HttpResponseMessage> task = Sender.client.PutAsync(url,
                (HttpContent)new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)content));
            try
            {
                return Sender.RunTask(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string Delete(string url)
        {
            url = Sender.FixUrl(url);
            Task<HttpResponseMessage> task = Sender.client.DeleteAsync(url);
            try
            {
                return Sender.RunTask(task);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string RunTask(Task<HttpResponseMessage> task)
        {
            bool flag;
            try
            {
                flag = task.Wait(Sender.WaitingTime);
            }
            catch (Exception ex)
            {
                Exception exception = ex;
                while (exception.InnerException != null)
                    exception = exception.InnerException;
                throw exception;
            }

            if (!flag)
                throw new TimeoutException("Timeout");
            return task.Result.Content.ReadAsStringAsync().Result;
        }

        private static string FixUrl(string url) =>
            !url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? "http://" + url : url;
    }
}