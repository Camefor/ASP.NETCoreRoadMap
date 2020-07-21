using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LearnMediatR.Models;
using MediatR;
using LearnMediatR.Services;
using System.Threading;

/// <summary>
/// https://www.programmingwithwolfgang.com/mediator-pattern-in-asp-net-core-3-1/
/// </summary>
namespace LearnMediatR.Controllers {

    public class SayHelloCommand : IRequest<string> {
        public string SayHello { get; set; }
    }

    public class SayHelloCommandHandler : IRequestHandler<SayHelloCommand, string> {
        public async Task<string> Handle(SayHelloCommand request, CancellationToken cancellationToken) {
            //业务逻辑
            //这里是sayHello
            await Task.CompletedTask;
            if (request.SayHello !=null) {
                return request.SayHello;
            }
            return "Hello";
        }
    }
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IMyService _myService;
        private readonly IMediator _mediator;
        public HomeController(ILogger<HomeController> logger, IMediator mediator, IMyService myService) {
            _logger = logger;
            _mediator = mediator;
            _myService = myService;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet("sayhello")]
        public async Task<string> SayHello() {
            await Task.CompletedTask;
            //return _myService.SayHello();
            return await _mediator.Send(new SayHelloCommand {
                SayHello = "HaHa"
            });

        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
