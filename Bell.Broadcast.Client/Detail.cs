using Ring.Play.Common;
using Ring.Play.Core;
using Ring.Play.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;

namespace Ring.Play.Client
{
    public partial class Detail : Form
    {
        public Detail()
        {
            InitializeComponent();

            //Login.userAccount = "linxb";
            System.Threading.Thread thread = new System.Threading.Thread(new ParameterizedThreadStart(Week));
            thread.Start("Init");

            //WeekTask();
        }

        private void Detail_Load(object sender, EventArgs e)
        {
            lbUserAccount.Text = Login.userAccount;
            BaseService<tb_PlanModel> baseServicePlay = new BaseService<tb_PlanModel>();//获取当前登录用户的播放列表
            var list = baseServicePlay.Find(x => x.UserAccount == Login.userAccount).OrderByDescending(x => x.CreateTime).ToList();
            gv.AutoGenerateColumns = false;
            gv.DataSource = list;
        }

        //单击列表按钮：播放事件
        private void gv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gv.Columns[e.ColumnIndex].Name == "btnPlay")//单击播放按钮
            {
                try
                {
                    string url = gv.Rows[e.RowIndex].Cells[6].Value.ToString();
                    //SoundPlayer simpleSound = new SoundPlayer(url);
                    //simpleSound.Play();//播放铃声

                    axWindowsMediaPlayer.URL = url;
                    //axWindowsMediaPlayer.Ctlcontrols.play();//播放文件
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }

        #region 循环播放当前用户的播放方案
        ConcurrentBag<List<PlayCountOutput>> dataBag = new ConcurrentBag<List<PlayCountOutput>>();
        List<PlayCountOutput> arrList = new List<PlayCountOutput>();//收集播放数据
        private void Week(object tag)
        {
            try
            {
                var db = new RingDBContext();
                var list = db.tb_PlanModel.Where(x => x.UserAccount == Login.userAccount && x.Status == "1").OrderByDescending(x => x.CreateTime).ToList();
                var length = list.Count;
                var listString = JsonConvert.SerializeObject(list); ;
                var countEntity = JsonConvert.DeserializeObject<List<PlayCountOutput>>(listString);//收集未播放铃声的信息

                for (int i = 0; i < length; i++)
                {
                    if (string.IsNullOrEmpty(list[i].Loop)) { continue; }//非空判断
                    if (string.IsNullOrEmpty(list[i].Time)) { continue; }

                    var loop = list[i].Loop;//循环周期：星期一，星期二，星期三，星期四
                    var arrLoop = loop.Split('，');
                    var time = list[i].Time;//播放时间：13:00,14:00,17:30
                    var arrTime = time.Split(',');

                    var arrNo = 1;//序号
                    var arr = from tb in arrLoop
                              from tb2 in arrTime
                              select new { No = (arrNo++), Id = list[i].Id, Loop = tb, Time = tb2, IsPlay = 0, Url = list[i].Url };

                    var arrString = JsonConvert.SerializeObject(arr);
                    var data = JsonConvert.DeserializeObject<List<PlayCountOutput>>(arrString);
                    arrList.AddRange(data);
                }

                if (length > 0 && tag.ToString() == "Init")//播放方案为空时，则直接不执行。
                {
                    Thread thread = new Thread(Play);//开启播放线程
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// 播放铃声
        /// </summary>
        private void Play()
        {
            try
            {
                object Lock = new object();
                string[] day = new string[] { "星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六" };
                while (true)//循环播放
                {
                    Thread.Sleep(1);//这段代码很重要，休眠1毫秒，消耗CPU可以减低百分百（否在几个无限循环就要爆CPU）
                    var nowWeek = day[Convert.ToInt16(DateTime.Now.DayOfWeek)];

                    //List集合是线程不安全，没加：ToArray()，则报错：集合已修改；可能无法执行枚举操作。
                    var listToArray = arrList.ToArray();
                    foreach (var item in listToArray)
                    {
                        if (item.Loop == nowWeek && item.IsPlay == 0)
                        {
                            var nowTime = System.DateTime.Now.ToShortTimeString();//13:00
                            //nowTime = "15:15";
                            if (item.Time == nowTime)
                            {
                                try
                                {
                                    item.IsPlay = 1; //如果铃声播放失败，捕获异常，继续循环，因为如果铃声格式不支持时，将播放失败。
                                    var url = item.Url;
                                    //Thread.Sleep(1);//如果两个设置同样的时间，则执行速度过快，会出现覆盖
                                    lock (Lock)//线程锁，避免并发时，碰在一起
                                    {
                                        Thread.Sleep(5000);
                                        axWindowsMediaPlayer.close();
                                        axWindowsMediaPlayer.URL = url;//播放文件
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        #endregion

        #region 一周循环一次任务
        /// <summary>
        ///
        /// </summary>
        private void WeekTask()
        {
            DateTime now = DateTime.Now;
            DateTime addDay = DateTime.Now.AddDays(7); //一周：7天
            int total = (int)((addDay - now).TotalMilliseconds);

            var task = new System.Threading.Timer(ClearList);
            task.Change(total, Timeout.Infinite);

            //DateTime now = DateTime.Now;
            //DateTime oneOClock = DateTime.Now.AddSeconds(10); //10秒
            //if (now > oneOClock)
            //{
            //    oneOClock = oneOClock.AddDays(1.0);
            //}
            //int msUntilFour = (int)((oneOClock - now).TotalMilliseconds);

            //var t = new System.Threading.Timer(ClearList);
            //t.Change(msUntilFour, Timeout.Infinite);
        }

        /// <summary>
        /// 执行设置播放数据为0，未播放任务
        /// </summary>
        /// <param name="state"></param>
        private void ClearList(object state)
        {
            if (arrList.Count() == 0) { return; }
            foreach (var item in arrList)
            {
                item.IsPlay = 0;//设置未播放
            }
            WeekTask();//回调，再次设定
        }
        #endregion

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                new Login().SavePlan();

                BaseService<tb_PlanModel> baseServicePlay = new BaseService<tb_PlanModel>();//获取当前登录用户的播放列表
                var list = baseServicePlay.Find(x => x.UserAccount == Login.userAccount).OrderByDescending(x => x.CreateTime).ToList();
                gv.DataSource = list;

                //var db = new RingDBContext();
                //var list = db.tb_PlanModel.Where(x => x.UserAccount == Login.userAccount).OrderByDescending(x => x.CreateTime).ToList();
                //gv.AutoGenerateColumns = false;
                //gv.DataSource = list;

                arrList = new List<PlayCountOutput>();//清空
                Week("Refresh");
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// 账号切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReturn_Click(object sender, EventArgs e)
        {
            this.Hide();

            Login mylogin = new Login();
            mylogin.StartPosition = FormStartPosition.CenterParent;
            mylogin.ShowDialog();
        }
        
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}


//var d = "[{\"Week\":\"星期五\",\"Time\":\"23:40\",\"IsPlay\":0},{\"Week\":\"星期六\",\"Time\":\"23:40\",\"IsPlay\":0}]";
//var o = JsonConvert.DeserializeObject<List<PlayCountOutput>>(d);


//public void Init()
//{
//    SoundPlayer simpleSound = new SoundPlayer(@"E:\新建文件夹\03.系统编码\AnMeng.XMChildren_Project\AnMeng.XMChildren_Project\obj\Release\Package\PackageTmp\Content\upload\noise\msg.wav");
//    simpleSound.Play();
//}