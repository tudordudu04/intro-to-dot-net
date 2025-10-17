namespace Lab03.Requests;

public record UpdateBookRequest(int Id, string Title, string Author, int Year);