using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemAttribute))]
public class ItemDrawer : PropertyDrawer
{

    private const string ITEM_TABLE = "Assets/Data/Items/Item Data Table.asset";

    private static ItemDataTable table = null;
    private static string[] allItems = new string[0];

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Initialize list of items
        if (table == null)
        {
            table = AssetDatabase.LoadAssetAtPath(ITEM_TABLE, typeof(ItemDataTable)) as ItemDataTable;
            List<string> allItemsList = new List<string>();
            if (table)
            {
                foreach (ItemData data in table.Data)
                {
                    if (!allItemsList.Contains(data.ItemID))
                    {
                        allItemsList.Add(data.ItemID);
                    }
                }
            }

            allItems = allItemsList.ToArray();
        }

        if (property.propertyType == SerializedPropertyType.String)
        {
            string currentItem = property.stringValue;
            int currentIndex = 0;
            for (int i = 0; i < allItems.Length; i++)
            {
                if (allItems[i] == currentItem)
                {
                    currentIndex = i;
                    break;
                }
            }
            if (allItems.Length > 0)
            {
                property.stringValue = allItems[currentIndex];
            }

            position.width = 300;
            int newIndex = EditorGUI.Popup(position, currentIndex, allItems);

            if (newIndex != currentIndex)
            {
                currentIndex = newIndex;
                string newItem = allItems[currentIndex];
                property.stringValue = newItem;
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use [Item] with strings.");
        }
    }
}