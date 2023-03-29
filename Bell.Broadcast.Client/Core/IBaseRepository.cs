using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Play.Core
{
    /// <summary>
    /// 接口基类
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// 根据条件查询返回列表数据
        /// </summary>
        /// <param name="whereLambda">条件筛选</param>
        /// <returns></returns>
        IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda = null);

        /// <summary>
        /// 根据主键ID获取模型数据【优先使用】
        /// </summary>
        /// <param name="ID">主键</param>
        /// <returns>实体</returns>
        T Find(string ID);

        /// <summary>
        /// 查询数据【请优先使用Find(int ID)】
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns>实体</returns>
        T Find(Expression<Func<T, bool>> whereLambda);

        /// <summary>
        /// 查询是否存在记录
        /// </summary>
        /// <param name="anyLambda">条件筛选</param>
        /// <returns></returns>
        bool Exist(Expression<Func<T, bool>> anyLambda);

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">新增的对象</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        bool Add(T entity, bool isSave = true);

        /// <summary>
        /// 新增多条记录
        /// </summary>
        /// <param name="entities">新增的对象列表</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        bool Add(IEnumerable<T> entities, bool isSave = true);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">更新的对象</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        bool Update(T entity, bool isSave = true);

        /// <summary>
        /// 更新指定数据项
        /// </summary>
        /// <param name="entity">更新的对象</param>
        /// <param name="colnames">更新的列</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        bool Update(T entity, object[] colnames, bool isSave = true);

        bool Update(T entity, Expression<Func<T, object>> propertyExpression, bool isSave = true);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">删除的对象</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        bool Delete(T entity, bool isSave = true);

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">删除的对象列表</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        bool Delete(IEnumerable<T> entities, bool isSave = true);

        /// <summary>
        /// 删除指定条件的数据
        /// </summary>
        /// <param name="whereLambda">条件筛选</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        bool Delete(Expression<Func<T, bool>> whereLambda, bool isSave = true);
    }
}
