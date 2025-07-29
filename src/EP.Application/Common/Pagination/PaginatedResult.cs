namespace EP.Application.Common.Pagination
{
    public class PaginatedResult<TEntity>
    (int pageIndex, int pageSize, int totalCount, IEnumerable<TEntity> data)
    where TEntity : class
    {
        public int Current { get; } = pageIndex;
        public int PageSize { get; } = pageSize;
        public int Total { get; } = totalCount;
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
        public IEnumerable<TEntity> List { get; } = data;
    }

}
