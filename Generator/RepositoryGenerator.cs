using DbmlRepositoryGenerator.Infrastructure;
using DbmlRepositoryGenerator.Models;
using DbmlRepositoryGenerator.TemplateGenerator;
using DbmlRepositoryGenerator.TemplateModels;
using DotLiquid;

namespace DbmlRepositoryGenerator.Generator;

// Main code generator with Liquid templates
public class RepositoryGenerator
{
    private readonly GeneratorOptions _options;
    private readonly TemplateManager _templateManager;

    public RepositoryGenerator(GeneratorOptions options)
    {
        _options = options;
        _templateManager = new TemplateManager(_options.TemplateDirectory);
    }

    public async Task GenerateAsync(DbmlSchema schema)
    {
        if (!Directory.Exists(_options.OutputDirectory))
            Directory.CreateDirectory(_options.OutputDirectory);

        // Initialize template directories if they don't exist
        await EnsureTemplateDirectoryExists();

        // Generate entities
        foreach (var table in schema.Tables)
        {
            await GenerateEntityAsync(table);
            if (_options.GenerateInterfaces)
                await GenerateRepositoryInterfaceAsync(table);
            await GenerateRepositoryAsync(table);
            await GenerateConfiguration(table);
        }

        // Generate Unit of Work
        await GenerateUnitOfWorkInterfaceAsync(schema);
        await GenerateUnitOfWorkAsync(schema);

        // Generate DbContext
        await GenerateDbContextAsync(schema);
    }

    private async Task GenerateConfiguration(DbmlTable table)
    {
        var template = await _templateManager.GetTemplateAsync("configuration");
        var className = ToPascalCase(table.Name);

        var model = new ConfigurationTemplateModel
        {
            Name = table.Name,
            ClassName = ToPascalCase(table.Name),
            TableName = ToPascalCase(table.Name),
            Namespace = _options.Namespace,
            Note = table.Note,
            Properties = table.Columns.Select(c => new PropertyTemplateModel
            {
                Name = c.Name,
                PropertyName = ToPascalCase(c.Name),
                ColumnName = ToPascalCase(c.Name),
                Type = c.Type,
                ClrType = TypeMapper.MapDbTypeToClrType(c.Type, _options.Provider),
                IsPrimaryKey = c.IsPrimaryKey,
                IsNotNull = c.IsNotNull,
                IsUnique = c.IsUnique,
                IsIncrement = c.IsIncrement,
                IsNullable = !c.IsNotNull && !c.IsPrimaryKey && TypeMapper.IsNullableType(TypeMapper.MapDbTypeToClrType(c.Type, _options.Provider)),
                DefaultValue = c.DefaultValue,
                Note = c.Note
            }).ToList()
        };

        var result = template.Render(Hash.FromAnonymousObject(new
        {
            @namespace = model.Namespace,
            class_name = model.ClassName,
            table_name = model.TableName,
            properties = model.Properties.Select(p => new
            {
                property_name = p.PropertyName,
                column_name = p.ColumnName,
                clr_type = p.ClrType,
                is_primary_key = p.IsPrimaryKey,
                is_not_null = p.IsNotNull,
                is_unique = p.IsUnique,
                is_increment = p.IsIncrement,
                is_nullable = p.IsNullable,
                default_value = p.DefaultValue,
                note = p.Note
            }).ToArray(),
            custom = _options.CustomVariables
        }));

        var outputDirectory = Path.Combine(_options.OutputDirectory, $"{model.Namespace}.Infrastructure.Tenant", "Data", "Configurations");
        var filePath = Path.Combine(outputDirectory, $"{model.TableName}Configuration.cs");
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);
        await File.WriteAllTextAsync(filePath, result);
    }

    private async Task EnsureTemplateDirectoryExists()
    {
        if (!Directory.Exists(_options.TemplateDirectory))
        {
            Directory.CreateDirectory(_options.TemplateDirectory);

            // Create default templates
            var templates = new Dictionary<string, string>
            {
                { "entity.liquid", _templateManager.GetDefaultTemplate("entity") },
                { "repository-interface.liquid", _templateManager.GetDefaultTemplate("repository-interface") },
                { "repository.liquid", _templateManager.GetDefaultTemplate("repository") },
                { "unitofwork-interface.liquid", _templateManager.GetDefaultTemplate("unitofwork-interface") },
                { "unitofwork.liquid", _templateManager.GetDefaultTemplate("unitofwork") },
                { "dbcontext.liquid", _templateManager.GetDefaultTemplate("dbcontext") },
                { "configuration.liquid", _templateManager.GetDefaultTemplate("configuration") }
            };

            foreach (var template in templates)
            {
                var templatePath = Path.Combine(_options.TemplateDirectory, template.Key);
                await File.WriteAllTextAsync(templatePath, template.Value);
            }
        }
    }

    private async Task GenerateEntityAsync(DbmlTable table)
    {
        var template = await _templateManager.GetTemplateAsync("entity");

        var model = new EntityTemplateModel
        {
            Name = table.Name,
            ClassName = ToPascalCase(table.Name),
            TableName = ToPascalCase(table.Name),
            Namespace = _options.Namespace,
            Note = table.Note,
            Properties = table.Columns.Where(w => !w.Name.Equals("id", StringComparison.OrdinalIgnoreCase)).Select(c => new PropertyTemplateModel
            {
                Name = c.Name,
                PropertyName = ToPascalCase(c.Name),
                ColumnName = ToPascalCase(c.Name),
                Type = c.Type,
                ClrType = TypeMapper.MapDbTypeToClrType(c.Type, _options.Provider),
                IsPrimaryKey = c.IsPrimaryKey,
                IsNotNull = c.IsNotNull,
                IsUnique = c.IsUnique,
                IsIncrement = c.IsIncrement,
                IsNullable = !c.IsNotNull && !c.IsPrimaryKey && TypeMapper.IsNullableType(TypeMapper.MapDbTypeToClrType(c.Type, _options.Provider)),
                DefaultValue = c.DefaultValue,
                Note = c.Note
            }).ToList()
        };

        var result = template.Render(Hash.FromAnonymousObject(new
        {
            @namespace = model.Namespace,
            class_name = model.ClassName,
            table_name = model.TableName,
            properties = model.Properties.Select(p => new
            {
                property_name = p.PropertyName,
                column_name = p.ColumnName,
                clr_type = p.ClrType,
                is_primary_key = p.IsPrimaryKey,
                is_not_null = p.IsNotNull,
                is_unique = p.IsUnique,
                is_increment = p.IsIncrement,
                is_nullable = p.IsNullable,
                default_value = p.DefaultValue,
                note = p.Note
            }).ToArray(),
            custom = _options.CustomVariables
        }));

        var outputDirectory = Path.Combine(_options.OutputDirectory, $"{model.Namespace}.Domain", "Entities");
        var filePath = Path.Combine(outputDirectory, $"{model.ClassName}.cs");
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);
        await File.WriteAllTextAsync(filePath, result);
    }

    private async Task GenerateRepositoryInterfaceAsync(DbmlTable table)
    {
        var template = await _templateManager.GetTemplateAsync("repository-interface");
        var className = ToPascalCase(table.Name);

        var model = new RepositoryTemplateModel
        {
            EntityName = className,
            ClassName = $"{className}Repository",
            InterfaceName = $"I{className}Repository",
            Namespace = _options.Namespace,
            GenerateAsync = _options.GenerateAsync,
            GenerateInterfaces = _options.GenerateInterfaces
        };

        var result = template.Render(Hash.FromAnonymousObject(new
        {
            @namespace = model.Namespace,
            entity_name = model.EntityName,
            class_name = model.ClassName,
            interface_name = model.InterfaceName,
            generate_async = model.GenerateAsync,
            generate_interfaces = model.GenerateInterfaces,
            custom = _options.CustomVariables
        }));

        var outputDirectory = Path.Combine(_options.OutputDirectory, $"{model.Namespace}.Application", "Common", "Interfaces");
        var filePath = Path.Combine(outputDirectory, $"{model.InterfaceName}.cs");
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);
        await File.WriteAllTextAsync(filePath, result);
    }

    private async Task GenerateRepositoryAsync(DbmlTable table)
    {
        var template = await _templateManager.GetTemplateAsync("repository");
        var className = ToPascalCase(table.Name);

        var model = new RepositoryTemplateModel
        {
            EntityName = className,
            ClassName = $"{className}Repository",
            InterfaceName = _options.GenerateInterfaces ? $"I{className}Repository" : $"IRepository<{className}>",
            Namespace = _options.Namespace,
            GenerateAsync = _options.GenerateAsync,
            GenerateInterfaces = _options.GenerateInterfaces
        };

        var result = template.Render(Hash.FromAnonymousObject(new
        {
            @namespace = model.Namespace,
            entity_name = model.EntityName,
            class_name = model.ClassName,
            interface_name = model.InterfaceName,
            generate_async = model.GenerateAsync,
            generate_interfaces = model.GenerateInterfaces,
            custom = _options.CustomVariables
        }));

        var outputDirectory = Path.Combine(_options.OutputDirectory, $"{model.Namespace}.Infrastructure.Tenant", "Data", "Repositories");
        var filePath = Path.Combine(outputDirectory, $"{model.ClassName}.cs");
        if (!Directory.Exists(outputDirectory))
            Directory.CreateDirectory(outputDirectory);
        await File.WriteAllTextAsync(filePath, result);
    }

    private async Task GenerateUnitOfWorkInterfaceAsync(DbmlSchema schema)
    {
        var template = await _templateManager.GetTemplateAsync("unitofwork-interface");

        var model = new UnitOfWorkTemplateModel
        {
            Namespace = _options.Namespace,
            GenerateAsync = _options.GenerateAsync,
            GenerateInterfaces = _options.GenerateInterfaces,
            Repositories = schema.Tables.Select(t =>
            {
                var className = ToPascalCase(t.Name);
                return new UnitOfWorkRepositoryModel
                {
                    EntityName = className,
                    RepositoryName = className + "Repository",
                    InterfaceName = _options.GenerateInterfaces ? $"I{className}Repository" : $"IRepository<{className}>",
                    FieldName = ToCamelCase(className) + "Repository"
                };
            }).ToList()
        };

        var result = template.Render(Hash.FromAnonymousObject(new
        {
            @namespace = model.Namespace,
            generate_async = model.GenerateAsync,
            generate_interfaces = model.GenerateInterfaces,
            repositories = model.Repositories.Select(r => new
            {
                entity_name = r.EntityName,
                repository_name = r.RepositoryName,
                interface_name = r.InterfaceName,
                field_name = r.FieldName
            }).ToArray(),
            custom = _options.CustomVariables
        }));

        var filePath = Path.Combine(_options.OutputDirectory, "IUnitOfWork.cs");
        await File.WriteAllTextAsync(filePath, result);
    }

    private async Task GenerateUnitOfWorkAsync(DbmlSchema schema)
    {
        var template = await _templateManager.GetTemplateAsync("unitofwork");

        var model = new UnitOfWorkTemplateModel
        {
            Namespace = _options.Namespace,
            GenerateAsync = _options.GenerateAsync,
            GenerateInterfaces = _options.GenerateInterfaces,
            Repositories = schema.Tables.Select(t =>
            {
                var className = ToPascalCase(t.Name);
                return new UnitOfWorkRepositoryModel
                {
                    EntityName = className,
                    RepositoryName = className + "Repository",
                    InterfaceName = _options.GenerateInterfaces ? $"I{className}Repository" : $"IRepository<{className}>",
                    FieldName = ToCamelCase(className) + "Repository"
                };
            }).ToList()
        };

        var result = template.Render(Hash.FromAnonymousObject(new
        {
            @namespace = model.Namespace,
            generate_async = model.GenerateAsync,
            generate_interfaces = model.GenerateInterfaces,
            repositories = model.Repositories.Select(r => new
            {
                entity_name = r.EntityName,
                repository_name = r.RepositoryName,
                interface_name = r.InterfaceName,
                field_name = r.FieldName
            }).ToArray(),
            custom = _options.CustomVariables
        }));

        var filePath = Path.Combine(_options.OutputDirectory, "UnitOfWork.cs");
        await File.WriteAllTextAsync(filePath, result);
    }

    private async Task GenerateDbContextAsync(DbmlSchema schema)
    {
        var template = await _templateManager.GetTemplateAsync("dbcontext");

        var model = new DbContextTemplateModel
        {
            Namespace = _options.Namespace,
            ClassName = "ApplicationDbContext",
            Entities = schema.Tables.Select(t => new DbContextEntityModel
            {
                EntityName = ToPascalCase(t.Name),
                DbSetName = ToPascalCase(t.Name) + "s"
            }).ToList(),
            Relationships = schema.References.Select(r => new RelationshipTemplateModel
            {
                FromEntity = ToPascalCase(r.FromTable),
                ToEntity = ToPascalCase(r.ToTable),
                FromColumn = r.FromColumn,
                ToColumn = r.ToColumn,
                RelationType = r.RelationType
            }).ToList()
        };

        var result = template.Render(Hash.FromAnonymousObject(new
        {
            @namespace = model.Namespace,
            class_name = model.ClassName,
            entities = model.Entities.Select(e => new
            {
                entity_name = e.EntityName,
                db_set_name = e.DbSetName
            }).ToArray(),
            relationships = model.Relationships.Select(r => new
            {
                from_entity = r.FromEntity,
                to_entity = r.ToEntity,
                from_column = r.FromColumn,
                to_column = r.ToColumn,
                relation_type = r.RelationType
            }).ToArray(),
            custom = _options.CustomVariables
        }));

        var filePath = Path.Combine(_options.OutputDirectory, $"{model.ClassName}.cs");
        await File.WriteAllTextAsync(filePath, result);
    }

    private static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split('_', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }

        return string.Join("", words);
    }

    private static string ToCamelCase(string input)
    {
        var pascalCase = ToPascalCase(input);
        if (string.IsNullOrEmpty(pascalCase))
            return pascalCase;

        return char.ToLower(pascalCase[0]) + pascalCase.Substring(1);
    }
}