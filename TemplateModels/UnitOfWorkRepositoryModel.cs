using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateModels;

public class UnitOfWorkRepositoryModel : Drop
{
    public string EntityName { get; set; } = string.Empty;
    public string RepositoryName { get; set; } = string.Empty;
    public string InterfaceName { get; set; } = string.Empty;
    public string FieldName { get; set; } = string.Empty;
}
