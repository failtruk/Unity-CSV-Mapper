using System;
using System.Collections.Generic;
using System.Reflection;

[Serializable]
public class NestedFieldMapping
{
    public FieldInfo ListField;
    public List<FieldMapping> NestedMappings = new List<FieldMapping>();
    public bool IsExpanded = false;

    public NestedFieldMapping(FieldInfo listField)
    {
        ListField = listField;
    }

    public void AddMappings(List<string> fields, List<string> csvColumns)
    {
        foreach (var field in fields)
        {
            string defaultCsvColumn = csvColumns.Count > 0 ? csvColumns[0] : "";
            NestedMappings.Add(new FieldMapping { FieldName = field, CsvColumn = defaultCsvColumn });
        }
    }
}
