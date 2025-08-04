using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.Infrastructure;

// Template creation helper
public static class TemplateHelper
{
    public static async Task CreateCustomTemplatesAsync(string templateDirectory)
    {
        if (!Directory.Exists(templateDirectory))
            Directory.CreateDirectory(templateDirectory);

        var templates = new Dictionary<string, string>
            {
                { "entity.liquid", GetAdvancedEntityTemplate() },
                { "repository-interface.liquid", GetAdvancedRepositoryInterfaceTemplate() },
                //{ "repository.liquid", GetAdvancedRepositoryTemplate() },
                { "unitofwork-interface.liquid", GetAdvancedUnitOfWorkInterfaceTemplate() },
                { "unitofwork.liquid", GetAdvancedUnitOfWorkTemplate() },
                { "dbcontext.liquid", GetAdvancedDbContextTemplate() },
                { "dto.liquid", GetDtoTemplate() },
                { "service-interface.liquid", GetServiceInterfaceTemplate() },
                { "service.liquid", GetServiceTemplate() }
            };

        foreach (var template in templates)
        {
            var templatePath = Path.Combine(templateDirectory, template.Key);
            await File.WriteAllTextAsync(templatePath, template.Value);
        }
    }

    private static string GetAdvancedEntityTemplate()
    {
        return @"using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace {{ namespace }}.Entities
{
    /// <summary>
    /// Entity representing {{ table_name }} table
    /// {% if note %}{{ note }}{% endif %}
    /// </summary>
    [Table(""{{ table_name }}"")]
    public class {{ class_name }}
    {
        {% for property in properties %}
        {% comment %}Generate property attributes{% endcomment %}
        {% if property.is_primary_key %}
        [Key]
        {% endif %}
        {% if property.is_increment and property.is_primary_key %}
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        {% endif %}
        [Column(""{{ property.column_name }}"")]
        {% if property.is_not_null and property.clr_type == 'string' %}
        [Required]
        {% endif %}
        {% if property.is_unique %}
        // TODO: Add unique constraint configuration in DbContext
        {% endif %}
        
        /// <summary>
        /// {{ property.property_name }}{% if property.note %} - {{ property.note }}{% endif %}
        /// </summary>
        public {{ property.clr_type }}{% if property.is_nullable %}?{% endif %} {{ property.property_name }} { get; set; }{% if property.default_value %} = {{ property.default_value }};{% endif %}
        
        {% endfor %}
        
        // Navigation properties will be added here based on relationships
        {% comment %}TODO: Add navigation properties based on foreign keys{% endcomment %}
    }
}";
    }

    private static string GetAdvancedRepositoryInterfaceTemplate()
    {
        return @"using System;
using System.Collections.Generic;
using System.Linq.Expressions;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using {{ namespace }}.Entities;

namespace {{ namespace }}.Repositories
{
    /// <summary>
    /// Repository interface for {{ entity_name }} entity
    /// </summary>
    public interface {{ interface_name }} : IRepository<{{ entity_name }}>
    {
        // Add custom methods specific to {{ entity_name }} here
        {% comment %}
        // Example custom methods:
        // Task<IEnumerable<{{ entity_name }}>> GetActiveAsync();
        // Task<{{ entity_name }}> GetByNameAsync(string name);
        {% endcomment %}
    }

    /// <summary>
    /// Generic repository interface
    /// </summary>
    /// <typeparam name=""T"">Entity type</typeparam>
    public interface IRepository<T> where T : class
    {
        {% if generate_async %}
        /// <summary>
        /// Gets an entity by its identifier asynchronously
        /// </summary>
        Task<T?> GetByIdAsync(object id);
        
        /// <summary>
        /// Gets all entities asynchronously
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();
        
        /// <summary>
        /// Finds entities matching the predicate asynchronously
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Gets the first entity matching the predicate or null asynchronously
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        
        /// <summary>
        /// Adds an entity asynchronously
        /// </summary>
        Task AddAsync(T entity);
        
        /// <summary>
        /// Adds multiple entities asynchronously
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);
        
        /// <summary>
        /// Removes an entity by id asynchronously
        /// </summary>
        Task<bool> RemoveAsync(object id);
        
        /// <summary>
        /// Saves changes asynchronously
        /// </summary>
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
        
        /// <summary>
        /// Updates an entity
        /// </summary>
        void Update(T entity);
        
        /// <summary>
        /// Removes an entity
        /// </summary>
        void Remove(T entity);
        
        /// <summary>
        /// Removes multiple entities
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);
    }
}";
    }

    //        private static string GetAdvancedRepositoryTemplate()
    //        {
    //            return @"using System;
    //using System.Collections.Generic;
    //using System.Linq;
    //using System.Linq.Expressions;
    //{% if generate_async %}using System.Threading.Tasks;{% endif %}
    //using Microsoft.EntityFrameworkCore;
    //using Microsoft.Extensions.Logging;
    //using {{ namespace }}.Entities;

    //namespace {{ namespace }}.Repositories
    //{
    //    /// <summary>
    //    /// Repository implementation for {{ entity_name }}
    //    /// </summary>
    //    public class {{ class_name }} : {{ interface_name }}
    //    {
    //        private readonly DbContext _context;
    //        private readonly DbSet<{{ entity_name }}> _dbSet;
    //        private readonly ILogger<{{ class_name }}>? _logger;

    //        public {{ class_name }}(DbContext context, ILogger<{{ class_name }}>? logger = null)
    //        {
    //            _context = context ?? throw new ArgumentNullException(nameof(context));
    //            _dbSet = context.Set<{{ entity_name }}>();
    //            _logger = logger;
    //        }

    //        {% if generate_async %}
    //        public async Task<{{ entity_name }}?> GetByIdAsync(object id)
    //        {
    //            try
    //            {
    //                _logger?.LogError(ex, "Error creating { { entity_name } }
    //            ");
    //                throw;
    //        }
    //    }

    //    public async Task<{{ entity_name
    //}}Dto ?> UpdateAsync(int id, Update{ { entity_name } }
    //Dto dto)
    //        {
    //            try
    //            {
    //                var entity = await _unitOfWork.{{ entity_name }}Repository.GetByIdAsync(id);
    //if (entity == null) return null;

    //MapToEntity(dto, entity);
    //_unitOfWork.{ { entity_name } }
    //Repository.Update(entity);
    //await _unitOfWork.CompleteAsync();
    //return MapToDto(entity);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger?.LogError(ex, "Error updating {{ entity_name }} with id: {Id}", id);
    //throw;
    //            }
    //        }

    //        public async Task<bool> DeleteAsync(int id)
    //{
    //    try
    //    {
    //        var result = await _unitOfWork.{ { entity_name } }
    //        Repository.RemoveAsync(id);
    //        if (result)
    //            await _unitOfWork.CompleteAsync();
    //        return result;
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger?.LogError(ex, "Error deleting {{ entity_name }} with id: {Id}", id);
    //        throw;
    //    }
    //}
    //{% endif %}

    //private
    //{ { entity_name } }
    //Dto MapToDto({{ entity_name }} entity)
    //        {
    //            // TODO: Consider using AutoMapper for complex mappings
    //            return new {{ entity_name }}Dto
    //            {
    //                // Map properties here
    //            };
    //        }

    //        private
    //{ { entity_name } }
    //MapToEntity(Create{ { entity_name } }
    //Dto dto)
    //        {
    //            // TODO: Consider using AutoMapper for complex mappings
    //            return new {{ entity_name }}
    //            {
    //    // Map properties here
    //}
    //;
    //        }

    //        private void MapToEntity(Update{{ entity_name }}Dto dto, {{ entity_name }} entity)
    //        {
    //            // TODO: Consider using AutoMapper for complex mappings
    //            // Update entity properties from dto
    //        }
    //    }
    //}";
    //        }
    //    }
    //}LogDebug(""Getting { { entity_name } }
    //with id: { Id}
    //"", id);
    //return await _dbSet.FindAsync(id);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger?.LogError(ex, ""Error getting {{ entity_name }} with id: { Id}
    //"", id);
    //throw;
    //            }
    //        }

    //        public async Task<IEnumerable<{ { entity_name } }>> GetAllAsync()
    //        {
    //    try
    //    {
    //        _logger?.LogDebug(""Getting all { { entity_name } }
    //        entities"");
    //        return await _dbSet.ToListAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger?.LogError(ex, ""Error getting all { { entity_name } }
    //        entities"");
    //        throw;
    //    }
    //}

    //public async Task<IEnumerable<{ { entity_name } }>> FindAsync(Expression < Func <{ { entity_name } }, bool>> predicate)
    //        {
    //            try
    //            {
    //                _logger?.LogDebug(""Finding {{ entity_name }} entities with predicate"");
    //return await _dbSet.Where(predicate).ToListAsync();
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger?.LogError(ex, ""Error finding {{ entity_name }} entities"");
    //throw;
    //            }
    //        }

    //        public async Task<{ { entity_name } }?> FirstOrDefaultAsync(Expression<Func<{{ entity_name }}, bool>> predicate)
    //        {
    //            try
    //            {
    //                _logger?.LogDebug(""Getting first {{ entity_name }} entity with predicate"");
    //return await _dbSet.FirstOrDefaultAsync(predicate);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger?.LogError(ex, ""Error getting first {{ entity_name }} entity"");
    //throw;
    //            }
    //        }

    //        public async Task AddAsync({{ entity_name }} entity)
    //        {
    //            try
    //            {
    //                _logger?.LogDebug(""Adding {{ entity_name }} entity"");
    //await _dbSet.AddAsync(entity);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger?.LogError(ex, ""Error adding {{ entity_name }} entity"");
    //throw;
    //            }
    //        }

    //        public async Task AddRangeAsync(IEnumerable<{{ entity_name }}> entities)
    //        {
    //    try
    //    {
    //        _logger?.LogDebug(""Adding multiple { { entity_name } }
    //        entities"");
    //        await _dbSet.AddRangeAsync(entities);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger?.LogError(ex, ""Error adding multiple { { entity_name } }
    //        entities"");
    //        throw;
    //    }
    //}

    //public async Task<bool> RemoveAsync(object id)
    //{
    //    try
    //    {
    //        _logger?.LogDebug(""Removing { { entity_name } }
    //        with id: { Id}
    //"", id);
    //var entity = await GetByIdAsync(id);
    //if (entity != null)
    //{
    //    _dbSet.Remove(entity);
    //    return true;
    //}
    //return false;
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger?.LogError(ex, ""Error removing {{ entity_name }} with id: { Id}
    //"", id);
    //throw;
    //            }
    //        }

    //        public async Task<int> SaveChangesAsync()
    //{
    //    try
    //    {
    //        _logger?.LogDebug(""Saving changes"");
    //        return await _context.SaveChangesAsync();
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger?.LogError(ex, ""Error saving changes"");
    //        throw;
    //    }
    //}
    //{% endif %}

    //public void Update({{ entity_name }} entity)
    //        {
    //            try
    //            {
    //                _logger?.LogDebug(""Updating {{ entity_name }} entity"");
    //_dbSet.Update(entity);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger?.LogError(ex, ""Error updating {{ entity_name }} entity"");
    //throw;
    //            }
    //        }

    //        public void Remove({{ entity_name }} entity)
    //        {
    //            try
    //            {
    //                _logger?.LogDebug(""Removing {{ entity_name }} entity"");
    //_dbSet.Remove(entity);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger?.LogError(ex, ""Error removing {{ entity_name }} entity"");
    //throw;
    //            }
    //        }

    //        public void RemoveRange(IEnumerable<{{ entity_name }}> entities)
    //        {
    //    try
    //    {
    //        _logger?.LogDebug(""Removing multiple { { entity_name } }
    //        entities"");
    //        _dbSet.RemoveRange(entities);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger?.LogError(ex, ""Error removing multiple { { entity_name } }
    //        entities"");
    //        throw;
    //    }
    //}
    //    }
    //}";
    //        }

    private static string GetAdvancedUnitOfWorkInterfaceTemplate()
    {
        return @"using System;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using {{ namespace }}.Repositories;

namespace {{ namespace }}
{
    /// <summary>
    /// Unit of Work interface for managing repositories and transactions
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        {% for repo in repositories %}
        /// <summary>
        /// Gets the {{ repo.entity_name }} repository
        /// </summary>
        {{ repo.interface_name }} {{ repo.repository_name }} { get; }
        {% endfor %}

        {% if generate_async %}
        /// <summary>
        /// Saves all changes asynchronously
        /// </summary>
        /// <returns>Number of affected records</returns>
        Task<int> CompleteAsync();
        
        /// <summary>
        /// Begins a transaction asynchronously
        /// </summary>
        Task BeginTransactionAsync();
        
        /// <summary>
        /// Commits the current transaction asynchronously
        /// </summary>
        Task CommitTransactionAsync();
        
        /// <summary>
        /// Rolls back the current transaction asynchronously
        /// </summary>
        Task RollbackTransactionAsync();
        {% endif %}
        
        /// <summary>
        /// Saves all changes synchronously
        /// </summary>
        /// <returns>Number of affected records</returns>
        int Complete();
        
        /// <summary>
        /// Begins a transaction synchronously
        /// </summary>
        void BeginTransaction();
        
        /// <summary>
        /// Commits the current transaction synchronously
        /// </summary>
        void CommitTransaction();
        
        /// <summary>
        /// Rolls back the current transaction synchronously
        /// </summary>
        void RollbackTransaction();
    }
}";
    }

    private static string GetAdvancedUnitOfWorkTemplate()
    {
        return @"using System;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using {{ namespace }}.Entities;
using {{ namespace }}.Repositories;

namespace {{ namespace }}
{
    /// <summary>
    /// Unit of Work implementation
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly ILogger<UnitOfWork>? _logger;
        private IDbContextTransaction? _transaction;
        {% for repo in repositories %}
        private {{ repo.interface_name }}? _{{ repo.field_name }};
        {% endfor %}

        public UnitOfWork(DbContext context, ILogger<UnitOfWork>? logger = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger;
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
            try
            {
                _logger?.LogDebug(""Completing unit of work asynchronously"");
                var result = await _context.SaveChangesAsync();
                _logger?.LogDebug(""Completed unit of work. {Count} records affected"", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error completing unit of work"");
                throw;
            }
        }
        
        public async Task BeginTransactionAsync()
        {
            try
            {
                _logger?.LogDebug(""Beginning transaction asynchronously"");
                _transaction = await _context.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error beginning transaction"");
                throw;
            }
        }
        
        public async Task CommitTransactionAsync()
        {
            try
            {
                _logger?.LogDebug(""Committing transaction asynchronously"");
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error committing transaction"");
                throw;
            }
        }
        
        public async Task RollbackTransactionAsync()
        {
            try
            {
                _logger?.LogDebug(""Rolling back transaction asynchronously"");
                if (_transaction != null)
                {
                    await _transaction.RollbackAsync();
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error rolling back transaction"");
                throw;
            }
        }
        {% endif %}

        public int Complete()
        {
            try
            {
                _logger?.LogDebug(""Completing unit of work synchronously"");
                var result = _context.SaveChanges();
                _logger?.LogDebug(""Completed unit of work. {Count} records affected"", result);
                return result;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error completing unit of work"");
                throw;
            }
        }
        
        public void BeginTransaction()
        {
            try
            {
                _logger?.LogDebug(""Beginning transaction synchronously"");
                _transaction = _context.Database.BeginTransaction();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error beginning transaction"");
                throw;
            }
        }
        
        public void CommitTransaction()
        {
            try
            {
                _logger?.LogDebug(""Committing transaction synchronously"");
                _transaction?.Commit();
                _transaction?.Dispose();
                _transaction = null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error committing transaction"");
                throw;
            }
        }
        
        public void RollbackTransaction()
        {
            try
            {
                _logger?.LogDebug(""Rolling back transaction synchronously"");
                _transaction?.Rollback();
                _transaction?.Dispose();
                _transaction = null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error rolling back transaction"");
                throw;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}";
    }

    private static string GetAdvancedDbContextTemplate()
    {
        return @"using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using {{ namespace }}.Entities;

namespace {{ namespace }}
{
    /// <summary>
    /// Application database context
    /// </summary>
    public class {{ class_name }} : DbContext
    {
        private readonly ILogger<{{ class_name }}>? _logger;

        public {{ class_name }}(DbContextOptions<{{ class_name }}> options, ILogger<{{ class_name }}>? logger = null)
            : base(options)
        {
            _logger = logger;
        }

        {% for entity in entities %}
        /// <summary>
        /// DbSet for {{ entity.entity_name }} entities
        /// </summary>
        public DbSet<{{ entity.entity_name }}> {{ entity.db_set_name }} { get; set; }
        {% endfor %}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            _logger?.LogDebug(""Configuring entity relationships"");

            {% for relationship in relationships %}
            // Configure {{ relationship.from_entity }} -> {{ relationship.to_entity }} relationship
            modelBuilder.Entity<{{ relationship.from_entity }}>()
                .HasOne<{{ relationship.to_entity }}>()
                {% if relationship.relation_type == '1:n' %}.WithMany(){% else %}.WithOne(){% endif %}
                .HasForeignKey(""{{ relationship.from_column }}"")
                .OnDelete(DeleteBehavior.Restrict); // Change as needed
            {% endfor %}
            
            // Add any additional configuration here
            ConfigureEntities(modelBuilder);
        }
        
        /// <summary>
        /// Configure additional entity settings
        /// </summary>
        private void ConfigureEntities(ModelBuilder modelBuilder)
        {
            // Add custom entity configurations here
            {% for entity in entities %}
            // Configure {{ entity.entity_name }}
            // modelBuilder.Entity<{{ entity.entity_name }}>()
            //     .HasIndex(e => e.SomeProperty)
            //     .IsUnique();
            {% endfor %}
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            if (_logger != null)
            {
                optionsBuilder.LogTo(message => _logger.LogInformation(message));
            }
        }
    }
}";
    }

    private static string GetDtoTemplate()
    {
        return @"using System;
using System.ComponentModel.DataAnnotations;

namespace {{ namespace }}.DTOs
{
    /// <summary>
    /// Data Transfer Object for {{ entity_name }}
    /// </summary>
    public class {{ class_name }}Dto
    {
        {% for property in properties %}
        {% if property.is_not_null and property.clr_type == 'string' %}
        [Required]
        {% endif %}
        public {{ property.clr_type }}{% if property.is_nullable %}?{% endif %} {{ property.property_name }} { get; set; }
        {% endfor %}
    }
    
    /// <summary>
    /// Create DTO for {{ entity_name }}
    /// </summary>
    public class Create{{ class_name }}Dto
    {
        {% for property in properties %}
        {% unless property.is_primary_key or property.is_increment %}
        {% if property.is_not_null and property.clr_type == 'string' %}
        [Required]
        {% endif %}
        public {{ property.clr_type }}{% if property.is_nullable %}?{% endif %} {{ property.property_name }} { get; set; }
        {% endunless %}
        {% endfor %}
    }
    
    /// <summary>
    /// Update DTO for {{ entity_name }}
    /// </summary>
    public class Update{{ class_name }}Dto
    {
        {% for property in properties %}
        {% unless property.is_increment %}
        public {{ property.clr_type }}{% if not property.is_primary_key and not property.is_not_null %}?{% endif %} {{ property.property_name }} { get; set; }
        {% endunless %}
        {% endfor %}
    }
}";
    }

    private static string GetServiceInterfaceTemplate()
    {
        return @"using System;
using System.Collections.Generic;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using {{ namespace }}.DTOs;

namespace {{ namespace }}.Services
{
    /// <summary>
    /// Service interface for {{ entity_name }}
    /// </summary>
    public interface I{{ entity_name }}Service
    {
        {% if generate_async %}
        Task<{{ entity_name }}Dto?> GetByIdAsync(int id);
        Task<IEnumerable<{{ entity_name }}Dto>> GetAllAsync();
        Task<{{ entity_name }}Dto> CreateAsync(Create{{ entity_name }}Dto dto);
        Task<{{ entity_name }}Dto?> UpdateAsync(int id, Update{{ entity_name }}Dto dto);
        Task<bool> DeleteAsync(int id);
        {% else %}
        {{ entity_name }}Dto? GetById(int id);
        IEnumerable<{{ entity_name }}Dto> GetAll();
        {{ entity_name }}Dto Create(Create{{ entity_name }}Dto dto);
        {{ entity_name }}Dto? Update(int id, Update{{ entity_name }}Dto dto);
        bool Delete(int id);
        {% endif %}
    }
}";
    }

    private static string GetServiceTemplate()
    {
        return @"using System;
using System.Collections.Generic;
using System.Linq;
{% if generate_async %}using System.Threading.Tasks;{% endif %}
using Microsoft.Extensions.Logging;
using {{ namespace }}.DTOs;
using {{ namespace }}.Entities;

namespace {{ namespace }}.Services
{
    /// <summary>
    /// Service implementation for {{ entity_name }}
    /// </summary>
    public class {{ entity_name }}Service : I{{ entity_name }}Service
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<{{ entity_name }}Service>? _logger;

        public {{ entity_name }}Service(IUnitOfWork unitOfWork, ILogger<{{ entity_name }}Service>? logger = null)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
        }

        {% if generate_async %}
        public async Task<{{ entity_name }}Dto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.{{ entity_name }}Repository.GetByIdAsync(id);
                return entity != null ? MapToDto(entity) : null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error getting {{ entity_name }} with id: {Id}"", id);
                throw;
            }
        }

        public async Task<IEnumerable<{{ entity_name }}Dto>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.{{ entity_name }}Repository.GetAllAsync();
                return entities.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ""Error getting all {{ entity_name }} entities"");
                throw;
            }
        }

        public async Task<{{ entity_name }}Dto> CreateAsync(Create{{ entity_name }}Dto dto)
        {
            try
            {
                var entity = MapToEntity(dto);
                await _unitOfWork.{{ entity_name }}Repository.AddAsync(entity);
                await _unitOfWork.CompleteAsync();
                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                _logger?.";
    }
}
