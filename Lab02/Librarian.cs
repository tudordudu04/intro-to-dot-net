namespace Lab02;

public class Librarian
{
    public string Name { get; init; }
    public string Email { get; init; }
    public string LibrarySection { get; init; }

    public Librarian(string name, string email, string section)
    {
        Name = name;
        Email = email;
        LibrarySection = section;
    }
}