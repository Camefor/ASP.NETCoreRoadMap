using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnMediatR.Controllers {
    public class VideoController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
