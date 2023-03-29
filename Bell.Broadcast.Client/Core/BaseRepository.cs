using Ring.Play.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ring.Play.Core
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {

        protected RingDBContext dbContext = ContextFactory.GetCurrentContext();

        private DbSet<T> Entities { get { return dbContext.Set<T>(); } }

        private DbEntityEntry<T> GetEntity(T entity)
        {
            try
            {
                return dbContext.Entry<T>(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 提交数据上下文更新
        /// </summary>
        /// <returns></returns>
        public bool SaveChange()
        {
            try
            {
                return dbContext.SaveChanges() > 0;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 重新加载数据上下文
        /// </summary>
        public void Dispose()
        {
            ContextFactory.DisposeDBContext();
        }

        /// <summary>
        /// 数据实体列表
        /// </summary>
        public IQueryable<T> QEntities { get { return Entities; } }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">新增的对象</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        public bool Add(T entity, bool isSave = true)
        {
            try
            {
                Entities.Add(entity);
                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 新增多条记录
        /// </summary>
        /// <param name="entities">新增的对象列表</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        public bool Add(IEnumerable<T> entities, bool isSave = true)
        {
            try
            {
                foreach (T entity in entities)
                {
                    GetEntity(entity).State = System.Data.Entity.EntityState.Added;
                }
                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }
        /// <summary>
        /// 获取条件结果记录数
        /// </summary>
        /// <param name="predicate">查询的条件</param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return Entities.Count(predicate);
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">更新的对象</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        public bool Update(T entity, bool isSave = true)
        {
            try
            {
                //Entities.Attach(entity);
                GetEntity(entity).State = System.Data.Entity.EntityState.Modified;
                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
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
            try
            {
                Entities.Attach(entity);
                var entitystaus = ((IObjectContextAdapter)dbContext).ObjectContext.ObjectStateManager.GetObjectStateEntry(entity);
                foreach (object name in colnames)
                {
                    entitystaus.SetModifiedProperty(name.ToString());
                }
                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        public bool Update(T entity, Expression<Func<T, object>> propertyExpression, bool isSave = true)
        {
            try
            {
                Entities.Attach(entity);
                var entitystaus = ((IObjectContextAdapter)dbContext).ObjectContext.ObjectStateManager.GetObjectStateEntry(entity);

                ReadOnlyCollection<MemberInfo> memberInfos = ((dynamic)propertyExpression.Body).Members;//解析投影集合,返回成员名称
                foreach (var memberInfo in memberInfos)
                {
                    entitystaus.SetModifiedProperty(memberInfo.Name);
                }
                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <param name="entities">更新的列表对象</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        public bool Update(IEnumerable<T> entities, bool isSave = true)
        {
            try
            {
                foreach (var entity in entities)
                {
                    GetEntity(entity).State = System.Data.Entity.EntityState.Modified;
                    if (GetEntity(entity).State != System.Data.Entity.EntityState.Modified)
                    {
                        Entities.Attach(entity);
                        GetEntity(entity).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">删除的对象</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        public bool Delete(T entity, bool isSave = true)
        {
            try
            {
                Entities.Attach(entity);
                //Entities.Remove(entity);//真删
                ((IObjectContextAdapter)dbContext).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Modified);//更新状态
                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <param name="entities">删除的对象列表</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        public bool Delete(IEnumerable<T> entities, bool isSave = true)
        {
            try
            {
                foreach (T entity in entities)
                {
                    Entities.Attach(entity);
                    //Entities.Remove(entity);//真删
                    ((IObjectContextAdapter)dbContext).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Modified);//更新状态
                }
                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }

        }

        /// <summary>
        /// 删除指定条件的数据
        /// </summary>
        /// <param name="whereLambda">条件筛选</param>
        /// <param name="isSave">是否实时保存，如果false，最终需要调用SaveChange()来保存</param>
        /// <returns></returns>
        public bool Delete(Expression<Func<T, bool>> whereLambda, bool isSave = true)
        {
            try
            {
                IEnumerable<T> entities = LoadEntities(whereLambda);

                var v = entities.ToList();

                foreach (T entity in entities)
                {
                    Entities.Remove(entity);//真删

                    #region 假删：通过反射，对字段：Status进行赋值
                    //Type entityType = entity.GetType();
                    //PropertyInfo[] proInfo = entityType.GetProperties();
                    //foreach (var item in proInfo)
                    //{
                    //    if (item.Name == "Status")
                    //    {
                    //        item.SetValue(entity, 1);
                    //    }
                    //}
                    //((IObjectContextAdapter)dbContext).ObjectContext.ObjectStateManager.ChangeObjectState(entity, System.Data.Entity.EntityState.Modified);//假删
                    #endregion
                }
                return isSave ? SaveChange() : true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 根据条件查询返回列表数据
        /// </summary>
        /// <param name="whereLambda">条件筛选</param>
        /// <returns></returns>
        public IQueryable<T> LoadEntities(Expression<Func<T, bool>> whereLambda = null)
        {
            try
            {
                if (whereLambda == null)
                    return Entities.AsQueryable();
                else
                    return Entities.Where<T>(whereLambda).AsQueryable();//AsNoTracking(),这个方法可以解决刷新时，同一个上下文，查询时，重新获取数据库
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 查询是否存在记录
        /// </summary>
        /// <param name="anyLambda">条件筛选</param>
        /// <returns></returns>
        public bool Exist(Expression<Func<T, bool>> anyLambda)
        {
            try
            {
                //查询当前上下文中是否存在（非数据库中）
                if (!Entities.Local.AsQueryable<T>().Any(anyLambda))
                {
                    //查询数据库中是否存在
                    return Entities.Any(anyLambda);
                }
                return true;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return false;
        }

        /// <summary>
        /// 根据主键ID获取模型数据【优先使用】
        /// </summary>
        /// <param name="ID">主键</param>
        /// <returns>实体</returns>
        public T Find(string ID)
        {
            try
            {
                return Entities.Find(ID);
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// 查询数据【请优先使用Find(int ID)】
        /// </summary>
        /// <param name="whereLambda">查询表达式</param>
        /// <returns>实体</returns>
        public T Find(Expression<Func<T, bool>> whereLambda)
        {
            try
            {
                T _entity = Entities.FirstOrDefault<T>(whereLambda);
                return _entity;
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return null;
        }

        /// <summary>
        /// 执行SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="cmd">Sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public int ExecuteSqlCommand(string cmd, params SqlParameter[] parameters)
        {
            try
            {
                return dbContext.Database.ExecuteSqlCommand(cmd, parameters);
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 执行SQL语句查询
        /// </summary>
        /// <param name="cmd">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public IQueryable<T> ExecuteSqlSearch(string cmd, params SqlParameter[] parameters)
        {
            try
            {
                return dbContext.Database.SqlQuery<T>(cmd, parameters).AsQueryable();
            }
            catch (DbEntityValidationException dbEx)
            {
                Log.Error(dbEx);
                throw dbEx;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

    }
}
