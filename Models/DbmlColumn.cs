using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.Models;

public class DbmlColumn
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsPrimaryKey { get; set; }
    public bool IsNotNull { get; set; }
    public bool IsUnique { get; set; }
    public bool IsIncrement { get; set; }
    public string? DefaultValue { get; set; }
    public string? Note { get; set; }
}
