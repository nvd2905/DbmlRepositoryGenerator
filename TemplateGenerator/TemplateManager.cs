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
            _ => throw new ArgumentException($"Unknown template: {templateName}")
        };
    }

    private string GetDefaultEntityTemplate()
    {
        return @"using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace {{ namespace }}.Entities
{
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
    }
}";
    }

    private string GetDefaultRepositoryInterfaceTemplate()
    {
        return @"using System;
using System.Collections.Generic;
using System.Linq.Expressions;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using {{ namespace }}.Entities;

namespace {{ namespace }}.Repositories
{
    public interface {{ interface_name }} : IRepository<{{ entity_name }}>
    {
        // Add custom methods here
    }

    public interface IRepository<T> where T : class
    {
        {% if generate_async %}
        Task<T?> GetByIdAsync(object id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task<bool> RemoveAsync(object id);
        Task<int> SaveChangesAsync();
        {% else %}
        T? GetById(object id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T? FirstOrDefault(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        bool Remove(object id);
        int SaveChanges();
        {% endif %}
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}";
    }

    private string GetDefaultRepositoryTemplate()
    {
        return @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using Microsoft.EntityFrameworkCore;
using {{ namespace }}.Entities;

namespace {{ namespace }}.Repositories
{
    public class {{ class_name }} : {{ interface_name }}
    {
        private readonly DbContext _context;
        private readonly DbSet<{{ entity_name }}> _dbSet;

        public {{ class_name }}(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<{{ entity_name }}>();
        }

        {% if generate_async %}
        public async Task<{{ entity_name }}?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<{{ entity_name }}>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<{{ entity_name }}>> FindAsync(Expression<Func<{{ entity_name }}, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<{{ entity_name }}?> FirstOrDefaultAsync(Expression<Func<{{ entity_name }}, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync({{ entity_name }} entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<{{ entity_name }}> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> RemoveAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                return true;
            }
            return false;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        {% endif %}

        public void Update({{ entity_name }} entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove({{ entity_name }} entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<{{ entity_name }}> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}";
    }

    private string GetDefaultUnitOfWorkInterfaceTemplate()
    {
        return @"using System;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using {{ namespace }}.Repositories;

namespace {{ namespace }}
{
    public interface IUnitOfWork : IDisposable
    {
        {% for repo in repositories %}
        {{ repo.interface_name }} {{ repo.repository_name }} { get; }
        {% endfor %}

        {% if generate_async %}Task<int> CompleteAsync();{% endif %}
        int Complete();
    }
}";
    }

    private string GetDefaultUnitOfWorkTemplate()
    {
        return @"using System;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using Microsoft.EntityFrameworkCore;
using {{ namespace }}.Entities;
using {{ namespace }}.Repositories;

namespace {{ namespace }}
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        {% for repo in repositories %}
        private {{ repo.interface_name }}? _{{ repo.field_name }};
        {% endfor %}

        public UnitOfWork(DbContext context)
        {
            _context = context;
        }

        {% for repo in repositories %}
        public {{ repo.interface_name }} {{ repo.repository_name }}
        {
            get { return _{{ repo.field_name }} ??= new {{ repo.entity_name }}Repository(_context); }
        }
        {% endfor %}

        {% if generate_async %}
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        {% endif %}

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
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
}
