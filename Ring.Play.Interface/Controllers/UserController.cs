using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ring.Play.Interface.Controllers
{
    [ApiController]
    [Route("api/User")]

    public class UserController : ControllerBase
    {
        [Route("Get")]
        [HttpGet]
        public string Login(string UserAccount, string Password)
        {
            var msg = new { code = "50000", msg = "网络错误：" };
            if (UserAccount != "linxb" && UserAccount != "yajing")
            {
                msg = new { code = "100", msg = "账号出错！" };
            }
            else if (Password != "123456")
            {
                msg = new { code = "200", msg = "密码出错！" };
            }
            else
            {
                msg = new { code = "ok", msg = "验证通过！" };
            }

            string str = JsonConvert.SerializeObject(msg);

            return str;
        }

      
        [HttpPost]
        [Route("GetLogin")]
        public string GetLogin(User user)
        {
            string userAccount = user.UserAccount;
            string password = user.Password;

            var msg = new { code = "50000", msg = "网络错误：" };
            if (userAccount != "linxb" && userAccount != "yajing")
            {
                msg = new { code = "100", msg = "账号出错！" };
            }
            else if (password != "123456")
            {
                msg = new { code = "200", msg = "密码出错！" };
            }
            else
            {
                msg = new { code = "ok", msg = "验证通过！" };
            }

            string str = JsonConvert.SerializeObject(msg);

            return str;
        }

        [HttpGet]
        [Route("GetPlan")]
        public string GetPlan(string UserAccount)
        {
            List<tb_PlanModel> data = new List<tb_PlanModel>();
            if (UserAccount== "linxb")
            {
                data.Add(new tb_PlanModel { UserCode = "A1", UserAccount = "linxb", Scene = "第一条", Use = "msg.wav", Time = "16:04", Loop = "星期三，星期四", Status = "1", Url = "C:\\Users\\92330\\Desktop\\新建文件夹\\msg.wav" });
                data.Add(new tb_PlanModel { UserCode = "A1", UserAccount = "linxb", Scene = "第二条", Use = "当你 王心凌.mp3", Time = "16:30", Loop = "星期三", Status = "1", Url = "C:\\Users\\92330\\Desktop\\新建文件夹\\当你 王心凌.mp3" });
            }
            else if (UserAccount == "yajing")
            {
                data.Add(new tb_PlanModel { UserCode = "A2", UserAccount = "yajing", Scene = "第三条", Use = "msg.wav", Time = "13:20", Loop = "星期三，星期四", Status = "1", Url = "C:\\Users\\92330\\Desktop\\新建文件夹\\msg.wav" });
                data.Add(new tb_PlanModel { UserCode = "A2", UserAccount = "yajing", Scene = "第四条", Use = "当你 王心凌.mp3", Time = "13:30", Loop = "星期三，星期四", Status = "1", Url = "C:\\Users\\92330\\Desktop\\新建文件夹\\当你 王心凌.mp3" });
            }

            string str = JsonConvert.SerializeObject(data);
            return str;
        }
    }

    public class User
    {
        public string UserAccount { get; set; }
        public string Password { get; set; }
    }

    public class tb_PlanModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 登录用户主键
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 登录用户账号
        /// </summary>
        public string UserAccount { get; set; }

        /// <summary>
        /// 播放场景
        /// </summary>
        public string Scene { get; set; }

        /// <summary>
        /// 铃声用途
        /// </summary>
        public string Use { get; set; }

        /// <summary>
        /// 播放时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 循环周期
        /// </summary>
        public string Loop { get; set; }

        /// <summary>
        /// 是否启用（0：停用 1：启用）
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 铃声路径
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
