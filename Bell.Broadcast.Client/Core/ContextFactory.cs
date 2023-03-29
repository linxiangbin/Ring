﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Play.Core
{
    /// <summary>
    /// 上下文简单工厂
    /// </summary>
    public class ContextFactory
    {
        //关于DbContext的单例问题，看了一些文章讲DbContex是轻量级的，创建的开销不大，另一个DbContext并不能保证线程安全。
        //对于DbContext单例化、静态化很多人反对，但每个操作都进行创建和销毁也不合理，实现一个请求内单例还是比较合适。
        //看到@Kencery这么写，我特意查了些资料：MSDN中讲CallContext提供对每个逻辑执行线程都唯一的数据槽，而在WEB程序里，每一个请求就是一个逻辑线程
        //所以使用CallContext来实现单个请求之内的DbContext单例是合理的。

        /// <summary>
        /// 获取当前数据上下文
        /// </summary>
        /// <returns></returns>
        public static RingDBContext GetCurrentContext()
        {
            RingDBContext _dbContext = CallContext.GetData("RingDBContext") as RingDBContext;
            if (_dbContext == null)
            {
                _dbContext = new RingDBContext();
                CallContext.SetData("RingDBContext", _dbContext);
            }
            return _dbContext;
        }

        /// <summary>
        /// 释放数据上下文
        /// </summary>
        public static void DisposeDBContext()
        {
            RingDBContext _dbContext = CallContext.GetData("RingDBContext") as RingDBContext;
            if (_dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = new RingDBContext();
                CallContext.SetData("RingDBContext", _dbContext);
            }
        }
    }
}
