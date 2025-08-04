using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.TemplateModels;

public class RelationshipTemplateModel : Drop
{
    public string FromEntity { get; set; } = string.Empty;
    public string ToEntity { get; set; } = string.Empty;
    public string FromColumn { get; set; } = string.Empty;
    public string ToColumn { get; set; } = string.Empty;
    public string RelationType { get; set; } = string.Empty;
}
