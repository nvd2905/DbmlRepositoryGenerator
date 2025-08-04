using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateModels;

public class DbContextEntityModel : Drop
{
    public string EntityName { get; set; } = string.Empty;
    public string DbSetName { get; set; } = string.Empty;
}
