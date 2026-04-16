using LMS.Core.Repository.Implementation;
using LMS.Core.Repository.Interfaces;
using LMS.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("LmsDb")!;
        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookAuthorRepository, BookAuthorRepository>();
        services.AddScoped<IBookCopyRepository, BookCopyRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IIssueRepository, IssueRepository>();

        services.AddScoped<BookService>();
        services.AddScoped<StudentService>();
        services.AddScoped<IssueService>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var bookService = scope.ServiceProvider.GetRequiredService<BookService>();
var studentService = scope.ServiceProvider.GetRequiredService<StudentService>();
var issueService = scope.ServiceProvider.GetRequiredService<IssueService>();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Librarian Menu");
    Console.WriteLine("1. Add new book");
    Console.WriteLine("2. Delete book");
    Console.WriteLine("3. Add new copy");
    Console.WriteLine("4. Delete copy");
    Console.WriteLine("5. Add student");
    Console.WriteLine("6. Delete student");
    Console.WriteLine("7. View all books");
    Console.WriteLine("8. View issued books count");
    Console.WriteLine("0. Exit");
    Console.Write("Select option: ");

    var input = Console.ReadLine();
    if (string.Equals(input, "0", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    try
    {
        switch (input)
        {
            case "1":
                AddBook(bookService);
                break;
            case "2":
                DeleteBook(bookService);
                break;
            case "3":
                AddCopies(bookService);
                break;
            case "4":
                RemoveCopies(bookService);
                break;
            case "5":
                AddStudent(studentService);
                break;
            case "6":
                DeleteStudent(studentService);
                break;
            case "7":
                ListBooks(bookService);
                break;
            case "8":
                ShowIssuedBooks(issueService);
                break;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static void AddBook(BookService bookService)
{
    Console.Write("Book name: ");
    var name = Console.ReadLine();

    Console.Write("Author names (comma separated): ");
    var authorsInput = Console.ReadLine() ?? string.Empty;
    var authorNames = authorsInput
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(a => (object)a)
        .ToList();

    var created = bookService.AddBook(name, authorNames);
    if (created is null)
    {
        Console.WriteLine("Book already exists with the same authors.");
        return;
    }

    Console.WriteLine($"Added book with ID {created.BookId}.");
}

static void DeleteBook(BookService bookService)
{
    Console.Write("Book ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out var bookId))
    {
        Console.WriteLine("Invalid book ID.");
        return;
    }

    bookService.Delete(bookId);
    Console.WriteLine("Book deleted.");
}

static void AddCopies(BookService bookService)
{
    Console.Write("Book ID: ");
    if (!int.TryParse(Console.ReadLine(), out var bookId))
    {
        Console.WriteLine("Invalid book ID.");
        return;
    }

    Console.Write("Number of copies to add: ");
    if (!int.TryParse(Console.ReadLine(), out var count))
    {
        Console.WriteLine("Invalid count.");
        return;
    }

    bookService.AddCopies(bookId, count);
    Console.WriteLine("Copies added.");
}

static void RemoveCopies(BookService bookService)
{
    Console.Write("Book ID: ");
    if (!int.TryParse(Console.ReadLine(), out var bookId))
    {
        Console.WriteLine("Invalid book ID.");
        return;
    }

    Console.Write("Number of copies to remove: ");
    if (!int.TryParse(Console.ReadLine(), out var count))
    {
        Console.WriteLine("Invalid count.");
        return;
    }

    bookService.RemoveCopies(bookId, count);
    Console.WriteLine("Copies removed.");
}

static void AddStudent(StudentService studentService)
{
    Console.Write("Student name: ");
    var name = Console.ReadLine();

    var student = studentService.AddStudent(name ?? string.Empty);
    Console.WriteLine($"Student added with ID {student.StudentId}.");
}

static void DeleteStudent(StudentService studentService)
{
    Console.Write("Student ID to delete: ");
    if (!int.TryParse(Console.ReadLine(), out var studentId))
    {
        Console.WriteLine("Invalid student ID.");
        return;
    }

    var deleted = studentService.DeleteStudent(studentId);
    Console.WriteLine(deleted ? "Student deleted." : "Student not found.");
}

static void ListBooks(BookService bookService)
{
    var books = bookService.GetAllBooks();
    if (books.Count == 0)
    {
        Console.WriteLine("No books found.");
        return;
    }

    Console.WriteLine("Books in library:");
    foreach (var book in books)
    {
        var authors = book.AuthorNames.Count == 0 ? "(none)" : string.Join(", ", book.AuthorNames);
        Console.WriteLine($"{book.BookId}: {book.BookName} | Authors: {authors} | Issued: {book.IssuedCount} | Available: {book.AvailableCount}");
    }
}

static void ShowIssuedBooks(IssueService issueService)
{
    var issuedBooks = issueService.GetIssuedBooks();
    Console.WriteLine($"Issued books count: {issuedBooks.Count}");
}
