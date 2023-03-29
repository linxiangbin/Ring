using Newtonsoft.Json;
using Ring.Play.Common;
using Ring.Play.Core;
using Ring.Play.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ring.Play.Client
{
    public partial class Login : Form
    {
        public static string userAccount = "";//登录用户账号,全局单例
        private string serverUrl = "";
        public Login()
        {
            serverUrl = System.Configuration.ConfigurationManager.AppSettings["ServerUrl"].ToString();
            InitializeComponent();
        }


        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, EventArgs e)
        {
            userAccount = txtUserAccount.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(userAccount))
            {
                MessageBox.Show("账号不能为空！"); return;
            }
            else if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("密码不能为空！"); return;
            }

            try
            {
                var str = new StringBuilder();
                str.Append("UserAccount=" + userAccount);
                str.Append("&Password=" + password);

                string url = string.Format(serverUrl + "/User/Get?{0}", str);
                HttpClient httpClient = new HttpClient();
                //string result = httpClient.GetAsync(url).Result.Content.ReadAsStringAsync().Result;

                var param = new { UserAccount = userAccount, Password = password };
                var paramJson = JsonConvert.SerializeObject(param);
                StringContent stringContent = new StringContent(paramJson, Encoding.UTF8, "application/json");
                string url2 = serverUrl + "/User/GetLogin";

                var post = httpClient.PostAsync(url2, stringContent).Result;
                var statusCode = post.StatusCode;//状态码
                if (statusCode != HttpStatusCode.OK)
                {
                    MessageBox.Show(statusCode.ToString()); return;
                }

                //是否记住
                bool isRemeber = ckRemember.Checked;
                if (isRemeber == true)
                {
                    //Session[""]
                    //Cache
                }

                string result = post.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeObject<dynamic>(result);
                string code = res.code;
                string msg = res.msg;

                if (code == "ok")
                {
                    SavePlan();
                    this.DialogResult = DialogResult.OK;//跳转窗体
                }
                else
                {
                    MessageBox.Show(msg);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                MessageBox.Show(ex.InnerException.Message);
            }
        }

        /// <summary>
        ///  获取当前登录用户的播放方案
        /// </summary>
        public void SavePlan()
        {
            HttpClient clientPlan = new HttpClient();
            string urlPlan = serverUrl + "/User/GetPlan?UserAccount=" + userAccount;
            string resultPlan = clientPlan.GetAsync(urlPlan).Result.Content.ReadAsStringAsync().Result;
            var listPlan = JsonConvert.DeserializeObject<List<tb_PlanModel>>(resultPlan);
            foreach (var item in listPlan)
            {
                item.CreateTime = DateTime.Now;
            }
           
            //先删除当前账号的播放方案
            BaseService<tb_PlanModel> baseServicePlay = new BaseService<tb_PlanModel>();
            var deletePlan = baseServicePlay.Delete(x => x.UserAccount == userAccount);

            var result = baseServicePlay.Add(listPlan);    //添加数据库


            //var db = new RingDBContext();
            //var addRange = db.tb_PlanModel.AddRange(listPlan);
            //var dbRestult = db.SaveChanges();
        }

    }
}
