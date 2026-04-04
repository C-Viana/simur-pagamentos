using MongoDB.Driver;

namespace simur_backend.Models.Pagination
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;

        public PaginationParams() { }

        public PaginationParams(int page, int size, string sort)
        {
            Page = page;
            Size = size;
            Sort = sort;
        }

        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string Sort { get; set; } = "ASC";

        public int PageSize
        {
            get => Size;
            set => Size = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string SortDirection
        {
            get => Sort = (!string.IsNullOrWhiteSpace(Sort) && Sort.StartsWith("desc", StringComparison.InvariantCultureIgnoreCase)) 
                ? "DESC" 
                : "ASC";
        }
    }
}
