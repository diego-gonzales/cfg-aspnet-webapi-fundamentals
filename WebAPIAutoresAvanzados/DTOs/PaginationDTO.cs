namespace WebAPIAutoresAvanzados;

public class PaginationDTO
{
    public int Page { get; set; } = 1;
    private int perPage = 10;
    private readonly int maxItemsPerPage = 50;

    public int PerPage
    {
        get { return perPage; }
        set { perPage = value > maxItemsPerPage ? maxItemsPerPage : value; }
    }
}
