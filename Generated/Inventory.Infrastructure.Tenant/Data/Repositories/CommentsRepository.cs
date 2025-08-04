//************************************************************************
//*         TRADE SECRET MATERIAL OF PENTANA SOLUTIONS PTY LTD           *
//*             TRADE SECRET MATERIAL SUBJECT TO LICENCE                 *
//************************************************************************

using Inventory.Application.Common.Interfaces;

namespace Inventory.Infrastructure.Tenant.Data.Repositories;

namespace Inventory.Repositories;

public class CommentsRepository : BaseRepository<CommentsRepository, int>, ICommentsRepository
{
    private readonly ApplicationTenantDbContext _tenantDbContext;
    public CommentsRepository(ApplicationTenantDbContext tenantDbContext) : base(tenantDbContext)
    {
        _tenantDbContext = tenantDbContext;
    }
}