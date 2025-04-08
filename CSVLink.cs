using System;

[System.Serializable]
public class CSVLink
{
    public string csvURL;  // URL to the CSV file
    public Type scriptableObjectType;  // The type of ScriptableObject this CSV updates
    public string assetFolderPath;  // Folder path to save the ScriptableObjects
    public bool autoUpdate;  // Optional: automatic update flag
}

