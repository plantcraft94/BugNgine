using UnityEngine;
using System;
using Object = UnityEngine.Object;
[CreateAssetMenu(fileName = "New Door", menuName = "Door/NewDoor", order = 1)]
public class DoorData : ScriptableObject
{
    public string DoorID;
    public Object scene1;
    public Object scene2;

    void OnEnable()
    {
        // Only generate a new ID if it's empty (when first created)
        if (string.IsNullOrEmpty(DoorID))
        {
            // Generate a random string using GUID
            // Use more characters (16 instead of 8) to further reduce collision chance
            DoorID = "door_" + Guid.NewGuid().ToString("N").Substring(0, 16);

#if UNITY_EDITOR
            // If in editor, verify this ID doesn't already exist
            VerifyUniqueID();

            // Make sure changes are saved when in the editor
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }

#if UNITY_EDITOR
    // Checks if the ID already exists in any other door data asset and generates a new one if needed
    private void VerifyUniqueID()
    {
        // Find all door data assets
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:DoorData");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            DoorData otherDoor = UnityEditor.AssetDatabase.LoadAssetAtPath<DoorData>(path);

            // Skip checking against self
            if (otherDoor == this)
                continue;

            // If we found a door with the same ID, generate a new one and restart the check
            if (otherDoor != null && otherDoor.DoorID == DoorID)
            {
                DoorID = "door_" + Guid.NewGuid().ToString("N").Substring(0, 16);
                VerifyUniqueID(); // Recursive call to verify again with the new ID
                return;
            }
        }
    }
#endif
}
