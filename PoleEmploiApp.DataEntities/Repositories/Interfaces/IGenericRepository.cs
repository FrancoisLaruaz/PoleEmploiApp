using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoleEmploiApp.DataEntities.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        string Test(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        IEnumerable<T> FindAllBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
        IEnumerable<T> List();
        IEnumerable<T> Edit(IEnumerable<T> entityList);
        PoleEmploiContext Context();

        void setLazyLoading(bool LazyLoadingEnabled);
        T Get(int id);
        int Count();
        T Get(Guid id);
        bool Add(T entity);
        bool Delete(T entity);
        bool Delete(int id);
        T Delete(Guid id);
        T Edit(T entity);

        bool Save();
        void Reload(T entity, string property);
        void ReloadCollection(T entity, string collection);


    }
}
