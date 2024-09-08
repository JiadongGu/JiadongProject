using UnityEditor;
using UnityEngine;
using System.IO;

public class RetailItemNamerTool : Editor
{
    [MenuItem("Tools/Update RetailItem Names")]
    public static void UpdateRetailItemNames()
    {
        // Get the currently selected folder in the project window
        string selectedFolder = GetSelectedFolder();

        if (string.IsNullOrEmpty(selectedFolder))
        {
            Debug.LogWarning("Please select a folder in the Project window.");
            return;
        }

        // Find all RetailItem ScriptableObjects in the selected folder
        string[] assetPaths = AssetDatabase.FindAssets("t:RetailItem", new[] { selectedFolder });

        foreach (string assetPath in assetPaths)
        {
            string fullPath = AssetDatabase.GUIDToAssetPath(assetPath);
            Debug.Log(fullPath);
            RetailItem retailItem = AssetDatabase.LoadAssetAtPath<RetailItem>(fullPath);

            if (retailItem != null)
            {
                string assetName = Path.GetFileNameWithoutExtension(fullPath);
                
                // Set the itemName to the ScriptableObject's name
                retailItem.itemName = assetName;

                // Mark the object as dirty so the change can be saved
                EditorUtility.SetDirty(retailItem);
            }
        }

        // Save changes to the assets
        AssetDatabase.SaveAssets();
        Debug.Log("Updated all RetailItem item names.");
    }

    // Helper function to get the currently selected folder
    private static string GetSelectedFolder()
    {
        Object obj = Selection.activeObject;

        if (obj == null)
            return null;

        string path = AssetDatabase.GetAssetPath(obj);

        if (Directory.Exists(path))
        {
            return path;
        }
        else
        {
            return Path.GetDirectoryName(path);
        }
    }
}
