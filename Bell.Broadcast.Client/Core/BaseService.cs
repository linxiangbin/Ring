using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Play.Core
{
    public class BaseService<T> where T : class
    {
        //定义基础仓库接口
        protected IBaseRepository<T> CurrentRepository { get; set; }

        /// <summary>
        /// 构造函数实例化基础仓库
        /// </summary>
        public BaseService()
        {
            CurrentRepository = new BaseRepository<T>();
        }

        /// <summary>
        /// 根据主键ID查找数据
        /// </summary>
        /// <param name="ID">主键</param>
        /// <returns>实体</returns>
        public T FindByID(string ID)
        {
            return CurrentRepository.Find(ID);
        }

        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="whereLambda">过滤条件</param>
        /// <returns></returns>
        public IList<T> Find(Expression<Func<T, bool>> whereLambda)
        {
            return CurrentRepository.LoadEntities(whereLambda).ToList<T>();
        }

        /// <summary>
        /// 是否存在记录
        /// </summary>
        /// <param name="anyLambda">过滤条件</param>
        /// <returns></returns>
        public bool Exist(Expression<Func<T, bool>> anyLambda)
        {
            return CurrentRepository.Exist(anyLambda);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <param name="isSave">是否操作数据库</param>
        /// <returns>添加后的数据实体</returns>
        public bool Add(T entity, bool isSave = true)
        {
            return CurrentRepository.Add(entity, isSave);
        }

        /// <summary>
        /// 添加多条记录
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="isSave">是否操作数据库</param>
        /// <returns></returns>
        public bool Add(IList<T> entities, bool isSave = true)
        {
            return CurrentRepository.Add(entities, isSave);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <param name="isSave">是否操作数据库</param>
        /// <returns>是否成功</returns>
        public bool Update(T entity, bool isSave = true)
        {
            return CurrentRepository.Update(entity, isSave);
        }

        /// <summary>
        /// 更新指定数据项
        /// </summary>
        /// <param name="entity">更新的对象</param>
        /// <param name="colnames">更新的列</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        public bool Update(T entity, object[] colnames, bool isSave = true)
        {
            return CurrentRepository.Update(entity, colnames, isSave);
        }

        /// <summary>
        /// 按条件更新
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="isSave"></param>
        /// <returns></returns>
        public bool Update(T entity, Expression<Func<T, object>> propertyExpression, bool isSave = true)
        {
            return CurrentRepository.Update(entity, propertyExpression, isSave);
        }

        /// <summary>
        /// 单条删除
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <param name="isSave">是否操作数据库</param>
        /// <returns>是否成功</returns>
        public bool Delete(T entity, bool isSave = true)
        {
            return CurrentRepository.Delete(entity, isSave);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="whereLambda">过滤条件</param>
        /// <param name="isSave">是否操作数据库</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> whereLambda, bool isSave = true)
        {
            return CurrentRepository.Delete(whereLambda, isSave);
        }

        /// <summary>
        /// 删除多条记录
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <param name="isSave">是否操作数据库</param>
        /// <returns></returns>
        public bool Delete(IList<T> entities, bool isSave = true)
        {
            return CurrentRepository.Delete(entities, isSave);
        }
    }
}
