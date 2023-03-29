using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Play.Models
{
    /// <summary>
    /// 播放方案
    /// </summary>
    [Table("tb_Plan")]
    public class tb_PlanModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public Nullable<int> Id { get; set; }

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

        private string _Status;
        [NotMapped]
        public string StatusText
        {
            get { if (this.Status == "0") { return "停用"; } else if (this.Status == "1") { return "启用"; } else { return "未取到"; }; }
        }

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
