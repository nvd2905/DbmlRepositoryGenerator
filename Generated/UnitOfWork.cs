//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Microsoft.EntityFrameworkCore.Storage;
using Inventory.Application.Common.Interfaces;

namespace Inventory.Infrastructure.Tenant.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    protected readonly ApplicationTenantDbContext _context;

    public UnitOfWork(ApplicationTenantDbContext context)
    {
        _context = context;
    }

    #region IRepository

    
        private IUsersRepository? _usersRepository;
        
        private IPostsRepository? _postsRepository;
        
        private ICommentsRepository? _commentsRepository;
        

    #endregion IRepository

    #region Repository

    
        public IUsersRepository UsersRepository
        {
            get { return _usersRepository ??= new UsersRepository(_context); }
        }
        
        public IPostsRepository PostsRepository
        {
            get { return _postsRepository ??= new PostsRepository(_context); }
        }
        
        public ICommentsRepository CommentsRepository
        {
            get { return _commentsRepository ??= new CommentsRepository(_context); }
        }
        

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
            throw new InvalidDataException("An error occurred while saving the entity changes", ex);
        }
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _context.Database.BeginTransaction();
    }
}