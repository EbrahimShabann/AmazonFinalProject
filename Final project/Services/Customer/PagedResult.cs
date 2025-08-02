namespace Final_project.Services.Customer
{
   
        public class PagedResult<T>
        {
            public List<T> Items { get; set; } = new();
            public List<T> TotalItems { get; set; } = new();       
            public int TotalItemsCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)TotalItemsCount / PageSize);

    }

    
}
