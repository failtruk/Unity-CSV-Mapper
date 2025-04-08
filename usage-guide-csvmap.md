# CSV Importer - Usage Guide

This guide provides step-by-step instructions on how to use the CSV Importer tool in your Unity project.

## Table of Contents

1. [Opening the Importer](#opening-the-importer)
2. [Basic Workflow](#basic-workflow)
3. [Flat Import vs. Nested Import](#flat-import-vs-nested-import)
4. [Setting Up CSV Mappings](#setting-up-csv-mappings)
5. [Managing Configurations](#managing-configurations)
6. [Working with Remote Data](#working-with-remote-data)
7. [Tips and Best Practices](#tips-and-best-practices)
8. [Troubleshooting](#troubleshooting)

## Opening the Importer

To open the CSV Importer tool:

1. In the Unity Editor, navigate to the top menu
2. Click on **Tools > CSV Importer**
3. The CSV Importer window will appear

## Basic Workflow

The general workflow for using the CSV Importer follows these steps:

1. **Prepare Your ScriptableObject** - Create a ScriptableObject class that will store your data
2. **Prepare Your CSV** - Create a CSV file with appropriate headers matching field names
3. **Open the Importer** - Launch the CSV Importer window
4. **Add ScriptableObject** - Drag and drop your ScriptableObject into the window
5. **Load CSV** - Enter the URL to your CSV file and load it
6. **Map Fields** - Map CSV columns to ScriptableObject fields
7. **Import Data** - Click the Import button to process the data
8. **Save Configuration** (Optional) - Save your mapping for later use

## Flat Import vs. Nested Import

The CSV Importer supports two different import modes:

### Flat Import

In flat import mode, each row in your CSV becomes a new ScriptableObject instance. This is useful when:

- Each CSV row represents a complete and independent entity
- You want to create multiple ScriptableObject assets
- Your data doesn't have complex nested structures

Example:
```csharp
[CreateAssetMenu(fileName = "Character", menuName = "Game/Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public int health;
    public float speed;
    public CharacterClass characterClass;
}
```

### Nested Import

In nested import mode, each row in your CSV becomes an element in a list within a single ScriptableObject. This is useful when:

- You want to maintain a collection of related items
- Your game needs a central database or registry
- You prefer managing a single asset over multiple assets

Example:
```csharp
[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Game/Character Database")]
public class CharacterDatabase : ScriptableObject
{
    public List<CharacterData> characters = new List<CharacterData>();
}

[Serializable]
public class CharacterData
{
    public string characterName;
    public int health;
    public float speed;
    public CharacterClass characterClass;
}
```

## Setting Up CSV Mappings

To map CSV columns to your ScriptableObject fields:

1. After loading your CSV, you'll see a list of fields from your ScriptableObject
2. For each field, select the corresponding CSV column from the dropdown
3. For nested fields, expand the list field and map the columns to the nested fields

**Important Notes:**
- The tool will attempt to convert string values from the CSV to the appropriate types
- Enum values must match the enum names exactly (case-sensitive)
- Unity Object references cannot be automatically resolved and will be set to null

## Managing Configurations

Save time by managing import configurations:

### Saving a Configuration

1. After setting up your mappings, click "Save Configuration"
2. Choose a location and name for your configuration asset
3. The configuration will be saved as a ScriptableObject asset

### Loading a Configuration

1. Click "Load Configuration"
2. Select your previously saved configuration asset
3. Your mappings will be automatically restored

This is particularly useful for:
- Regular data updates
- Team members who need to perform the same import
- Complex mapping setups that you want to reuse

## Working with Remote Data

The CSV Importer can load data from remote URLs:

1. Enter the full URL to your CSV file in the "CSV URL" field
2. Click "Load CSV Columns" to fetch and parse the data
3. The tool will download the CSV and process it

**Requirements:**
- The URL must be accessible from your development machine
- The CSV should be properly formatted with headers
- Network connectivity is required during the import process

## Tips and Best Practices

For the best experience with the CSV Importer:

1. **Consistent Headers** - Ensure your CSV headers are consistent and match field names where possible
2. **Data Types** - Keep data types consistent in your CSV (don't mix numbers and text in the same column)
3. **Special Characters** - Be cautious with special characters in CSV; use quotes when necessary
4. **Field Names** - Use clear, descriptive field names in your ScriptableObjects
5. **Backup** - Always backup your project before performing large imports
6. **Version Control** - Commit your configuration assets to version control for team sharing
7. **Folder Structure** - Organize your imported assets in a logical folder structure

## Troubleshooting

### Common Issues and Solutions

**CSV Not Loading**
- Check your internet connection
- Verify the URL is correct and accessible
- Ensure the CSV is properly formatted

**Mapping Issues**
- Confirm that your CSV headers match what you expect
- Check for hidden characters or spaces in headers
- Make sure the right CSV column is selected for each field

**Type Conversion Errors**
- Check that the data in your CSV matches the expected types
- Look for special formatting in number fields (commas, currency symbols)
- Ensure enum values match exactly (case-sensitive)

**Import Failures**
- Check the Unity Console for specific error messages
- Verify write permissions to your project folders
- Make sure all required fields have mappings

### Unity Console Messages

Pay attention to the Unity Console for helpful messages:

- **Info Messages** - Indicate successful operations
- **Warning Messages** - Highlight potential issues that might affect data
- **Error Messages** - Show problems that prevented successful import

If you encounter persistent issues, check the GitHub repository for updates or open an issue with details about your problem.
