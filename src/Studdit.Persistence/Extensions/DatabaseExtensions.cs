using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Studdit.Domain.Entities;
using Studdit.Domain.ValueObjects;
using Studdit.Persistence.Context;

namespace Studdit.Persistence.Extensions
{
    /// <summary>
    /// Database initialization and seeding extensions
    /// </summary>
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                logger.LogInformation("Starting database migration...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migration completed successfully.");

                logger.LogInformation("Starting database seeding...");
                await SeedAsync(context, logger);
                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }

        public static async Task<bool> CanConnectAsync(this ApplicationDbContext context)
        {
            try
            {
                return await context.Database.CanConnectAsync();
            }
            catch
            {
                return false;
            }
        }

        private static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
        {
            // Seed Tags first (no dependencies)
            await SeedTagsAsync(context, logger);

            // Seed Users
            await SeedUsersAsync(context, logger);

            // Seed Questions (depends on Users and Tags)
            await SeedQuestionsAsync(context, logger);

            // Seed Answers (depends on Users and Questions)
            await SeedAnswersAsync(context, logger);

            await context.SaveChangesAsync();
        }

        private static async Task SeedTagsAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Tags.AnyAsync())
                return;

            logger.LogInformation("Seeding tags...");

            var tags = new List<Tag>
            {
                Tag.Create("csharp", "C# programming language and .NET development"),
                Tag.Create("typescript", "TypeScript programming language and related technologies"),
                Tag.Create("angular", "Angular framework for building web applications"),
                Tag.Create("dotnet", ".NET framework and .NET Core development"),
                Tag.Create("entityframework", "Entity Framework and Entity Framework Core ORM"),
                Tag.Create("javascript", "JavaScript programming language"),
                Tag.Create("sql", "SQL database queries and database design"),
                Tag.Create("webapi", "ASP.NET Core Web API development"),
                Tag.Create("authentication", "User authentication and authorization"),
                Tag.Create("database", "Database design and management")
            };

            await context.Tags.AddRangeAsync(tags);
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Users.AnyAsync())
                return;

            logger.LogInformation("Seeding users...");

            var users = new List<User>
            {
                User.Create(
                    "admin",
                    Email.Create("admin@studdit.com"),
                    "AQAAAAEAACcQAAAAEH8VJqo8BCrypt+Password+Hash+Example+1",
                    "Administrator"
                ),
                User.Create(
                    "john_doe",
                    Email.Create("john@example.com"),
                    "AQAAAAEAACcQAAAAEH8VJqo8BCrypt+Password+Hash+Example+2",
                    "John Doe"
                ),
                User.Create(
                    "jane_smith",
                    Email.Create("jane@example.com"),
                    "AQAAAAEAACcQAAAAEH8VJqo8BCrypt+Password+Hash+Example+3",
                    "Jane Smith"
                ),
                User.Create(
                    "dev_user",
                    Email.Create("dev@example.com"),
                    "AQAAAAEAACcQAAAAEH8VJqo8BCrypt+Password+Hash+Example+4",
                    "Development User"
                )
            };

            // Give admin high reputation
            users[0].AddReputation(1999);

            // Give some users moderate reputation
            users[1].AddReputation(499);
            users[2].AddReputation(299);

            await context.Users.AddRangeAsync(users);
        }

        private static async Task SeedQuestionsAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Questions.AnyAsync())
                return;

            logger.LogInformation("Seeding questions...");

            var users = await context.Users.ToListAsync();
            var tags = await context.Tags.ToListAsync();

            if (!users.Any() || !tags.Any())
                return;

            var csharpTag = tags.First(t => t.Name == "csharp");
            var dotnetTag = tags.First(t => t.Name == "dotnet");
            var angularTag = tags.First(t => t.Name == "angular");
            var typescriptTag = tags.First(t => t.Name == "typescript");
            var efTag = tags.First(t => t.Name == "entityframework");

            var questions = new List<Question>
            {
                Question.Create(
                    "How to implement CQRS pattern in .NET Core?",
                    "I'm trying to implement the CQRS pattern in my .NET Core application using MediatR. Can someone provide guidance on how to structure the commands, queries, and handlers properly? I want to ensure I'm following best practices for separation of concerns.",
                    users[1] // john_doe
                ),
                Question.Create(
                    "Entity Framework Core vs Dapper for complex queries",
                    "I'm working on a large-scale application and trying to decide between Entity Framework Core and Dapper for database operations. The application involves complex queries with multiple joins and aggregations. What are the performance implications and when should I choose one over the other?",
                    users[2] // jane_smith
                ),
                Question.Create(
                    "Angular Reactive Forms validation patterns",
                    "What are the best practices for implementing custom validators in Angular Reactive Forms? I need to validate complex business rules and show appropriate error messages. Should I use template-driven or reactive forms for complex scenarios?",
                    users[3] // dev_user
                ),
                Question.Create(
                    "TypeScript generic constraints with multiple interfaces",
                    "I'm trying to create a generic TypeScript function that accepts objects implementing multiple interfaces. How can I properly constrain the generic type parameter to ensure type safety while maintaining flexibility?",
                    users[1] // john_doe
                )
            };

            // Add tags to questions
            questions[0].AddTag(csharpTag);
            questions[0].AddTag(dotnetTag);

            questions[1].AddTag(csharpTag);
            questions[1].AddTag(efTag);
            questions[1].AddTag(dotnetTag);

            questions[2].AddTag(angularTag);
            questions[2].AddTag(typescriptTag);

            questions[3].AddTag(typescriptTag);

            await context.Questions.AddRangeAsync(questions);
        }

        private static async Task SeedAnswersAsync(ApplicationDbContext context, ILogger logger)
        {
            if (await context.Answers.AnyAsync())
                return;

            logger.LogInformation("Seeding answers...");

            var users = await context.Users.ToListAsync();
            var questions = await context.Questions.ToListAsync();

            if (!users.Any() || !questions.Any())
                return;

            var answers = new List<Answer>
            {
                Answer.Create(
                    "To implement CQRS properly in .NET Core with MediatR, you should separate your commands and queries into different namespaces. Create separate command and query handlers, and use the IRequest<T> interface for queries and IRequest for commands. Here's a basic structure: [Command/Query] -> [Handler] -> [Repository/DbContext]. Make sure to validate your commands using FluentValidation and implement proper error handling.",
                    users[0], // admin
                    questions[0]
                ),
                Answer.Create(
                    "For complex queries, I'd recommend a hybrid approach. Use Entity Framework Core for CRUD operations and simple queries, but switch to Dapper for complex reporting queries with multiple joins and aggregations. EF Core is great for maintainability and rapid development, while Dapper gives you full control over SQL and better performance for complex scenarios. You can use both in the same project.",
                    users[0], // admin
                    questions[1]
                ),
                Answer.Create(
                    "For Angular Reactive Forms with complex validation, create custom validators as functions that return ValidationErrors or null. You can compose multiple validators using the Validators.compose() method. For async validation (like checking if username exists), use async validators. Always use reactive forms for complex scenarios as they provide better control and testability.",
                    users[2], // jane_smith
                    questions[2]
                )
            };

            await context.Answers.AddRangeAsync(answers);
        }
    }
}
