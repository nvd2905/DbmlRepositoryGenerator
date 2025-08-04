//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Microsoft.EntityFrameworkCore.Storage;

namespace Inventory.Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    #region Repository
    
        IUsersRepository UsersRepository { get; }
        
        IPostsRepository PostsRepository { get; }
        
        ICommentsRepository CommentsRepository { get; }
        
    #endregion

    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    IDbContextTransaction BeginTransaction();
}
