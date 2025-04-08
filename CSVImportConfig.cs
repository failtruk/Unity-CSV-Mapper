using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CSVImportConfig", menuName = "CSV Importer/Import Configuration", order = 1)]
public class CSVImportConfig : ScriptableObject
{
    public string csvURL;
    public List<FieldMapping> flatFieldMappings = new List<FieldMapping>();
    public List<NestedFieldMappingSerializable> nestedFieldMappings = new List<NestedFieldMappingSerializable>();
}
