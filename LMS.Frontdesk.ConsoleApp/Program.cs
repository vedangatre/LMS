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
    Console.WriteLine("Frontdesk Menu");
    Console.WriteLine("1. Issue book to student");
    Console.WriteLine("2. Return book from student");
    Console.WriteLine("3. View all books");
    Console.WriteLine("4. View issued books");
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
                IssueBookToStudent(bookService, studentService, issueService);
                break;
            case "2":
                ReturnBookFromStudent(issueService);
                break;
            case "3":
                ListBooks(bookService);
                break;
            case "4":
                ViewIssuedBooks(issueService);
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

static void IssueBookToStudent(BookService bookService, StudentService studentService, IssueService issueService)
{
    Console.Write("Student ID: ");
    if (!int.TryParse(Console.ReadLine(), out var studentId))
    {
        Console.WriteLine("Invalid student ID.");
        return;
    }

    var student = studentService.GetStudentById(studentId);
    if (student is null)
    {
        Console.WriteLine("Student not found.");
        return;
    }

    Console.WriteLine($"Student: {student.StudentName}");
    Console.WriteLine();

    ListBooks(bookService);

    Console.Write("Book ID to issue: ");
    if (!int.TryParse(Console.ReadLine(), out var bookId))
    {
        Console.WriteLine("Invalid book ID.");
        return;
    }

    var books = bookService.GetAllBooks();
    var book = books.FirstOrDefault(b => b.BookId == bookId);
    if (book is null)
    {
        Console.WriteLine("Book not found.");
        return;
    }

    if (book.AvailableCount <= 0)
    {
        Console.WriteLine("No available copies of this book.");
        return;
    }

    var issueId = issueService.IssueBook(studentId, bookId);
    Console.WriteLine($"Book issued successfully. Issue ID: {issueId}");
    Console.WriteLine($"Book: {book.BookName} | Available copies now: {book.AvailableCount - 1}");
}

static void ReturnBookFromStudent(IssueService issueService)
{
    Console.Write("Issue ID: ");
    if (!int.TryParse(Console.ReadLine(), out var issueId))
    {
        Console.WriteLine("Invalid issue ID.");
        return;
    }

    var issuedBooks = issueService.GetIssuedBooks();
    var issue = issuedBooks.FirstOrDefault(i => i.IssueId == issueId);
    if (issue is null)
    {
        Console.WriteLine("Issue not found or book already returned.");
        return;
    }

    var result = issueService.ReturnBook(issueId);
    if (result > 0)
    {
        Console.WriteLine("Book returned successfully.");
        Console.WriteLine($"Issue ID: {issue.IssueId} | Student ID: {issue.StudentId} | Copy ID: {issue.CopyId}");
    }
    else
    {
        Console.WriteLine("Failed to return book.");
    }
}

static void ListBooks(BookService bookService)
{
    var books = bookService.GetAllBooks();
    if (books.Count == 0)
    {
        Console.WriteLine("No books found.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Books in library:");
    foreach (var book in books)
    {
        var authors = book.AuthorNames.Count == 0 ? "(none)" : string.Join(", ", book.AuthorNames);
        Console.WriteLine($"{book.BookId}: {book.BookName} | Authors: {authors} | Issued: {book.IssuedCount} | Available: {book.AvailableCount}");
    }
    Console.WriteLine();
}

static void ViewIssuedBooks(IssueService issueService)
{
    var issuedBooks = issueService.GetIssuedBooks();
    if (issuedBooks.Count == 0)
    {
        Console.WriteLine("No books currently issued.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Currently Issued Books:");
    foreach (var issue in issuedBooks)
    {
        Console.WriteLine($"Issue ID: {issue.IssueId} | Student ID: {issue.StudentId} | Copy ID: {issue.CopyId} | Issue Date: {issue.IssueDate:yyyy-MM-dd}");
    }
    Console.WriteLine();
}
