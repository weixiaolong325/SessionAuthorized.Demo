using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SessionAuthorized.Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SessionAuthorized.Demo.Controllers
{
    public class AccountController : Controller
    {
        //登录页面
        public IActionResult Login()
        {
            return View();
        }
        //登录提交
        [HttpPost]
        public IActionResult LoginSub(IFormCollection fromData)
        {
            string userName = fromData["userName"].ToString();
            string passWord = fromData["password"].ToString();
            //真正写法是读数据库验证
            if (userName == "test" && passWord == "123456")
            {
                #region 传统session/cookies
                //登录成功,记录用户登录信息
                //CurrentUser currentUser = new CurrentUser()
                //{
                //    Id = 123,
                //    Name = "测试账号",
                //    Account = userName
                //};

                //写sessin
                // HttpContext.Session.SetString("CurrentUser", JsonConvert.SerializeObject(currentUser));
                //写cookies
                //HttpContext.SetCookies("CurrentUser", JsonConvert.SerializeObject(currentUser));
                #endregion

                //用户角色列表，实际操作是读数据库
                var roleList = new List<string>()
                {
                    //"Admin",
                    "Test"
                };
                var claims = new List<Claim>() //用Claim保存用户信息
                {
                    new Claim(ClaimTypes.Name,"测试账号"),
                    new Claim("id","1"),
                    new Claim("account",userName),
                    new Claim("role","admin"),
                    //...可以增加任意信息
                };
                //填充角色
                foreach(var role in roleList)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                //把用户信息装到ClaimsPrincipal
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Customer"));
                //登录，把用户信息写入到cookie
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.Now.AddMinutes(30)//过期时间30分钟
                    }).Wait();

                //跳转到首页
                return RedirectToAction("Index", "Home");

            }
            else
            {
                TempData["err"] = "账号或密码不正确";
                //账号密码不对,跳回登录页
                return RedirectToAction("Login", "Account");
            }
        }
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public IActionResult LogOut()
        {
            // HttpContext.DeleteCookies("CurrentUser");
            //Session方式
            // HttpContext.Session.Remove("CurrentUser");

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
