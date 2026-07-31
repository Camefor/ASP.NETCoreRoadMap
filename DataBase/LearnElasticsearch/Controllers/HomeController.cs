using LearnElasticsearch.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// https://code-maze.com/elasticsearch-aspnet-core/
/// </summary>
namespace LearnElasticsearch.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly ElasticClient _client;

        public HomeController(ILogger<HomeController> logger , ElasticClient client) {
            _logger = logger;
            _client = client;
        }

        public IActionResult Index(string query) {

            //Match query
            //var results = _client.Search<Book>(s => s
            //   .Query(q => q.
            //       Match(c => c
            //       .Field(f => f.Title)
            //       .Query(query)
            //        )
            //       )
            //    );

            //Range Aggregation for PageCount
            //        var results = _client.Search<Book>(s => s
            //    .Query(q => q
            //        .MatchAll()
            //    )
            //    .Aggregations(a => a
            //        .Range("pageCounts" , r => r
            //             .Field(f => f.PageCount)
            //             .Ranges(r => r.From(0) ,
            //                     r => r.From(200).To(400) ,
            //                     r => r.From(400).To(600) ,
            //                     r => r.From(600)
            //             )
            //        )
            //    )
            //);


            //Terms Aggregation for Categories
          var  results = _client.Search<Book>(s => s
    .Query(q => q
        .MatchAll()
    )
    .Aggregations(a => a
        .Range("pageCounts" , r => r
             .Field(f => f.PageCount)
             .Ranges(r => r.From(0) ,
                     r => r.From(200).To(400) ,
                     r => r.From(400).To(600) ,
                     r => r.From(600)
             )
        )
        .Terms("categories" , t => t
             .Field("categories.keyword")
        )
    )
);
            return View(results);
        }



        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0 , Location = ResponseCacheLocation.None , NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
