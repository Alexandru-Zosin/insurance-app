namespace Application.Common;

public sealed record PageRequest
{
    public const int MaxPageSize = 50;

    public int PageNumber { get; }
    public int PageSize { get; }

    public PageRequest(int page = 1, int pageSize = 25)
    {
        PageNumber = page < 1 ? 1 : page;
        PageSize = pageSize < 1 ? 1
               : pageSize > MaxPageSize ? MaxPageSize
               : pageSize;
    }

    public int Offset => (PageNumber - 1) * PageSize;
    public int Take => PageSize;
}