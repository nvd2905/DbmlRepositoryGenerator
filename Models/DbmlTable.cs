using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.Models;

public class DbmlTable
{
    public string Name { get; set; } = string.Empty;
    public List<DbmlColumn> Columns { get; set; } = new();
    public List<DbmlIndex> Indexes { get; set; } = new();
    public string? Note { get; set; }
}
