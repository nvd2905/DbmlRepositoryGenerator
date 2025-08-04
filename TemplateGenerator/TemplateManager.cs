using DotLiquid;
using DotLiquid.FileSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateGenerator;

public class TemplateManager
{
    private readonly string _templateDirectory;
    private readonly Dictionary<string, Template> _templates = new();

    public TemplateManager(string templateDirectory)
    {
        _templateDirectory = templateDirectory;
        Template.FileSystem = new LocalFileSystem(_templateDirectory);
    }

    public async Task<Template> GetTemplateAsync(string templateName)
    {
        if (_templates.ContainsKey(templateName))
            return _templates[templateName];

        var templatePath = Path.Combine(_templateDirectory, $"{templateName}.liquid");

        string templateContent;
        if (File.Exists(templatePath))
        {
            templateContent = await File.ReadAllTextAsync(templatePath);
        }
        else
        {
            templateContent = GetDefaultTemplate(templateName);
        }

        var template = Template.Parse(templateContent);
        _templates[templateName] = template;
        return template;
    }

    public string GetDefaultTemplate(string templateName)
    {
        return templateName.ToLower() switch
        {
            "entity" => GetDefaultEntityTemplate(),
            "repository-interface" => GetDefaultRepositoryInterfaceTemplate(),
            "repository" => GetDefaultRepositoryTemplate(),
            "unitofwork-interface" => GetDefaultUnitOfWorkInterfaceTemplate(),
            "unitofwork" => GetDefaultUnitOfWorkTemplate(),
            "dbcontext" => GetDefaultDbContextTemplate(),
            "configuration" => GetDefaultConfigurationTemplate(),
            _ => throw new ArgumentException($"Unknown template: {templateName}")
        };
    }

    private string GetDefaultEntityTemplate()
    {
        return @"//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using {{ namespace }}.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace {{ namespace }}.Domain.Entities;
[Table(""{{ table_name }}"")]
public class {{ class_name }}
{
    {% for property in properties %}
    {% if property.is_primary_key %}[Key]{% endif %}
    {% if property.is_increment and property.is_primary_key %}[DatabaseGenerated(DatabaseGeneratedOption.Identity)]{% endif %}
    [Column(""{{ property.column_name }}"")]
    {% if property.is_not_null and property.clr_type == 'string' %}[Required]{% endif %}
    public {{ property.clr_type }}{% if property.is_nullable %}?{% endif %} {{ property.property_name }} { get; set; }
    
    {% endfor %}
}";
    }

    private string GetDefaultRepositoryInterfaceTemplate()
    {
        return @"//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

namespace {{ namespace }}.Application.Common.Interfaces;

public interface {{ interface_name }} : IBaseRepository<{{ entity_name }}, int>
{
}";
    }

    private string GetDefaultRepositoryTemplate()
    {
        return @"//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using {{ namespace }}.Application.Common.Interfaces;

namespace {{ namespace }}.Infrastructure.Tenant.Data.Repositories;

namespace {{ namespace }}.Repositories;

public class {{ class_name }} : BaseRepository<{{ class_name }}, int>, {{ interface_name }}
{
    private readonly ApplicationTenantDbContext _tenantDbContext;
    public {{ class_name }}(ApplicationTenantDbContext tenantDbContext) : base(tenantDbContext)
    {
        _tenantDbContext = tenantDbContext;
    }
}";
    }

    private string GetDefaultUnitOfWorkInterfaceTemplate()
    {
        return @"//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Microsoft.EntityFrameworkCore.Storage;

namespace {{ namespace }}.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    #region Repository
    {% for repo in repositories %}
        {{ repo.interface_name }} {{ repo.repository_name }} { get; }
        {% endfor %}
    #endregion

    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    IDbContextTransaction BeginTransaction();
}";
    }

    private string GetDefaultUnitOfWorkTemplate()
    {
        return @"//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Microsoft.EntityFrameworkCore.Storage;
using {{ namespace }}.Application.Common.Interfaces;

namespace {{ namespace }}.Infrastructure.Tenant.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    protected readonly ApplicationTenantDbContext _context;

    public UnitOfWork(ApplicationTenantDbContext context)
    {
        _context = context;
    }

    #region IRepository

    {% for repo in repositories %}
        private {{ repo.interface_name }}? _{{ repo.field_name }};
        {% endfor %}

    #endregion IRepository

    #region Repository

    {% for repo in repositories %}
        public {{ repo.interface_name }} {{ repo.repository_name }}
        {
            get { return _{{ repo.field_name }} ??= new {{ repo.entity_name }}Repository(_context); }
        }
        {% endfor %}

    #endregion

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed && disposing)
        {
            _context.Dispose();
        }
        this.disposed = true;
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException(""An error occurred while saving the entity changes"", ex);
        }
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }
}";
    }

    private string GetDefaultDbContextTemplate()
    {
        return @"using Microsoft.EntityFrameworkCore;
using {{ namespace }}.Entities;

namespace {{ namespace }}
{
    public class {{ class_name }} : DbContext
    {
        public {{ class_name }}(DbContextOptions<{{ class_name }}> options)
            : base(options)
        {
        }

        {% for entity in entities %}
        public DbSet<{{ entity.entity_name }}> {{ entity.db_set_name }} { get; set; }
        {% endfor %}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            {% for relationship in relationships %}
            // Configure {{ relationship.from_entity }} -> {{ relationship.to_entity }} relationship
            modelBuilder.Entity<{{ relationship.from_entity }}>()
                .HasOne<{{ relationship.to_entity }}>()
                {% if relationship.relation_type == '1:n' %}.WithMany(){% else %}.WithOne(){% endif %}
                .HasForeignKey(""{{ relationship.from_column }}"");
            {% endfor %}
        }
    }
}";
    }

    private string GetDefaultConfigurationTemplate()
    {
        return @"//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace {{ namespace }}.Domain.Entities;

public class {{ table_name }}Configuration : IEntityTypeConfiguration<{{ table_name }}>
{
    public void Configure(EntityTypeBuilder<{{ table_name }}> builder)
    {
{% for property in properties %}
        builder.Property(x => x.{{ property.column_name }});{% if property.is_primary_key %}
        builder.HasKey(x => x.{{ property.column_name }});{% endif %}
{% endfor %}
    }
}";
    }
}
