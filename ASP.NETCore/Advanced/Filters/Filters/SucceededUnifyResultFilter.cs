using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Filters
{
    /// <summary>
    /// 操作筛选器 异步实现
    /// </summary>
    public class SucceededUnifyResultFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 执行 Action 并获取结果
            var actionExecutedContext = await next(); // 具有执行操作方法的 ActionExecutionDelegate (next)

            //var result = actionExecutedContext.Result;
            //object? data = null;
            //// 排除以下结果，跳过规范化处理
            //var isDataResult = result switch
            //{
            //    ViewResult => false,
            //    PartialViewResult => false,
            //    FileResult => false,
            //    ChallengeResult => false,
            //    SignInResult => false,
            //    SignOutResult => false,
            //    RedirectToPageResult => false,
            //    RedirectToRouteResult => false,
            //    RedirectResult => false,
            //    RedirectToActionResult => false,
            //    LocalRedirectResult => false,
            //    ForbidResult => false,
            //    ViewComponentResult => false,
            //    _ => true,
            //};

            //// 目前支持返回值 ActionResult
            //if (isDataResult) data = result switch
            //{
            //    // 处理内容结果
            //    ContentResult content => content.Content,
            //    // 处理对象结果
            //    ObjectResult obj => obj.Value,
            //    _ => null
            //};


            ////await Task.FromResult("");
            //var jsonResult = new JsonResult(new { d = data, m = "hello" });
            //actionExecutedContext.Result = jsonResult;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }

    /// <summary>
    /// 操作筛选器 同步实现
    /// </summary>
    public class SampleActionFilter : ActionFilterAttribute
    {
        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    Console.WriteLine("动作方法执行执行之前调用");
        //}

        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    Console.WriteLine("动作方法执行执行之后调用");
        //}

    }


    /// <summary>
    /// 异步实现
    /// </summary>
    public class SampleAsyncActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Do something before the action executes.
            await next();
            // Do something after the action executes.
        }
    }



    /// <summary>
    /// 【可子类化和自定义的基于属性的内置筛选器】
    /// 结果筛选器会向响应添加标头
    /// </summary>
    public class ResponseHeaderAttribute : ActionFilterAttribute //多种筛选器接口具有相应属性，这些属性可用作自定义实现的基类。
    {
        private readonly string _name;
        private readonly string _value;
        public ResponseHeaderAttribute(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(_name, _value);
            base.OnResultExecuting(context);
        }
    }


    /**
     * 
     * 筛选器属性：
ActionFilterAttribute
ExceptionFilterAttribute
ResultFilterAttribute
FormatFilterAttribute
ServiceFilterAttribute
TypeFilterAttribute
     * 
     * 
     * **/
}
