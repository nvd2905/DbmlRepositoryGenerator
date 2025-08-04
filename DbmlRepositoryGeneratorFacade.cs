using DbmlRepositoryGenerator.Generator;
using DbmlRepositoryGenerator.Infrastructure;
using DbmlRepositoryGenerator.Parsers;

namespace DbmlRepositoryGenerator;

public class DbmlRepositoryGeneratorFacade
{
    public async Task GenerateFromFileAsync(string dbmlFilePath, GeneratorOptions options)
    {
        var dbmlContent = await File.ReadAllTextAsync(dbmlFilePath);
        await GenerateFromContentAsync(dbmlContent, options);
    }

    public async Task GenerateFromContentAsync(string dbmlContent, GeneratorOptions options)
    {
        var parser = new DbmlParser();
        var schema = parser.ParseDbml(dbmlContent);

        var generator = new RepositoryGenerator(options);
        await generator.GenerateAsync(schema);
    }
}