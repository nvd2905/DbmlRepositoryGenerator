using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateModels;

public class RepositoryTemplateModel : Drop
{
    public string EntityName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string InterfaceName { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public bool GenerateAsync { get; set; }
    public bool GenerateInterfaces { get; set; }
}
