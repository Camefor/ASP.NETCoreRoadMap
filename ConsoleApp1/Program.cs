using AngleSharp;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1 {
    class Data {
        public string text { get; set; }
        public string img { get; set; }

        public bool HasMicro { get; set; }
    }

    class MyData {
        public string text { get; set; }
        public List<string> imgs { get; set; }

        public bool HasMicro { get; set; }
    }


    public class Program {
        static HtmlParser htmlParser = new HtmlParser();
        static readonly HttpClient client = new HttpClient();
        static HttpWebRequest myReq;
        static string MTsFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\显微1\\";  // +N.ts
        static string FinallyPath = String.Empty;

        //======================Main Function======================
        static void Main(string[] args) {



            Console.ReadKey();
        }
        //======================Main Function======================


        private static void GetImageFromWeb() {
            if (!System.IO.Directory.Exists(MTsFilePath)) {
                System.IO.Directory.CreateDirectory(MTsFilePath);
            }
            //有效值001-421
            int Count = 421;
            string pad = "";
            List<string> test = new List<string>();
            List<string> test1 = new List<string>();
            List<string> test2 = new List<string>();
            List<string> test3 = new List<string>();
            List<string> test4 = new List<string>();
            List<MyData> MyData = new List<MyData>();

            //注意
            for (int j = 409; j <= Count; j++) {
                Console.WriteLine($"第 {j} 次下载任务.");
                MyData data = new MyData();
                data.imgs = new List<string>();


                #region "构造请求地址"
                var sourceUrl = "http://libproject.hkbu.edu.hk/was40/detail?channelid=47953&lang=chs&searchword=pid=B00";

                //下载带显微鉴别 的中药  的图片

                pad = j.ToString().PadLeft(3, '0');
                sourceUrl += pad;
                var Mic_sourceUrl = @$"http://libproject.hkbu.edu.hk/was40/function/cmmid_micro_uat.jsp?id=B00{pad}&lang=chs";

                #endregion ""

                //加载HTML
                //var sourceHtmlDom = GetHtml3(sourceUrl);//之前
                var sourceHtmlDom = GetHtml(Mic_sourceUrl);//下载显微鉴别 类型的

                //HTML 解析成 IDocument
                var dom = htmlParser.ParseDocument(sourceHtmlDom);

                //解析 提取
                #region "提取目标名称"
                var fileName = dom.QuerySelectorAll("p.text2");
                if (fileName != null) {
                    foreach (var p in fileName) {
                        var name = HtmlToPlainText(p.InnerHtml);// 沉香 Chenxiang

                        if (!string.IsNullOrEmpty(name)) {
                            name = name.Trim();
                            //"人参 Renshen"
                            var txtarr = name.Split(' ');
                            data.text = txtarr[0];
                            Console.WriteLine($"资源名称: {data.text}");


                            #region "提取底部文字"
                            var bottomTxt = dom.QuerySelectorAll("p.text");
                            if (bottomTxt != null) {
                                foreach (var t in bottomTxt) {
                                    var txt = HtmlToPlainText(t.InnerHtml);// 沉香 Chenxiang
                                    txt = txt.Split("本记录")[0];
                                    txt = txt.Replace(">", "");
                                    var secondDir = Path.Combine(MTsFilePath, data.text);
                                    if (!System.IO.Directory.Exists(secondDir)) {
                                        System.IO.Directory.CreateDirectory(secondDir);
                                    }
                                    var oneFileName = data.text + ".txt";
                                    var fPath = Path.Combine(secondDir, oneFileName);
                                    //不存在 创建文件
                                    //if (!System.IO.File.Exists(fPath)) {
                                    //    System.IO.File.Create(fPath);
                                    //}

                                    FileInfo myFile = new FileInfo(fPath);
                                    StreamWriter sw = myFile.CreateText();
                                    string[] strs = { txt };
                                    foreach (var s in strs) {
                                        sw.WriteLine(s);
                                    }
                                    sw.Close();
                                    //存文本到txt


                                }
                            }

                            #endregion "提取底部文字"

                            #region "提取目标图片地址"
                            var image = dom.QuerySelectorAll("img");
                            if (image != null) {
                                int count = 0;
                                foreach (var item in image) {
                                    var img = item.OuterHtml;//"<img src=\"images_mmd/trans.png\" height=\"18\">"
                                    if (img.Contains("trsimage/mmd/micro")) {//trsimage之前
                                                                             //目标图片
                                        var src = item.GetAttribute("src");//"../trsimage/mmd/B00421.jpg"
                                        var s = src.Replace("..", "");
                                        var findSrc = "http://libproject.hkbu.edu.hk" + s;//"http://libproject.hkbu.edu.hk/../trsimage/mmd/B00421.jpg"
                                        Console.WriteLine($"资源地址: { findSrc}");
                                        data.imgs.Add(findSrc);


                                        //下载
                                        var secondDir = Path.Combine(MTsFilePath, data.text);
                                        if (!System.IO.Directory.Exists(secondDir)) {
                                            System.IO.Directory.CreateDirectory(secondDir);
                                        }
                                        count++;


                                        var oneFileName = count.ToString() + Path.GetExtension(findSrc);
                                        FinallyPath = Path.Combine(secondDir, oneFileName);
                                        GetImgRes(findSrc, FinallyPath);
                                    }
                                }
                            }
                            #endregion "提取目标图片地址"
                        }
                    }
                }
                #endregion "提取目标名称"

            }
        }

        static void GetImgRes(string imgUrl, string fullPath) {
            try {
                HttpResponseMessage response = client.GetAsync(imgUrl).Result;
                response.EnsureSuccessStatusCode();
                var respnseBody = response.Content.ReadAsByteArrayAsync().Result;
                var resStream = response.Content.ReadAsStreamAsync().Result;

                if (System.IO.File.Exists(fullPath)) {
                    System.IO.File.Delete(fullPath);
                }

                //https://blog.lindexi.com/post/C-dotnet-%E5%B0%86-Stream-%E4%BF%9D%E5%AD%98%E5%88%B0%E6%96%87%E4%BB%B6%E7%9A%84%E6%96%B9%E6%B3%95.html
                using (var fileStream = File.Create(fullPath)) {
                    resStream.Seek(0, SeekOrigin.Begin);
                    resStream.CopyTo(fileStream);
                }
                Console.WriteLine("任务结束，成功保存文件");
                Console.WriteLine();
            } catch (Exception ex) {
                Console.WriteLine("保存文件错误" + ex.Message);
            }
        }
        private static string HtmlToPlainText(string html) {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);
            return text;
        }
        static public string GetHtml(string url) {
            HttpWebRequest myReq =
            (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();
            if (response.StatusCode == HttpStatusCode.OK) {
                Console.WriteLine("获取Html成功");
            }
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            return readStream.ReadToEnd();
        }

        public static Stream GetHtmlStream(string url) {
            Thread.Sleep(200);
            myReq =
           (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();
            if (response.StatusCode == HttpStatusCode.OK) {
                Console.WriteLine("获取Html成功");
            }
            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            return readStream.BaseStream;

        }



        public static string GetHtml3(string url) {
            return GetHtml_bro(url, Encoding.UTF8);
        }


        public static string GetHtml_bro(string url, Encoding ed) {
            string Html = string.Empty;//初始化新的webRequst
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(url);

            Request.KeepAlive = true;
            Request.ProtocolVersion = HttpVersion.Version11;
            Request.Method = "GET";
            Request.Accept = "*/* ";
            Request.UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/536.5 (KHTML, like Gecko) Chrome/19.0.1084.56 Safari/536.5";
            Request.Referer = url;

            HttpWebResponse htmlResponse = (HttpWebResponse)Request.GetResponse();
            //从Internet资源返回数据流
            Stream htmlStream = htmlResponse.GetResponseStream();
            //读取数据流
            StreamReader weatherStreamReader = new StreamReader(htmlStream, ed);
            //读取数据
            Html = weatherStreamReader.ReadToEnd();
            weatherStreamReader.Close();
            htmlStream.Close();
            htmlResponse.Close();
            //针对不同的网站查看html源文件
            return Html;
        }

        public static string GetHtml2(string url) {
            try {
                WebClient MyWebClient = new WebClient();
                MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
                string pageHtml = Encoding.Default.GetString(pageData);  //如果获取网站页面采用的是GB2312，则使用这句            
                //string pageHtml = Encoding.UTF8.GetString(pageData); //如果获取网站页面采用的是UTF-8，则使用这句

                Console.WriteLine(pageHtml);//在控制台输入获取的内容
                return pageHtml;
            } catch (WebException webEx) {
                Console.WriteLine(webEx.Message.ToString());
                return null;
            }
        }
    }
}
