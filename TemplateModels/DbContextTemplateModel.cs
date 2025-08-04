using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateModels;

public class DbContextTemplateModel : Drop
{
    public List<DbContextEntityModel> Entities { get; set; } = new();
    public List<RelationshipTemplateModel> Relationships { get; set; } = new();
    public string Namespace { get; set; } = string.Empty;
    public string ClassName { get; set; } = "ApplicationDbContext";
}