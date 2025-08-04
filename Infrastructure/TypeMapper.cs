using DbmlRepositoryGenerator.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.Infrastructure;

// Type mapping utility (same as before)
public static class TypeMapper
{
    private static readonly Dictionary<string, string> SqlServerTypes = new()
        {
            { "int", "int" },
            { "bigint", "long" },
            { "smallint", "short" },
            { "tinyint", "byte" },
            { "bit", "bool" },
            { "decimal", "decimal" },
            { "numeric", "decimal" },
            { "money", "decimal" },
            { "float", "double" },
            { "real", "float" },
            { "datetime", "DateTime" },
            { "datetime2", "DateTime" },
            { "date", "DateTime" },
            { "time", "TimeSpan" },
            { "varchar", "string" },
            { "nvarchar", "string" },
            { "char", "string" },
            { "nchar", "string" },
            { "text", "string" },
            { "ntext", "string" },
            { "uniqueidentifier", "Guid" },
            { "varbinary", "byte[]" },
            { "binary", "byte[]" },
            { "image", "byte[]" }
        };

    public static string MapDbTypeToClrType(string dbType, DatabaseProvider provider = DatabaseProvider.SqlServer)
    {
        dbType = dbType.ToLower();

        // Remove size specifications
        dbType = Regex.Replace(dbType, @"\(\d+(?:,\d+)?\)", "");

        return provider switch
        {
            DatabaseProvider.SqlServer => SqlServerTypes.GetValueOrDefault(dbType, "string"),
            _ => SqlServerTypes.GetValueOrDefault(dbType, "string")
        };
    }

    public static bool IsNullableType(string clrType)
    {
        return clrType != "string" && clrType != "byte[]";
    }
}
