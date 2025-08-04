using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.Models;

public class DbmlSchema
{
    public List<DbmlTable> Tables { get; set; } = new();
    public List<DbmlReference> References { get; set; } = new();
}
