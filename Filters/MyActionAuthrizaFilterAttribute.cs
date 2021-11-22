using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SessionAuthorized.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SessionAuthorized.Demo.Filters
{
    public class MyActionAuthrizaFilterAttribute : Attribute, IActionFilter
    {
 
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }
        /// <summary>
        /// 进入action前
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //throw new NotImplementedException();
            Console.WriteLine("开始验证权限...");
           // CurrentUser currentUser = context.HttpContext.GetCurrentUserBySession();
            CurrentUser currentUser = context.HttpContext.GetCurrentUserByCookie();
            if (currentUser == null)
            {
                Console.WriteLine("没有权限...");
                if (this.IsAjaxRequest(context.HttpContext.Request))
                {
                    context.Result = new JsonResult(new
                    {
                        Success = false,
                        Message = "没有权限"
                    });
                }
                context.Result = new RedirectResult("/Account/Login");
                return;
            }
            Console.WriteLine("权限验证成功...");
        }
        private bool IsAjaxRequest(HttpRequest request)
        {
            string header = request.Headers["X-Requested-With"];
            return "XMLHttpRequest".Equals(header);
        }
    }
}
