namespace Ecom.API.Helper
{
    public class Pagination<T> where T : class
    {

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Data { get; set; }
        public Pagination(int pageNumber, int pageSize, int totalCount, IEnumerable<T> data)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = data;
        }



    }
}
