using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoleEmploiApp.DataEntities.Repositories.Interfaces
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        // dotnet ef dbcontext scaffold "Server=MSI;Database=Doctor;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o ColibNodel 

        protected PoleEmploiContext _context;

        public GenericRepository(PoleEmploiContext context)
        {
            _context = context;

        }

        public PoleEmploiContext Context()
        {
            return this._context;
        }

        public void setLazyLoading(bool LazyLoadingEnabled)
        {
            //  this._context.Configuration.LazyLoadingEnabled = LazyLoadingEnabled;
        }

        public virtual IEnumerable<T> List()
        {
            return _context.Set<T>().ToList();
        }

        public string Test(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            string result = "ok";
            try
            {
                IEnumerable<T> query = _context.Set<T>().Where(predicate);
                result = result + " : " + query.Count();
            }
            catch (Exception e)
            {
                result = e.ToString();
            }
            return result;
        }


        public IEnumerable<T> FindAllBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            try
            {
                IEnumerable<T> query = _context.Set<T>().Where(predicate);
                return query;
            }
            catch
            {
            }
            return new List<T>();
        }

        public virtual bool Add(T entity)
        {
            bool result = true;
            try
            {
                _context.Set<T>().Add(entity);
            }
            catch 
            {
                result = false;
            }
            return result;
        }

        public virtual bool Delete(T entity)
        {
            bool result = true;
            try
            {
                if (entity != null)
                {
                    _context.Set<T>().Remove(entity);
                }

            }
            catch 
            {
                result = false;
            }
            return result;
        }

        public virtual bool Delete(int id)
        {
            bool result = true;
            try
            {
                _context.Set<T>().Remove(Get(id));
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public virtual T Delete(Guid id)
        {
            try
            {
                // return _context.Set<T>().Remove(entity);
            }
            catch
            {

            }
            return null;
        }

        public virtual T Edit(T entity)
        {
            try
            {
                _context.Entry(entity).CurrentValues.SetValues(entity);
                return entity;
            }
            catch 
            {

            }
            return null;
        }

        public virtual IEnumerable<T> Edit(IEnumerable<T> entityList)
        {
            try
            {
                foreach (T entity in entityList)
                {
                    _context.Entry(entity).CurrentValues.SetValues(entity);
                }
                return entityList;
            }
            catch
            {
          
            }
            return null;
        }

        public virtual bool Save()
        {
            bool result = true;
            try
            {
                _context.SaveChanges();
            }
            catch (ValidationException e)
            {
                result = false;
                var sb = new StringBuilder();
                sb.AppendLine($"DbUpdateException error details - {e?.InnerException?.InnerException?.Message}");
                sb.AppendLine("ValidationResult = " + e.ValidationResult);
             //   Logger.Logger.GenerateError(e, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, sb.ToString());
            }
            catch (DbUpdateException e)
            {
                result = false;
                var sb = new StringBuilder();
                sb.AppendLine($"DbUpdateException error details - {e?.InnerException?.InnerException?.Message}");

                foreach (var eve in e.Entries)
                {
                    sb.AppendLine($"| Entity of type {eve.Entity.GetType().Name} in state {eve.State} could not be updated.");
                }
            }
            catch 
            {
                result = false;
            }
            return result;
        }




        public virtual T Get(int id)
        {
            T m = null;
            try
            {
                m = _context.Set<T>().Find(id);
            }
            catch 
            {
               
            }
            return m;
        }



        public virtual int Count()
        {

            return _context.Set<T>().Count(); 
        }

        public virtual T Get(Guid id)
        {
            T m = null;

            return m;
        }

        public virtual void Reload(T entity, string property)
        {
            _context.Entry(entity).Reference(property).Load();
        }

        public virtual void ReloadCollection(T entity, string collection)
        {

            _context.Entry(entity).Collection(collection).Load();

        }

    }
}
