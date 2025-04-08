# CSV Importer for Unity

A powerful Unity Editor tool that enables developers to import CSV data directly into ScriptableObjects. This tool streamlines the process of populating game data from external sources, making it ideal for content-heavy games, live service games, or any project that requires frequent data updates.

## Features

- Import CSV data directly into ScriptableObjects
- Support for both flat and nested data structures
- Map CSV columns to ScriptableObject fields through an intuitive UI
- Save and load import configurations for repeated use
- Handle different data types (int, float, bool, string, enum)
- Process data from remote URLs
- Create multiple ScriptableObject instances or update nested collections

## Installation

1. Add this repository to your Unity project:
   - Via Package Manager: Add package from git URL `https://github.com/yourusername/csv-importer-unity.git`
   - Or clone/download this repository and copy the files into your Assets folder

2. Once imported, the CSV Importer will be available under the "Tools" menu in the Unity Editor.

## Requirements

- Unity 2019.4 or newer
- Unity Editor Coroutines package (com.unity.editorcoroutines)

## Quick Start Guide

### Basic Usage

1. Open the CSV Importer window by navigating to **Tools > CSV Importer**
2. Drag and drop a ScriptableObject into the designated area
3. Enter a URL to your CSV file and click "Load CSV Columns"
4. Map the CSV columns to the appropriate ScriptableObject fields
5. Click "Import CSV Data" to create or update your ScriptableObjects

### Working with Flat Data

For simple ScriptableObjects with primitive fields (int, float, bool, string, etc.), the importer will create a new ScriptableObject instance for each row in your CSV file.

Example ScriptableObject:
```csharp
[CreateAssetMenu(fileName = "Item", menuName = "Game/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int value;
    public float weight;
    public ItemType type;
}
```

Example CSV:
```
itemName,value,weight,type
Sword,100,5.5,Weapon
Shield,50,10.0,Armor
Potion,25,0.5,Consumable
```

### Working with Nested Data

For ScriptableObjects containing Lists of custom classes, the importer can add each CSV row as an element in the list.

Example ScriptableObject with nested data:
```csharp
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();
}

[Serializable]
public class ItemData
{
    public string itemName;
    public int value;
    public float weight;
    public ItemType type;
}
```

Using the same CSV data as above, the importer would add each row as an element in the `items` list.

## Saving and Loading Configurations

Once you've set up your CSV mappings, you can save the configuration for future use:

1. Click "Save Configuration" and choose a location to save the configuration asset
2. To reuse the configuration, click "Load Configuration" and select your saved configuration

This is particularly useful for regular data updates where the structure remains consistent.

## Advanced Usage

### Custom Data Conversion

The tool automatically handles conversion between string values from the CSV and the appropriate types in your ScriptableObjects. It supports:

- Basic types (int, float, bool, string)
- Enum values (must match the enum name exactly)
- Unity Object references (requires further setup)

### Import Modes

Two primary import modes are available:

1. **Flat Import** - Creates a new ScriptableObject for each row in the CSV
2. **Nested Import** - Adds each row as an element in a list within a single ScriptableObject

Select the appropriate mode based on your data structure.

## Troubleshooting

### Common Issues

- **CSV Not Loading**: Ensure the URL is correct and accessible. Check Unity console for network errors.
- **Mapping Issues**: Verify that CSV column names match what you expect.
- **Type Conversion Errors**: Check that the data in your CSV is compatible with your ScriptableObject field types.
- **Unity Object References**: The tool cannot automatically resolve references to other Unity objects. These fields will be set to null.

### Console Messages

The tool provides detailed logging in the Unity console to help diagnose issues:

- Success messages confirm when CSV data has been loaded or imported
- Error messages indicate what went wrong during the import process
- Warning messages alert you to potential issues that might affect your data

## Extending the Tool

The CSV Importer is designed to be extensible. Here are some ways you can customize it for your project:

### Custom Data Parsers

Implement the `IDataParser` interface to support additional data formats:

```csharp
public class CustomDataParser : IDataParser
{
    public void Parse(string data)
    {
        // Your custom parsing logic
    }
    
    public List<string> GetColumns()
    {
        // Return parsed columns
    }
    
    public List<Dictionary<string, string>> GetRows()
    {
        // Return parsed data rows
    }
}
```

### Custom Value Converters

Modify the `ConvertValue` method in `CSVImporterEditorWindow.cs` to support additional data types or custom conversion logic.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Acknowledgments

- Inspired by the need for efficient data pipelines in game development
- Thanks to the Unity Editor Coroutines package for enabling asynchronous operations
