using DbmlRepositoryGenerator;
using DbmlRepositoryGenerator.Generator;
using DbmlRepositoryGenerator.Infrastructure;
using DbmlRepositoryGenerator.Infrastructure.Enum;
using DbmlRepositoryGenerator.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;

namespace SampleUsage
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            // Setup DI
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
            services.AddTransient<DbmlParser>();
            services.AddTransient<RepositoryGenerator>();

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            // Example 1: Generate from DBML file  
            // Use first argument as targetNamespace if provided, otherwise use a default
            //var targetNamespace = args.Length > 0 ? args[0] : "SampleNamespace";
            //await GenerateFromFileExample(targetNamespace);

            //// Example 2: Generate from DBML content  
            //await GenerateFromContentExample();  

            //// Example 3: Use generated code in DI container  
            ////await UsageExample();  

            //// Example 4: Create custom templates  
            //await CreateCustomTemplatesExample();  

            // Setup CLI
            var inputOption = new Option<FileInfo>(
                aliases: new[] { "--input", "-i" },
                description: "Path to the DBML schema file")
            {
                IsRequired = true
            };

            var outputOption = new Option<DirectoryInfo>(
                aliases: new[] { "--output", "-o" },
                description: "Output directory for generated files")
            {
                IsRequired = true
            };

            var namespaceOption = new Option<string>(
                aliases: new[] { "--namespace", "-n" },
                description: "Namespace for generated code",
                getDefaultValue: () => "Generated");

            var overwriteOption = new Option<bool>(
                aliases: new[] { "--overwrite" },
                description: "Overwrite existing files",
                getDefaultValue: () => true);

            var rootCommand = new RootCommand("Generate Repository and Unit of Work pattern from DBML files")
            {
                inputOption,
                outputOption,
                namespaceOption,
                overwriteOption
            };

            rootCommand.SetHandler(async (input, output, namespaceName, overwrite) =>
            {
                try
                {
                    logger.LogInformation("Starting DBML Repository Generator");
                    logger.LogInformation("Input: {Input}", input.FullName);
                    logger.LogInformation("Output: {Output}", output.FullName);
                    logger.LogInformation("Namespace: {Namespace}", namespaceName);

                    var parser = serviceProvider.GetRequiredService<DbmlParser>();
                    
                    await GenerateFromFileExample(namespaceName, input.FullName, output.FullName);


                    logger.LogInformation("✅ Generation completed successfully!");
                    logger.LogInformation("📁 Files generated in: {OutputPath}", output.FullName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ Generation failed");
                    Environment.Exit(1);
                }
            }, inputOption, outputOption, namespaceOption, overwriteOption);

            return await rootCommand.InvokeAsync(args);
        }

        private static async Task GenerateFromFileExample(string targetNamespace, string input, string output)
        {
            Console.WriteLine("=== Generating from DBML file ===");

            var options = new GeneratorOptions
            {
                Namespace = targetNamespace,
                OutputDirectory = output,
                TemplateDirectory = "./Templates",
                GenerateAsync = true,
                GenerateInterfaces = true,
                Provider = DatabaseProvider.SqlServer,
                CustomVariables = new Dictionary<string, object>
                {
                    { "author", "DBML Repository Generator" },
                    { "version", "1.0.0" },
                    { "generate_swagger_docs", true }
                }
            };

            var generator = new DbmlRepositoryGeneratorFacade();
            await generator.GenerateFromFileAsync(input, options);

            Console.WriteLine("Generated files:");
            if (Directory.Exists("./Generated"))
            {
                foreach (var file in Directory.GetFiles("./Generated", "*.cs"))
                {
                    Console.WriteLine($"  - {Path.GetFileName(file)}");
                }
            }
        }

        private static async Task GenerateFromContentExample()
        {
            Console.WriteLine("\n=== Generating from DBML content ===");

            var dbmlContent = @"
Table products {
  id int [pk, increment]
  name varchar(255) [not null]
  description text
  price decimal(10,2) [not null]
  category_id int
  created_at datetime [not null, default: 'now()']
}

Table categories {
  id int [pk, increment]
  name varchar(100) [not null, unique]
  description text
}

Ref: products.category_id > categories.id
";

            var options = new GeneratorOptions
            {
                Namespace = "ECommerce",
                OutputDirectory = "./ECommerce.Generated",
                TemplateDirectory = "./Templates",
                GenerateAsync = true,
                GenerateInterfaces = true,
                Provider = DatabaseProvider.SqlServer
            };

            var generator = new DbmlRepositoryGeneratorFacade();
            await generator.GenerateFromContentAsync(dbmlContent, options);

            Console.WriteLine("E-Commerce entities generated successfully!");
        }

        private static async Task UsageExample()
        {
            Console.WriteLine("\n=== Usage Example with DI Container ===");

            // This shows how you would use the generated code
            var services = new ServiceCollection();

            //// Add Entity Framework
            //services.AddDbContext<BlogApp.ApplicationDbContext>(options =>
            //    options.UseSqlServer("Server=localhost;Database=BlogApp;Trusted_Connection=true;"));

            //// Add repositories and unit of work
            //services.AddScoped<BlogApp.IUnitOfWork, BlogApp.UnitOfWork>();
            //services.AddScoped<BlogApp.Repositories.IUsersRepository, BlogApp.Repositories.UsersRepository>();
            //services.AddScoped<BlogApp.Repositories.IPostsRepository, BlogApp.Repositories.PostsRepository>();
            //services.AddScoped<BlogApp.Repositories.ICommentsRepository, BlogApp.Repositories.CommentsRepository>();

            // Add logging
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("Services configured for BlogApp");
            Console.WriteLine("Available services:");
            Console.WriteLine("  - ApplicationDbContext");
            Console.WriteLine("  - IUnitOfWork");
            Console.WriteLine("  - IUsersRepository");
            Console.WriteLine("  - IPostsRepository");
            Console.WriteLine("  - ICommentsRepository");
        }

        private static async Task CreateCustomTemplatesExample()
        {
            Console.WriteLine("\n=== Creating Custom Templates ===");

            await TemplateHelper.CreateCustomTemplatesAsync("./CustomTemplates");

            Console.WriteLine("Custom templates created in ./CustomTemplates/");
            Console.WriteLine("Available templates:");
            Console.WriteLine("  - entity.liquid (Enhanced with documentation)");
            Console.WriteLine("  - repository-interface.liquid (With logging support)");
            Console.WriteLine("  - repository.liquid (With error handling)");
            Console.WriteLine("  - unitofwork-interface.liquid (With transactions)");
            Console.WriteLine("  - unitofwork.liquid (Full transaction support)");
            Console.WriteLine("  - dbcontext.liquid (Enhanced configuration)");
            Console.WriteLine("  - dto.liquid (Create/Update DTOs)");
            Console.WriteLine("  - service-interface.liquid (Business logic layer)");
            Console.WriteLine("  - service.liquid (Service implementation)");

            // Now use custom templates
            var options = new GeneratorOptions
            {
                Namespace = "AdvancedBlog",
                OutputDirectory = "./AdvancedGenerated",
                TemplateDirectory = "./CustomTemplates",
                GenerateAsync = true,
                GenerateInterfaces = true,
                Provider = DatabaseProvider.SqlServer
            };

            var dbmlContent = @"
Table articles {
  id int [pk, increment]
  title varchar(255) [not null]
  slug varchar(255) [not null, unique]
  content text [not null]
  excerpt varchar(500)
  author_id int [not null]
  status varchar(20) [not null, default: 'draft']
  published_at datetime
  created_at datetime [not null, default: 'now()']
  updated_at datetime
}

Table authors {
  id int [pk, increment]
  name varchar(100) [not null]
  email varchar(255) [not null, unique]
  bio text
  avatar_url varchar(500)
  created_at datetime [not null, default: 'now()']
}

Ref: articles.author_id > authors.id
";

            var generator = new DbmlRepositoryGeneratorFacade();
            await generator.GenerateFromContentAsync(dbmlContent, options);

            Console.WriteLine("Advanced templates used successfully!");
        }
    }

    // Example of how to extend the generator with custom functionality
    public static class GeneratorExtensions
    {
        public static async Task GenerateApiControllersAsync(this DbmlRepositoryGeneratorFacade generator,
            string dbmlContent, GeneratorOptions options)
        {
            // You could extend this to generate Web API controllers
            Console.WriteLine("Custom extension: API Controllers would be generated here");
        }

        public static async Task GenerateGraphQLSchemasAsync(this DbmlRepositoryGeneratorFacade generator,
            string dbmlContent, GeneratorOptions options)
        {
            // You could extend this to generate GraphQL schemas
            Console.WriteLine("Custom extension: GraphQL schemas would be generated here");
        }
    }
}