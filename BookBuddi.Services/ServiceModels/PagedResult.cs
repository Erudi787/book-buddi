namespace BookBuddi.Services.ServiceModels
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedResult()
        {
        }

        public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public static PagedResult<T> Create(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }

        public static PagedResult<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var enumerable = source.ToList();
            var count = enumerable.Count;
            var items = enumerable.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<T>(items, count, pageNumber, pageSize);
        }
    }
}
