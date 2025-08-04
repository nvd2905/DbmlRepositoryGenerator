using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateModels;

public class PropertyTemplateModel : Drop
{
    public string Name { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string ClrType { get; set; } = string.Empty;
    public bool IsPrimaryKey { get; set; }
    public bool IsNotNull { get; set; }
    public bool IsUnique { get; set; }
    public bool IsIncrement { get; set; }
    public bool IsNullable { get; set; }
    public string? DefaultValue { get; set; }
    public string? Note { get; set; }
}
