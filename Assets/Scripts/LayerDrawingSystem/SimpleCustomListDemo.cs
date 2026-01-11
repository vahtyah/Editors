using System;
using UnityEngine;
using UnityEditor;
using VahTyah;
using EditorStyles = UnityEditor.EditorStyles;
using Random = UnityEngine.Random;
using SimpleCustomList = VahTyah.List.SimpleCustomList;

public class SimpleCustomListDemo :  EditorWindow
{
    [Serializable]
    public class TestItem
    {
        public string name = "New Item";
        public ItemType type = ItemType.Normal;
        public int value = 0;
    }

    public enum ItemType
    {
        Normal,
        Weapon,
        Armor,
        Consumable,
        QuestItem
    }

    [SerializeField]
    private TestItem[] items = new TestItem[0];

    // ✅ THÊM:  Styles Database
    [SerializeField]
    private ListStylesDatabase stylesDatabase;
    private int selectedStyleIndex = 0;
    private string[] styleNames;

    private SerializedObject serializedObject;
    private SerializedProperty itemsProperty;
    private SimpleCustomList customList;

    [MenuItem("Window/Simple Custom List Demo")]
    public static void ShowWindow()
    {
        GetWindow<SimpleCustomListDemo>("List Demo");
    }

    private void OnEnable()
    {
        // Initialize sample data
        if (items == null || items.Length == 0)
        {
            items = new TestItem[5];
            for (int i = 0; i < items.Length; i++)
            {
                items[i] = new TestItem 
                { 
                    name = $"Item {i}",
                    type = (ItemType)(i % 5),
                    value = Random.Range(1, 100)
                };
            }
        }

        // Setup SerializedObject
        serializedObject = new SerializedObject(this);
        itemsProperty = serializedObject.FindProperty("items");

        // Create custom list
        customList = new SimpleCustomList(serializedObject, itemsProperty, GetItemLabel);
        
        // ✅ THÊM:  Set parent window for repaint
        customList.ParentWindow = this;

        // Setup all callbacks
        SetupCallbacks();
    }

    private void SetupCallbacks()
    {
        // Header label
        customList.getHeaderLabelCallback = () => $"Levels";

        // Add dropdown callback
        customList.addElementWithDropdownCallback = ShowAddItemMenu;

        // Search filter callback
        customList.searchFilterCallback = (prop, index, query) =>
        {
            if (index < 0 || index >= items.Length)
                return false;

            TestItem item = items[index];
            
            // Search in name
            if (item.name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            
            // Search in type
            if (item.type. ToString().IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
            
            // Search in value (convert to string)
            if (item.value.ToString().Contains(query))
                return true;
            
            return false;
        };

        // Add callback (fallback)
        customList.addElementCallback = () =>
        {
            Array.Resize(ref items, items.Length + 1);
            items[items.Length - 1] = new TestItem { name = $"Item {items.Length - 1}" };
            serializedObject.Update();
        };

        // Remove callback
        customList.removeElementCallback = () =>
        {
            int index = customList.SelectedIndex;
            if (index >= 0 && index < items.Length)
            {
                var list = new System.Collections.Generic. List<TestItem>(items);
                list.RemoveAt(index);
                items = list.ToArray();
                serializedObject.Update();
            }
        };
        
        customList.displayContextMenuCallback = (index) =>
        {
            GenericMenu menu = new GenericMenu();
        
            TestItem item = items[index];
        
            // Custom actions
            menu. AddItem(new GUIContent($"Log '{item.name}' Info"), false, () =>
            {
                Debug.Log($"Item Info:\nName: {item.name}\nType: {item.type}\nValue: {item.value}");
            });
        
            menu.AddItem(new GUIContent("Set Value to 100"), false, () =>
            {
                items[index].value = 100;
                serializedObject.ApplyModifiedProperties();
                Repaint();
            });
        
            menu.AddSeparator("");
        
            // Use default menu items
            menu.AddItem(new GUIContent("Duplicate"), false, DuplicateSelected);
        
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                customList. SelectedIndex = index;
                customList.removeElementCallback?.Invoke();
            });
        
            menu.ShowAsContext();
        };

        // List changed callback
        customList.listChangedCallback = () =>
        {
            serializedObject.ApplyModifiedProperties();
            Repaint();
        };

        // Selection changed callback
        customList.selectionChangedCallback = () =>
        {
            Repaint();
        };

        // Reordered callbacks
        customList.listReorderedCallback = () =>
        {
            Debug.Log("List reordered!");
        };

        customList.listReorderedCallbackWithDetails = (from, to) =>
        {
            Debug.Log($"Item moved from index {from} to {to}:  {items[to].name}");
        };
        
        // Undo callback
        customList.listUndoCallback = (undoMessage) =>
        {
            Undo.RecordObject(this, undoMessage);
            Debug.Log($"[UNDO] {undoMessage}");
        };

        // Double-click callback
        customList.elementDoubleClickedCallback = (index) =>
        {
            if (index >= 0 && index < items.Length)
            {
                TestItem item = items[index];
                
                bool openInspector = EditorUtility.DisplayDialog(
                    "Double-Clicked",
                    $"You double-clicked:\n\n{item.name}\nType: {item.type}\nValue: {item.value}\n\nOpen in Inspector?",
                    "Yes",
                    "Cancel"
                );
                
                if (openInspector)
                {
                    Debug.Log($"[DOUBLE-CLICK] Opening inspector for:  {item.name}");
                    Selection.activeObject = this;
                    EditorGUIUtility.PingObject(this);
                }
            }
        };
    }

    // Dropdown menu (unchanged)
    private void ShowAddItemMenu(Rect buttonRect)
    {
        GenericMenu menu = new GenericMenu();

        // Add different item types
        menu.AddItem(new GUIContent("Items/Normal Item"), false, () => AddItem(ItemType.Normal));
        menu.AddItem(new GUIContent("Items/Weapon"), false, () => AddItem(ItemType.Weapon));
        menu.AddItem(new GUIContent("Items/Armor"), false, () => AddItem(ItemType. Armor));
        menu.AddItem(new GUIContent("Items/Consumable"), false, () => AddItem(ItemType. Consumable));
        menu.AddItem(new GUIContent("Items/Quest Item"), false, () => AddItem(ItemType.QuestItem));

        menu.AddSeparator("");

        // Add from template
        menu.AddItem(new GUIContent("Add from Template... "), false, () => 
        {
            AddItemFromTemplate();
        });

        // Add random
        menu.AddItem(new GUIContent("Add Random Item"), false, () => 
        {
            AddItem((ItemType)Random.Range(0, 5));
        });

        menu.AddSeparator("");

        // Bulk operations
        menu.AddItem(new GUIContent("Add 5 Items"), false, () => 
        {
            for (int i = 0; i < 5; i++)
            {
                AddItem(ItemType.Normal);
            }
        });

        menu.AddItem(new GUIContent("Add 10 Items"), false, () => 
        {
            for (int i = 0; i < 10; i++)
            {
                AddItem(ItemType.Normal);
            }
        });

        menu.AddSeparator("");

        // Duplicate selected
        if (customList.SelectedIndex >= 0 && customList. SelectedIndex < items.Length)
        {
            menu.AddItem(new GUIContent("Duplicate Selected"), false, DuplicateSelected);
        }
        else
        {
            menu.AddDisabledItem(new GUIContent("Duplicate Selected"));
        }

        menu. DropDown(buttonRect);
    }

    private void AddItem(ItemType type)
    {
        Array.Resize(ref items, items.Length + 1);
        
        string typeName = type.ToString();
        items[items.Length - 1] = new TestItem 
        { 
            name = $"{typeName} {items.Length}",
            type = type,
            value = GetDefaultValue(type)
        };
        
        serializedObject. Update();
        customList.listChangedCallback?. Invoke();
        
        Debug.Log($"Added {typeName}:  {items[items.Length - 1].name}");
    }

    private void AddItemFromTemplate()
    {
        int choice = EditorUtility.DisplayDialogComplex(
            "Add from Template",
            "Choose a template:",
            "Starter Pack",
            "Cancel",
            "Epic Loot"
        );

        if (choice == 0) // Starter Pack
        {
            AddItem(ItemType.Normal);
            AddItem(ItemType. Weapon);
            AddItem(ItemType.Consumable);
            Debug.Log("Added Starter Pack (3 items)");
        }
        else if (choice == 2) // Epic Loot
        {
            AddItem(ItemType. Weapon);
            AddItem(ItemType.Armor);
            items[items.Length - 1]. value = 999;
            items[items.Length - 2].value = 999;
            Debug.Log("Added Epic Loot (2 items with value 999)");
        }
    }

    private void DuplicateSelected()
    {
        int index = customList.SelectedIndex;
        if (index >= 0 && index < items.Length)
        {
            Array.Resize(ref items, items.Length + 1);
            
            TestItem original = items[index];
            items[items.Length - 1] = new TestItem
            {
                name = original. name + " (Copy)",
                type = original.type,
                value = original.value
            };
            
            serializedObject.Update();
            customList.listChangedCallback?.Invoke();
            
            Debug.Log($"Duplicated:  {original.name}");
        }
    }

    private int GetDefaultValue(ItemType type)
    {
        switch (type)
        {
            case ItemType.Normal: return 10;
            case ItemType.Weapon: return 50;
            case ItemType.Armor: return 40;
            case ItemType. Consumable: return 20;
            case ItemType.QuestItem: return 0;
            default: return 0;
        }
    }

    private string GetItemLabel(SerializedProperty elementProperty, int index)
    {
        if (index < 0 || index >= items.Length)
            return $"Item {index}";

        TestItem item = items[index];
        string icon = GetItemIcon(item.type);
        return $"{icon} {item.name} [{item.type}] (Value: {item.value})";
    }

    private string GetItemIcon(ItemType type)
    {
        switch (type)
        {
            case ItemType.Normal: return "📦";
            case ItemType.Weapon: return "⚔️";
            case ItemType.Armor: return "🛡️";
            case ItemType.Consumable: return "🧪";
            case ItemType.QuestItem: return "📜";
            default: return "❓";
        }
    }

    private void OnGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(5);
        EditorGUILayout. HelpBox(
            "✅ Features:\n" +
            "• Select style from database or create new\n" +
            "• Click [▾] button to see Add Dropdown menu\n" +
            "• Drag & Drop to reorder\n" +
            "• Search/Filter items\n" +
            "• Double-click for actions\n" +
            "• Right-click for context menu\n" +
            "• Keyboard navigation (↑↓←→)\n" +
            "• Full Undo support (Ctrl+Z)",
            MessageType.Info
        );

        EditorGUILayout.Space(10);

        // Display custom list
        customList.Display();

        EditorGUILayout.Space(10);

        // Selected item inspector
        if (customList. SelectedIndex >= 0 && customList.SelectedIndex < items. Length)
        {
            EditorGUILayout.LabelField("Selected Item", EditorStyles.boldLabel);
            
            TestItem selected = items[customList.SelectedIndex];
            
            EditorGUI.BeginChangeCheck();
            
            selected.name = EditorGUILayout.TextField("Name", selected.name);
            selected.type = (ItemType)EditorGUILayout.EnumPopup("Type", selected.type);
            selected. value = EditorGUILayout. IntField("Value", selected.value);
            
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Repaint();
            }
        }

        EditorGUILayout.Space(10);

        // Debug buttons
        EditorGUILayout.LabelField("Debug Actions", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("Add 100 Items (Test Pagination)"))
        {
            for (int i = 0; i < 100; i++)
            {
                AddItem((ItemType)(i % 5));
            }
        }
        
        if (GUILayout.Button("Clear All"))
        {
            if (EditorUtility.DisplayDialog("Clear All", "Remove all items?", "Yes", "No"))
            {
                items = new TestItem[0];
                serializedObject.Update();
                customList.listChangedCallback?.Invoke();
            }
        }
        
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}