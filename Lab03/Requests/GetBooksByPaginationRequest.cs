namespace Lab03.Requests;

public record GetBooksByPaginationRequest(int Page, int PageSize);