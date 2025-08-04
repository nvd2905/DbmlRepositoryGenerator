using DbmlRepositoryGenerator.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.Infrastructure;

public class GeneratorOptions
{
    public string Namespace { get; set; } = "Inventory";
    public string OutputDirectory { get; set; } = "./Generated";
    public string TemplateDirectory { get; set; } = "./Templates";
    public bool GenerateAsync { get; set; } = true;
    public bool GenerateInterfaces { get; set; } = true;
    public string ConnectionStringName { get; set; } = "DefaultConnection";
    public DatabaseProvider Provider { get; set; } = DatabaseProvider.SqlServer;
    public Dictionary<string, object> CustomVariables { get; set; } = new();
}