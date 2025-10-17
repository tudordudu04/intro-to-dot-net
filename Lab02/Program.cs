using Lab02;

var books = new List<Book>();

Console.WriteLine("Enter book details (Title, Author, YearPublished). Type 'done' to finish.");
while (true)
{
    Console.Write("Title: ");
    var title = Console.ReadLine();
    if (title?.ToLower() == "done") break;

    Console.Write("Author: ");
    var author = Console.ReadLine();

    Console.Write("Year Published: ");
    if (!int.TryParse(Console.ReadLine(), out int yearPublished))
    {
        Console.WriteLine("Invalid year. Try again.");
        continue;
    }

    books.Add(new Book(title, author, yearPublished));
}

Console.WriteLine("");
Console.WriteLine("All entered books:");

foreach (var book in books)
{
    Console.WriteLine($"{book.Title} by {book.Author} ({book.YearPublished})");
}

var borrower1 = new Borrower(1, "Alice", new List<Book>());
var borrower2 = borrower1 with { BorrowedBooks = new List<Book>(borrower1.BorrowedBooks) { books.FirstOrDefault() } };

Console.WriteLine("");
Console.WriteLine($"Original Borrower: {borrower1.Name}, No. of books borrowed: {borrower1.BorrowedBooks.Count}");
Console.WriteLine($"Cloned Borrower: {borrower2.Name}, No. of books borrowed: {borrower2.BorrowedBooks.Count}");

void DisplayInfo(object obj)
{
    switch (obj)
    {
        case Book b:
            Console.WriteLine($"Book: {b.Title}, Year: {b.YearPublished}");
            break;
        case Borrower br:
            Console.WriteLine($"Borrower: {br.Name}, Books borrowed: {br.BorrowedBooks.Count}");
            break;
        default:
            Console.WriteLine("Unknown type");
            break;
    }
}

Console.WriteLine("");
Console.WriteLine("Pattern Matching Info:");

DisplayInfo(books.FirstOrDefault());
DisplayInfo(borrower2);
DisplayInfo("Random String");

Console.WriteLine("");
Console.WriteLine("Books published after 2010:");

var recentBooks = books.Where(static b => b.YearPublished > 2010);
foreach (var rb in recentBooks)
{
    Console.WriteLine($"{rb.Title} ({rb.YearPublished})");
}
