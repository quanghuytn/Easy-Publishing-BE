using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.Pagination
{
    public class PaginatedResult<TEntity>
    (int pageIndex, int pageSize, int totalCount, IEnumerable<TEntity> data)
    where TEntity : class
    {
        public int PageIndex { get; } = pageIndex;
        public int PageSize { get; } = pageSize;
        public int TotalCount { get; } = totalCount;
        public IEnumerable<TEntity> Data { get; } = data;
    }

}
