namespace CommerceFlow.Application.Products
{
    public sealed class ProductQueryParameters
    {
        public string? Search { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string SortBy { get; set; } = "name";

        public string SortDirection { get; set; } = "asc";

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 20;
    }
}