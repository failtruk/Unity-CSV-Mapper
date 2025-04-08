using System;
using System.Collections.Generic;

[Serializable]
public class NestedFieldMappingSerializable
{
    public string ListFieldName;
    public List<FieldMapping> NestedMappings = new List<FieldMapping>();
    public bool IsExpanded = false;
}
