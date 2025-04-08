# CSV Importer - Code Overview

This document provides a technical overview of the CSV Importer codebase, explaining how the different components work together.

## Core Components

The CSV Importer is built from several key components that work together:

### 1. CSVImporterEditorWindow

The main Unity Editor window that provides the user interface. Key responsibilities:

- Presenting UI for drag & drop, CSV loading, and field mapping
- Managing the connection between CSV columns and ScriptableObject fields
- Handling the actual data import process
- Saving and loading configurations

### 2. CSVImportConfig

A ScriptableObject that stores import configurations for reuse:

- Stores the CSV URL
- Maintains mappings between CSV columns and ScriptableObject fields
- Preserves both flat and nested field mappings

### 3. FieldMapping

A serializable class that represents a simple mapping between:

- A CSV column name
- A field name in a ScriptableObject

### 4. NestedFieldMapping

Handles more complex mappings for list fields within ScriptableObjects:

- References a List field in the ScriptableObject
- Contains child FieldMapping objects for the fields of the list element type
- Manages UI expansion state

### 5. NestedFieldMappingSerializable

A serializable version of NestedFieldMapping used for configuration storage:

- Stores the list field name as a string (instead of FieldInfo which can't be serialized)
- Contains the same nested mappings and expansion state

### 6. CSVDataParser

Implements the IDataParser interface to handle CSV parsing:

- Parses header row to determine available columns
- Processes data rows into dictionaries for easy field access
- Handles CSV specifics like comma separation and quoted values

## Data Flow

Understanding how data flows through the system is key to working with the CSV Importer:

1. **Configuration Setup**:
   - User drags a ScriptableObject into the window
   - ScriptableObject fields are analyzed to create mapping options
   - User provides a CSV URL and loads the data
   - CSV header row is parsed to determine available columns
   - User maps CSV columns to ScriptableObject fields

2. **Import Process (Flat Mode)**:
   - For each row in the CSV data:
     - A new ScriptableObject instance is created
     - Values from the CSV are converted to appropriate types
     - Fields in the ScriptableObject are populated
     - The new ScriptableObject is saved as an asset

3. **Import Process (Nested Mode)**:
   - For each row in the CSV data:
     - A new instance of the list element type is created
     - Values from the CSV are converted to appropriate types
     - Fields in the list element are populated
     - The new element is added to the list in the ScriptableObject
   - The modified ScriptableObject is saved

## Reflection Usage

The CSV Importer makes heavy use of reflection to:

- Discover fields in ScriptableObjects and their types
- Identify list fields that can contain nested objects
- Get and set field values during the import process
- Create new instances of types for lists

```csharp
// Example of reflection to get fields
FieldInfo[] fields = obj.GetType().GetFields(
    BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
);

// Example of reflection to set a field value
fieldInfo.SetValue(newObj, value);
```

## Type Conversion

The `ConvertValue` method handles converting string values from the CSV to the appropriate types for ScriptableObject fields:

```csharp
private object ConvertValue(string stringValue, Type targetType)
{
    try
    {
        if (targetType == typeof(int)) return int.Parse(stringValue);
        if (targetType == typeof(float)) return float.Parse(stringValue);
        if (targetType == typeof(bool)) return bool.Parse(stringValue);
        if (targetType == typeof(string)) return stringValue;
        if (targetType.IsEnum) return Enum.Parse(targetType, stringValue);
        if (typeof(UnityEngine.Object).IsAssignableFrom(targetType))
        {
            return null;
        }
    }
    catch (Exception e)
    {
        Debug.LogError($"Error converting value '{stringValue}' to type {targetType.Name}: {e.Message}");
    }
    return null;
}
```

## Extension Points

The system has several points where it can be extended:

1. **IDataParser Interface**: Implement this interface to support additional data formats beyond CSV.

2. **ConvertValue Method**: Modify this method to support additional data types or custom conversion logic.

3. **New Field Types**: The system can be extended to handle more complex field types with custom handling.

## Web Requests

The CSV Importer uses Unity's `UnityWebRequest` to fetch CSV data from URLs:

```csharp
using (UnityWebRequest www = UnityWebRequest.Get(csvURL))
{
    yield return www.SendWebRequest();
    
    if (www.result != UnityWebRequest.Result.Success)
    {
        Debug.LogError($"Failed to download CSV: {www.error}");
    }
    else
    {
        csvDataText = www.downloadHandler.text;
        ParseCSVData();
    }
}
```

This allows the tool to work with both local and remote data sources.
