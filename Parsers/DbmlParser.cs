using DbmlRepositoryGenerator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DbmlRepositoryGenerator.Parsers;

public class DbmlParser
{
    public DbmlSchema ParseDbml(string dbmlContent)
    {
        var schema = new DbmlSchema();
        var lines = dbmlContent.Split('\n').Select(l => l.Trim()).ToArray();

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (i == 11)
            {
                Console.Write(line);
            }
            if (line.StartsWith("Table "))
            {
                var table = ParseTable(lines, ref i);
                schema.Tables.Add(table);
            }
            else if (line.StartsWith("Ref:"))
            {
                var reference = ParseReference(line);
                if (reference != null)
                    schema.References.Add(reference);
            }
        }

        return schema;
    }

    private DbmlTable ParseTable(string[] lines, ref int index)
    {
        var table = new DbmlTable();
        var tableLine = lines[index];

        // Extract table name
        var tableMatch = Regex.Match(tableLine, @"Table\s+(\w+)");
        if (tableMatch.Success)
            table.Name = tableMatch.Groups[1].Value;

        //index++; // Move to next line

        // Skip opening brace
        while (index < lines.Length && !lines[index].Contains("{"))
            index++;
        index++;

        // Parse columns
        while (index < lines.Length && !lines[index].Contains("}"))
        {
            var line = lines[index].Trim();
            if (!string.IsNullOrEmpty(line) && !line.StartsWith("//"))
            {
                if (line.StartsWith("Indexes"))
                {
                    index++;
                    while (index < lines.Length && !lines[index].Contains("}"))
                    {
                        var indexLine = lines[index].Trim();
                        if (!string.IsNullOrEmpty(indexLine))
                        {
                            var dbmlIndex = ParseIndex(indexLine);
                            if (dbmlIndex != null)
                                table.Indexes.Add(dbmlIndex);
                        }
                        index++;
                    }
                }
                else
                {
                    var column = ParseColumn(line);
                    if (column != null)
                        table.Columns.Add(column);
                }
            }
            index++;
        }

        return table;
    }

    private DbmlColumn? ParseColumn(string line)
    {
        var column = new DbmlColumn();

        // Parse column definition: name type [attributes]
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2) return null;

        column.Name = parts[0];
        column.Type = parts[1];

        // Parse attributes in brackets
        var attributeMatch = Regex.Match(line, @"\[(.*?)\]");
        if (attributeMatch.Success)
        {
            var attributes = attributeMatch.Groups[1].Value.Split(',')
                .Select(a => a.Trim().ToLower()).ToArray();

            column.IsPrimaryKey = attributes.Contains("pk") || attributes.Contains("primary key");
            column.IsNotNull = attributes.Contains("not null");
            column.IsUnique = attributes.Contains("unique");
            column.IsIncrement = attributes.Contains("increment") || attributes.Contains("auto_increment");

            // Parse default value
            var defaultMatch = attributes.FirstOrDefault(a => a.StartsWith("default:"));
            if (defaultMatch != null)
                column.DefaultValue = defaultMatch.Substring(8).Trim();
        }

        return column;
    }

    private DbmlIndex? ParseIndex(string line)
    {
        var index = new DbmlIndex();

        // Parse index definition
        var match = Regex.Match(line, @"(\w+)\s*\((.*?)\)");
        if (!match.Success) return null;

        index.Name = match.Groups[1].Value;
        index.Columns = match.Groups[2].Value.Split(',')
            .Select(c => c.Trim()).ToList();

        index.IsUnique = line.ToLower().Contains("unique");
        index.IsPrimaryKey = line.ToLower().Contains("pk");

        return index;
    }

    private DbmlReference? ParseReference(string line)
    {
        var reference = new DbmlReference();

        // Parse reference: Ref: table1.column1 > table2.column2
        var match = Regex.Match(line, @"Ref:\s*(\w+)\.(\w+)\s*([><-]+)\s*(\w+)\.(\w+)");
        if (!match.Success) return null;

        reference.FromTable = match.Groups[1].Value;
        reference.FromColumn = match.Groups[2].Value;
        reference.ToTable = match.Groups[4].Value;
        reference.ToColumn = match.Groups[5].Value;

        var relSymbol = match.Groups[3].Value;
        reference.RelationType = relSymbol switch
        {
            ">" => "n:1",
            "<" => "1:n",
            "-" => "1:1",
            _ => "n:1"
        };

        return reference;
    }
}
