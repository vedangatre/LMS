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

        services.AddScoped<BookService>();
    })
    .Build();

using var scope = host.Services.CreateScope();
var bookService = scope.ServiceProvider.GetRequiredService<BookService>();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("Student Menu");
    Console.WriteLine("1. Search book by name");
    Console.WriteLine("2. Search book by author");
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
                SearchByBookName(bookService);
                break;
            case "2":
                SearchByAuthorName(bookService);
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

static void SearchByBookName(BookService bookService)
{
    Console.Write("Book name: ");
    var name = Console.ReadLine();

    var results = bookService.SearchByBookName(name ?? string.Empty);
    PrintBooks(results);
}

static void SearchByAuthorName(BookService bookService)
{
    Console.Write("Author name: ");
    var name = Console.ReadLine();

    var results = bookService.SearchByAuthorName(name ?? string.Empty);
    PrintBooks(results);
}

static void PrintBooks(List<LMS.Core.Models.Book> books)
{
    if (books.Count == 0)
    {
        Console.WriteLine("No books found.");
        return;
    }

    Console.WriteLine("Books:");
    foreach (var book in books)
    {
        Console.WriteLine($"{book.BookId}: {book.BookName}");
    }
}
