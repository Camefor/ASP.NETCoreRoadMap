using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnElasticsearch.Controllers {
    public class RtfController : Controller {
        public IActionResult Index() {
            string fullName = @"G:\下载\sample.rtf";
            string rtfText = string.Empty;
            using (FileStream stream = new FileStream(fullName , FileMode.Open , FileAccess.Read)) {
                if (stream != null) {
                    using (StreamReader streamReader = new StreamReader(stream , Encoding.Default)) {
                        rtfText = streamReader.ReadToEnd();
                    }
                }
            }
            ViewData["Rtf"] = rtfText;
            return View();
        }
    }
}
