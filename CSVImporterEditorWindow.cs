using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;

public class CSVImporterEditorWindow : EditorWindow
{
    private ScriptableObject droppedObject;
    private string csvURL = "";
    private string assetFolderPath = "Assets/DataObjects";
    private List<string> csvColumns = new List<string>();
    private IDataParser dataParser;
    private string csvDataText = "";

    private List<NestedFieldMapping> nestedFieldMappings = new List<NestedFieldMapping>();
    private FieldMappingCollection fieldMappings = new FieldMappingCollection();

    // Variable for configuration
    private CSVImportConfig currentConfig;

    [MenuItem("Tools/CSV Importer")]
    public static void ShowWindow()
    {
        GetWindow<CSVImporterEditorWindow>("CSV Importer");
    }

    private void OnGUI()
    {
        DrawDropArea();

        if (droppedObject != null)
        {
            DrawCSVLoadSection();
            DrawConfigSaveLoadSection();

            if (csvColumns.Count > 0)
            {
                DrawFlatFieldMappingSection();
                DrawNestedMappingSection();
                if (GUILayout.Button("Import CSV Data"))
                {
                    if (nestedFieldMappings.Count > 0)
                    {
                        ImportNestedCSVData();
                    }
                    else
                    {
                        ImportFlatCSVData();
                    }
                }
            }
        }
    }

    // Draw the area where users can drag and drop a ScriptableObject
    private void DrawDropArea()
    {
        GUILayout.Label("Drag and Drop Scriptable Object");
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drop ScriptableObject Here");

        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition)) break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        droppedObject = DragAndDrop.objectReferences[0] as ScriptableObject;
                        if (droppedObject != null)
                        {
                            GetFieldsOfScriptableObject(droppedObject);
                        }
                        else
                        {
                            Debug.LogError("Dropped object is not a ScriptableObject.");
                        }
                    }
                }
                break;
        }
        GUILayout.Space(20);
    }

    // Draw the section for loading CSV data from a URL
    private void DrawCSVLoadSection()
    {
        GUILayout.Label($"ScriptableObject Type: {droppedObject.GetType().Name}");
        csvURL = EditorGUILayout.TextField("CSV URL", csvURL);

        if (GUILayout.Button("Load CSV Columns"))
        {
            if (string.IsNullOrEmpty(csvURL))
            {
                Debug.LogError("Please provide a valid CSV URL.");
            }
            else
            {
                EditorCoroutineUtility.StartCoroutine(FetchCSVData(), this);
            }
        }
    }

    // Draw the configuration save/load section
    private void DrawConfigSaveLoadSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Configuration");

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Configuration"))
        {
            SaveConfiguration();
        }
        if (GUILayout.Button("Load Configuration"))
        {
            LoadConfiguration();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SaveConfiguration()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save CSV Import Configuration", "CSVImportConfig", "asset", "Specify where to save the configuration.");
        if (!string.IsNullOrEmpty(path))
        {
            // Create a new configuration asset
            CSVImportConfig config = ScriptableObject.CreateInstance<CSVImportConfig>();
            config.csvURL = csvURL;

            // Copy flat field mappings
            config.flatFieldMappings = new List<FieldMapping>(fieldMappings.Mappings);

            // Copy nested field mappings
            foreach (var nestedMapping in nestedFieldMappings)
            {
                NestedFieldMappingSerializable serializableMapping = new NestedFieldMappingSerializable
                {
                    ListFieldName = nestedMapping.ListField.Name,
                    IsExpanded = nestedMapping.IsExpanded,
                    NestedMappings = new List<FieldMapping>(nestedMapping.NestedMappings)
                };
                config.nestedFieldMappings.Add(serializableMapping);
            }

            // Save the asset
            AssetDatabase.CreateAsset(config, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Assign the current configuration
            currentConfig = config;

            Debug.Log("Configuration saved successfully.");
        }
    }

    private void LoadConfiguration()
    {
        string path = EditorUtility.OpenFilePanel("Load CSV Import Configuration", Application.dataPath, "asset");
        if (!string.IsNullOrEmpty(path))
        {
            path = FileUtil.GetProjectRelativePath(path);
            currentConfig = AssetDatabase.LoadAssetAtPath<CSVImportConfig>(path);

            if (currentConfig != null)
            {
                // Assign the URL
                csvURL = currentConfig.csvURL;

                // Load flat field mappings
                fieldMappings.Mappings = new List<FieldMapping>(currentConfig.flatFieldMappings);

                // Load nested field mappings
                nestedFieldMappings.Clear();
                foreach (var serializableMapping in currentConfig.nestedFieldMappings)
                {
                    FieldInfo listField = droppedObject.GetType().GetField(serializableMapping.ListFieldName);
                    if (listField != null)
                    {
                        NestedFieldMapping nestedMapping = new NestedFieldMapping(listField)
                        {
                            IsExpanded = serializableMapping.IsExpanded,
                            NestedMappings = new List<FieldMapping>(serializableMapping.NestedMappings)
                        };
                        nestedFieldMappings.Add(nestedMapping);
                    }
                    else
                    {
                        Debug.LogError($"List field '{serializableMapping.ListFieldName}' not found in the ScriptableObject.");
                    }
                }

                // Refresh the field mappings
                GetFieldsOfScriptableObject(droppedObject);

                // Repaint the window
                Repaint();

                Debug.Log("Configuration loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to load configuration.");
            }
        }
    }

    // Draw the field mapping section for flat (non-nested) fields
    private void DrawFlatFieldMappingSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Map CSV Columns to ScriptableObject Fields");

        foreach (FieldMapping mapping in fieldMappings.Mappings)
        {
            int selectedCsvIndex = csvColumns.IndexOf(mapping.CsvColumn);
            if (selectedCsvIndex < 0)
            {
                selectedCsvIndex = 0;
            }

            // Create a dropdown to map CSV columns to fields in the ScriptableObject
            selectedCsvIndex = EditorGUILayout.Popup(
                $"Map CSV Column to {mapping.FieldName}",
                selectedCsvIndex,
                csvColumns.ToArray()
            );

            if (csvColumns.Count > 0)
            {
                mapping.CsvColumn = csvColumns[selectedCsvIndex];
            }
        }
    }

    // Draw the field mapping section for nested fields (e.g., lists within the ScriptableObject)
    private void DrawNestedMappingSection()
    {
        GUILayout.Space(10);
        GUILayout.Label("Nested Field Mappings");

        foreach (var nestedField in nestedFieldMappings)
        {
            bool isExpanded = EditorGUILayout.Foldout(nestedField.IsExpanded, $"List Field: {nestedField.ListField.Name}");
            nestedField.IsExpanded = isExpanded;

            if (isExpanded)
            {
                EditorGUI.indentLevel++;
                foreach (FieldMapping nestedMapping in nestedField.NestedMappings)
                {
                    int selectedNestedFieldIndex = csvColumns.IndexOf(nestedMapping.CsvColumn);
                    if (selectedNestedFieldIndex < 0) selectedNestedFieldIndex = 0;
                    selectedNestedFieldIndex = EditorGUILayout.Popup(
                        $"Map CSV Column to {nestedMapping.FieldName}",
                        selectedNestedFieldIndex,
                        csvColumns.ToArray()
                    );
                    nestedMapping.CsvColumn = csvColumns[selectedNestedFieldIndex];
                }
                EditorGUI.indentLevel--;
            }
        }
    }

    // Get the fields of the ScriptableObject and initialize field mappings
    private void GetFieldsOfScriptableObject(ScriptableObject obj)
    {
        fieldMappings.Clear();
        nestedFieldMappings.Clear();

        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        foreach (FieldInfo field in fields)
        {
            if (IsListOfSerializableClass(field))
            {
                var nestedMapping = nestedFieldMappings.Find(nm => nm.ListField.Name == field.Name);
                if (nestedMapping == null)
                {
                    nestedMapping = new NestedFieldMapping(field);
                    nestedMapping.AddMappings(GetSerializableFields(GetListElementType(field)), csvColumns);
                    nestedFieldMappings.Add(nestedMapping);
                }
                else
                {
                    nestedMapping.ListField = field;
                }
            }
            else
            {
                var existingMapping = fieldMappings.Mappings.Find(fm => fm.FieldName == field.Name);
                if (existingMapping == null)
                {
                    fieldMappings.AddMapping(field.Name, csvColumns);
                }
            }
        }
    }

    // Fetch CSV data from the provided URL
    private IEnumerator FetchCSVData()
    {
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
    }

    private void ParseCSVData()
    {
        dataParser = new CSVDataParser();
        dataParser.Parse(csvDataText);
        csvColumns = dataParser.GetColumns();

        // Update flat field mappings with CSV columns
        foreach (FieldMapping mapping in fieldMappings.Mappings)
        {
            if (!csvColumns.Contains(mapping.CsvColumn))
            {
                mapping.CsvColumn = csvColumns.Count > 0 ? csvColumns[0] : "";
            }
        }

        // Update nested field mappings with CSV columns
        foreach (var nestedField in nestedFieldMappings)
        {
            foreach (var nestedMapping in nestedField.NestedMappings)
            {
                if (!csvColumns.Contains(nestedMapping.CsvColumn))
                {
                    nestedMapping.CsvColumn = csvColumns.Count > 0 ? csvColumns[0] : "";
                }
            }
        }
    }

    // Check if a field is a list of a serializable class
    private bool IsListOfSerializableClass(FieldInfo field)
    {
        if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type elementType = field.FieldType.GetGenericArguments()[0];
            return elementType.IsClass && elementType.IsSerializable;
        }
        return false;
    }

    // Get the element type of a list field
    private Type GetListElementType(FieldInfo field)
    {
        if (IsListOfSerializableClass(field))
        {
            return field.FieldType.GetGenericArguments()[0];
        }
        return null;
    }

    // Get the serializable fields of a given type
    private List<string> GetSerializableFields(Type type)
    {
        List<string> serializableFields = new List<string>();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        foreach (FieldInfo field in fields)
        {
            if (field.IsPublic && (field.FieldType.IsPrimitive || field.FieldType == typeof(string) || field.FieldType.IsEnum || typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType)))
            {
                serializableFields.Add(field.Name);
            }
        }
        return serializableFields;
    }

    // Import CSV data to flat fields in the ScriptableObject
    private void ImportFlatCSVData()
    {
        var dataRows = dataParser.GetRows();

        foreach (var dataRow in dataRows)
        {
            ScriptableObject newObj = ScriptableObject.CreateInstance(droppedObject.GetType());
            foreach (var mapping in fieldMappings.Mappings)
            {
                if (dataRow.ContainsKey(mapping.CsvColumn))
                {
                    FieldInfo fieldInfo = newObj.GetType().GetField(mapping.FieldName);
                    if (fieldInfo != null)
                    {
                        object value = ConvertValue(dataRow[mapping.CsvColumn], fieldInfo.FieldType);
                        fieldInfo.SetValue(newObj, value);
                    }
                }
            }

            // Save the new ScriptableObject as an asset
            string assetPath = $"{assetFolderPath}/{droppedObject.GetType().Name}_{Guid.NewGuid()}.asset";
            AssetDatabase.CreateAsset(newObj, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Flat CSV data imported successfully as separate ScriptableObject assets.");
    }

    // Import CSV data into nested fields (lists) in the ScriptableObject
    private void ImportNestedCSVData()
    {
        var dataRows = dataParser.GetRows();

        foreach (var dataRow in dataRows)
        {
            foreach (var nestedField in nestedFieldMappings)
            {
                IList list = (IList)nestedField.ListField.GetValue(droppedObject);
                if (list == null)
                {
                    // Create a new list if the existing one is null
                    list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(GetListElementType(nestedField.ListField)));
                    nestedField.ListField.SetValue(droppedObject, list);
                }

                var nestedObj = Activator.CreateInstance(GetListElementType(nestedField.ListField));
                foreach (var nestedMapping in nestedField.NestedMappings)
                {
                    if (dataRow.ContainsKey(nestedMapping.CsvColumn))
                    {
                        FieldInfo nestedFieldInfo = nestedObj.GetType().GetField(nestedMapping.FieldName);
                        if (nestedFieldInfo != null)
                        {
                            object value = ConvertValue(dataRow[nestedMapping.CsvColumn], nestedFieldInfo.FieldType);
                            nestedFieldInfo.SetValue(nestedObj, value);
                        }
                    }
                }
                list.Add(nestedObj);
            }
        }

        EditorUtility.SetDirty(droppedObject);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Nested CSV data imported successfully into the ScriptableObject.");
    }

    // Convert a string value to the specified target type
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

    [Serializable]
    public class FieldMappingCollection
    {
        public List<FieldMapping> Mappings = new List<FieldMapping>();
        public List<string> ObjectFields = new List<string>();

        public void Clear()
        {
            Mappings.Clear();
            ObjectFields.Clear();
        }

        public void AddMapping(string fieldName, List<string> csvColumns)
        {
            string defaultCsvColumn = csvColumns.Count > 0 ? csvColumns[0] : "";
            Mappings.Add(new FieldMapping { FieldName = fieldName, CsvColumn = defaultCsvColumn });
        }
    }

    // IDataParser and CSVDataParser classes
    public interface IDataParser
    {
        void Parse(string data);
        List<string> GetColumns();
        List<Dictionary<string, string>> GetRows();
    }

    public class CSVDataParser : IDataParser
    {
        private List<string> columns = new List<string>();
        private List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();

        public void Parse(string data)
        {
            columns.Clear();
            rows.Clear();

            if (string.IsNullOrEmpty(data))
            {
                Debug.LogError("CSV data is empty.");
                return;
            }

            string[] lines = data.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                Debug.LogError("CSV data is empty.");
                return;
            }

            string headerLine = lines[0];
            columns.AddRange(ParseCSVLine(headerLine));

            for (int i = 1; i < lines.Length; i++)
            {
                string[] values = ParseCSVLine(lines[i]);
                var row = new Dictionary<string, string>();
                for (int j = 0; j < columns.Count; j++)
                {
                    string value = j < values.Length ? values[j] : "";
                    row[columns[j]] = value;
                }
                rows.Add(row);
            }
        }

        public List<string> GetColumns()
        {
            return columns;
        }

        public List<Dictionary<string, string>> GetRows()
        {
            return rows;
        }

        // Parse a line of CSV, handling quoted strings properly
        private string[] ParseCSVLine(string line)
        {
            List<string> values = new List<string>();
            bool inQuotes = false;
            string value = "";

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(value);
                    value = "";
                }
                else
                {
                    value += c;
                }
            }

            values.Add(value);

            return values.ToArray();
        }
    }
}
