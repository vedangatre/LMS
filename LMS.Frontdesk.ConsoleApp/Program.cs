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
    Console.WriteLine("=== Frontdesk Operations ===");
    Console.WriteLine("1. Issue book to student");
    Console.WriteLine("2. Return book from student");
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
                ReturnBookFromStudent(bookService, issueService);
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
    Console.WriteLine();
    Console.WriteLine("--- Issue Book to Student ---");

    Console.Write("Enter Student ID: ");
    if (!int.TryParse(Console.ReadLine(), out var studentId))
    {
        Console.WriteLine("❌ Invalid student ID.");
        return;
    }

    var student = studentService.GetStudentById(studentId);
    if (student is null)
    {
        Console.WriteLine("❌ Student not found.");
        return;
    }

    Console.WriteLine($"✓ Student Found: {student.StudentName}");
    Console.WriteLine();

    var allBooks = bookService.GetAllBooks();
    var availableBooks = allBooks.Where(b => b.AvailableCount > 0).ToList();

    if (availableBooks.Count == 0)
    {
        Console.WriteLine("❌ No books available for issue.");
        return;
    }

    Console.WriteLine("Available Books:");
    foreach (var book in availableBooks)
    {
        var authors = book.AuthorNames.Count == 0 ? "(none)" : string.Join(", ", book.AuthorNames);
        Console.WriteLine($"  ID: {book.BookId} | {book.BookName} | Authors: {authors} | Available: {book.AvailableCount}");
    }
    Console.WriteLine();

    Console.Write("Enter Book ID to issue: ");
    if (!int.TryParse(Console.ReadLine(), out var bookId))
    {
        Console.WriteLine("❌ Invalid book ID.");
        return;
    }

    var selectedBook = allBooks.FirstOrDefault(b => b.BookId == bookId);
    if (selectedBook is null)
    {
        Console.WriteLine("❌ Book not found.");
        return;
    }

    if (selectedBook.AvailableCount <= 0)
    {
        Console.WriteLine("❌ No available copies of this book.");
        return;
    }

    try
    {
        var issueId = issueService.IssueBook(studentId, bookId);

        // Get updated book info to show new counts
        var updatedBooks = bookService.GetAllBooks();
        var updatedBook = updatedBooks.FirstOrDefault(b => b.BookId == bookId)!;

        Console.WriteLine();
        Console.WriteLine("✓ Book Issued Successfully!");
        Console.WriteLine($"  Issue ID: {issueId}");
        Console.WriteLine($"  Book: {selectedBook.BookName}");
        Console.WriteLine($"  Student ID: {studentId}");
        Console.WriteLine($"  Student: {student.StudentName}");
        Console.WriteLine();
        Console.WriteLine("Updated Copy Counts:");
        Console.WriteLine($"  Total Copies: {updatedBook.AvailableCount + updatedBook.IssuedCount}");
        Console.WriteLine($"  Issued Copies: {updatedBook.IssuedCount}");
        Console.WriteLine($"  Available Copies: {updatedBook.AvailableCount}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Failed to issue book: {ex.Message}");
    }
}

static void ReturnBookFromStudent(BookService bookService, IssueService issueService)
{
    Console.WriteLine();
    Console.WriteLine("--- Return Book from Student ---");

    var issuedBooks = issueService.GetIssuedBooks();
    if (issuedBooks.Count == 0)
    {
        Console.WriteLine("❌ No books currently issued.");
        return;
    }

    Console.WriteLine("Currently Issued Books:");
    foreach (var issuedBook in issuedBooks)
    {
        Console.WriteLine($"  Issue ID: {issuedBook.IssueId} | Student ID: {issuedBook.StudentId} | Copy ID: {issuedBook.CopyId} | Issued: {issuedBook.IssueDate:yyyy-MM-dd}");
    }
    Console.WriteLine();

    Console.Write("Enter Issue ID to return: ");
    if (!int.TryParse(Console.ReadLine(), out var issueId))
    {
        Console.WriteLine("❌ Invalid issue ID.");
        return;
    }

    var issue = issuedBooks.FirstOrDefault(i => i.IssueId == issueId);
    if (issue is null)
    {
        Console.WriteLine("❌ Issue not found or book already returned.");
        return;
    }

    try
    {
        var result = issueService.ReturnBook(issueId);
        if (result > 0)
        {
            Console.WriteLine();
            Console.WriteLine("✓ Book Returned Successfully!");
            Console.WriteLine($"  Issue ID: {issue.IssueId}");
            Console.WriteLine($"  Copy ID: {issue.CopyId}");
            Console.WriteLine($"  Student ID: {issue.StudentId}");
            Console.WriteLine($"  Issue Date: {issue.IssueDate:yyyy-MM-dd}");
            Console.WriteLine($"  Return Date: {DateTime.Now:yyyy-MM-dd}");
            Console.WriteLine();

            // Show updated counts - refresh data
            var updatedIssuedBooks = issueService.GetIssuedBooks();
            Console.WriteLine($"Current Status:");
            Console.WriteLine($"  Total Issued Books: {updatedIssuedBooks.Count}");
        }
        else
        {
            Console.WriteLine("❌ Failed to return book.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error returning book: {ex.Message}");
    }
}
