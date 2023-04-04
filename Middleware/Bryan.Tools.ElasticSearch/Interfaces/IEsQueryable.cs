using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tools.Elastic
{
    public interface IEsQueryable<T>
    {
        IEsQueryable<T> Where(Expression<Func<T, bool>> expression);

        List<T> ToList();
        
        Task<T> FirstAsync();

        Task<List<T>> ToListAsync();
        List<T> ToPageList(int pageIndex, int pageSize);
        List<T> ToPageList(int pageIndex, int pageSize, out long totalNumber);
        IEsQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);

        IEsQueryable<T> GroupBy(Expression<Func<T, object>> expression);
    }
}