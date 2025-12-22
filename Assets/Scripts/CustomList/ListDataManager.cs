using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Watermelon.List
{
    /// <summary>
    /// Quản lý data operations cho list
    /// </summary>
    public class ListDataManager
    {
        // Data sources
        private SerializedObject serializedObject;
        private SerializedProperty elementsProperty;
        private List<SerializedProperty> elementsList;
        private IList elements;

        // Data mode flags
        private bool usingPropertyList = false;
        private bool usingListInterface = false;

        // Label handling
        private string labelPropertyName;
        private bool useLabelProperty;
        private SimpleCustomList.GetLabelDelegate getLabelCallback;

        // Properties
        public int Count
        {
            get
            {
                if (usingListInterface)
                    return elements.Count;
                else if (usingPropertyList)
                    return elementsList.Count;
                else
                    return elementsProperty.arraySize;
            }
        }

        public bool UsingListInterface => usingListInterface;

        // Constructor for SerializedProperty array with property name
        public void Initialize(SerializedObject serializedObject, SerializedProperty elementsProperty, string labelPropertyName)
        {
            this.serializedObject = serializedObject;
            this.elementsProperty = elementsProperty;
            this.labelPropertyName = labelPropertyName;
            this.useLabelProperty = true;
            this.usingPropertyList = false;
            this.usingListInterface = false;
        }

        // Constructor for SerializedProperty array with callback
        public void Initialize(SerializedObject serializedObject, SerializedProperty elementsProperty, SimpleCustomList.GetLabelDelegate getLabelCallback)
        {
            this.serializedObject = serializedObject;
            this.elementsProperty = elementsProperty;
            this.getLabelCallback = getLabelCallback;
            this.useLabelProperty = false;
            this.usingPropertyList = false;
            this.usingListInterface = false;
        }

        // Constructor for List<SerializedProperty> with property name
        public void Initialize(SerializedObject serializedObject, List<SerializedProperty> elementsList, string labelPropertyName)
        {
            this.serializedObject = serializedObject;
            this.elementsList = elementsList;
            this.labelPropertyName = labelPropertyName;
            this.useLabelProperty = true;
            this.usingPropertyList = true;
            this.usingListInterface = false;
        }

        // Constructor for List<SerializedProperty> with callback
        public void Initialize(SerializedObject serializedObject, List<SerializedProperty> elementsList, SimpleCustomList.GetLabelDelegate getLabelCallback)
        {
            this.serializedObject = serializedObject;
            this.elementsList = elementsList;
            this.getLabelCallback = getLabelCallback;
            this.useLabelProperty = false;
            this.usingPropertyList = true;
            this.usingListInterface = false;
        }

        // Constructor for IList with callback
        public void Initialize(IList elements, SimpleCustomList.GetLabelDelegate getLabelCallback)
        {
            this.elements = elements;
            this.getLabelCallback = getLabelCallback;
            this.useLabelProperty = false;
            this.usingPropertyList = false;
            this. usingListInterface = true;
        }

        public SerializedProperty GetElement(int index)
        {
            if (usingListInterface) return null;

            if (index >= Count || index < 0)
            {
                Debug.LogError("Index out of bounds:  " + index);
                return null;
            }

            if (usingPropertyList)
                return elementsList[index];
            else
                return elementsProperty.GetArrayElementAtIndex(index);
        }

        public string GetElementLabel(int index)
        {
            if (usingListInterface)
            {
                return getLabelCallback?. Invoke(null, index) ?? $"Element {index}";
            }

            SerializedProperty elementProperty = GetElement(index);
            
            if (useLabelProperty)
            {
                SerializedProperty labelProp = elementProperty.FindPropertyRelative(labelPropertyName);
                return labelProp != null ? labelProp.stringValue : $"Element {index}";
            }
            else
            {
                return getLabelCallback?.Invoke(elementProperty, index) ?? $"Element {index}";
            }
        }

        public void MoveElement(int srcIndex, int destIndex)
        {
            if (srcIndex == destIndex) return;

            if (usingListInterface)
            {
                var item = elements[srcIndex];
                elements.RemoveAt(srcIndex);
                elements.Insert(destIndex, item);
            }
            else if (usingPropertyList)
            {
                SerializedProperty temp = elementsList[srcIndex];
                elementsList.RemoveAt(srcIndex);
                elementsList. Insert(destIndex, temp);
            }
            else
            {
                elementsProperty.MoveArrayElement(srcIndex, destIndex);
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void DuplicateElement(int index)
        {
            if (usingListInterface)
            {
                var item = elements[index];
                elements. Insert(index + 1, item);
            }
            else if (usingPropertyList)
            {
                Debug.LogWarning("Duplicate not supported for List<SerializedProperty>");
                return;
            }
            else
            {
                elementsProperty.InsertArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
            }
        }

        public void ApplyModifiedProperties()
        {
            if (! usingListInterface && serializedObject != null)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}