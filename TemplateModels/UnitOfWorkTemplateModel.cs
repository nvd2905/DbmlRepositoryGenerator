using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateModels;

public class UnitOfWorkTemplateModel : Drop
{
    public List<UnitOfWorkRepositoryModel> Repositories { get; set; } = new();
    public string Namespace { get; set; } = string.Empty;
    public bool GenerateAsync { get; set; }
    public bool GenerateInterfaces { get; set; }
}