using System;
using System.IO;
using System.Net;

namespace TPLConsole
{
    public static class ApiHelper
    {
        /// <summary>
        /// 发起HTTP GET请求
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string HttpGet(string strUrl)
        {
            try
            {
                //定义接收的字符串
                string responseResult = String.Empty;
                //请求路径
                string url = strUrl;
                //定义request并设置request的路径
                WebRequest request = WebRequest.Create(url);   //创建一个有效的httprequest请求，地址和端口和指定路径必须要和网页系统工程师确认正确，不然一直通讯不成功
                request.Method = "get";
                //request.Headers.Add("appkey", "23f5b1bc252ca91d");
                //设置request的MIME类型及内容长度
                request.ContentType = "application/json";
                System.GC.Collect();
                HttpWebResponse apiRespone = (HttpWebResponse)request.GetResponse();
                if (apiRespone != null && apiRespone.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader sr;
                    using (sr = new StreamReader(apiRespone.GetResponseStream()))
                    {
                        responseResult = sr.ReadToEnd();  //网页系统的json格式的返回值，在responseResult里，具体内容就是网页系统负责工程师跟你协议号的返回值协议内容
                    }
                    sr.Close();
                }
                apiRespone.Close();
                return responseResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
