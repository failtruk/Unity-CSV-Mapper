# Unity CSV Importer

![CSV Importer Banner](https://via.placeholder.com/1200x300.png?text=Unity+CSV+Importer)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Unity Version](https://img.shields.io/badge/Unity-2019.4%2B-blue.svg)](https://unity.com/)

A powerful and flexible CSV importing solution for Unity that enables developers to seamlessly import data from CSV files into ScriptableObjects. Perfect for game designers and developers who need to manage large amounts of game data.

## 🚀 Features

- **Easy-to-use Editor Interface** - Drag and drop ScriptableObjects and import with just a few clicks
- **Flexible Mapping** - Map CSV columns to ScriptableObject fields with a visual interface
- **Support for Nested Data** - Import data into lists of custom classes within ScriptableObjects
- **Configuration Management** - Save and load import configurations for repeated use
- **Type Conversion** - Automatic conversion between CSV string values and various data types
- **Remote Data Support** - Import data from URLs, making it easy to update from remote sources

## 📋 Requirements

- Unity 2019.4 or newer
- Unity Editor Coroutines package (com.unity.editorcoroutines)

## 📥 Installation

### Option 1: Unity Package Manager (Recommended)

1. Open the Package Manager window in Unity (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter `https://github.com/failtruk/unity-csv-mapper.git`
6. Click "Add"

### Option 2: Manual Installation

1. Download or clone this repository
2. Copy the contents into your Unity project's Assets folder

## 🔍 Quick Preview

![CSV Importer Screenshot](https://via.placeholder.com/800x450.png?text=CSV+Importer+Screenshot)

## 🛠️ How It Works

The CSV Importer provides a simple workflow:

1. Open the tool (Tools > CSV Importer)
2. Drag your ScriptableObject into the window
3. Enter the URL to your CSV file
4. Map the CSV columns to your ScriptableObject fields
5. Import the data

For detailed instructions, check out the [Documentation](DOCUMENTATION.md).

## 💡 Use Cases

- **Game Data Management** - Import item, character, level data directly from spreadsheets
- **Localization** - Keep text and translations in external files for easy updates
- **Configuration** - Store game balance and configuration values in accessible formats
- **Live Service Games** - Update game content remotely without rebuilding the game
- **Team Collaboration** - Allow designers to modify data without touching code or Unity

## 📖 Example

```csharp
// Define a ScriptableObject for your game items
[CreateAssetMenu(fileName = "ItemData", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int cost;
    public float weight;
    public ItemType itemType;
}

// Create a CSV file with headers matching these field names:
// itemName,cost,weight,itemType
// "Magic Sword",100,5.5,Weapon
// "Health Potion",25,0.3,Consumable
// ...
```

Then use the CSV Importer to map these columns and import the data with just a few clicks!

## 📚 Documentation

For complete documentation, see:

- [User Guide](usage-guide-csvmap.md)
- [Code Overview](code-overview.md)
- [API Reference](github-documentation.md)

## ✅ To-Do List

- [ ] Add support for JSON data
- [ ] Implement asset reference resolution
- [ ] Add batch processing for multiple files
- [ ] Create runtime API for dynamic data loading
- [ ] Add data validation rules

## 🤝 Contributing

Contributions are welcome! Feel free to submit issues or pull requests.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- Thanks to Unity for providing the editor extensibility that makes this tool possible
- Inspired by the need for better data management solutions in game development
