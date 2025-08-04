using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateModels;

public class EntityTemplateModel : Drop
{
    public string Name { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public List<PropertyTemplateModel> Properties { get; set; } = new();
    public string? Note { get; set; }
    public string Namespace { get; set; } = string.Empty;
}
