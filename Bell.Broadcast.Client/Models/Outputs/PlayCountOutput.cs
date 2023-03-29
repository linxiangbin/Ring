using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Play.Models
{
    /// <summary>
    ///  播放信息统计：播放次数
    /// </summary>
    public class PlayCountOutput
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int No { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 播放星期1，2，3，4，5，6，7
        /// </summary>
        public string Loop { get; set; }

        /// <summary>
        /// 播放时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 是否已经播放：0：否 1：是
        /// </summary>
        public int IsPlay { get; set; }

        /// <summary>
        /// 铃声地址
        /// </summary>
        public string Url { get; set; }
    }
}
