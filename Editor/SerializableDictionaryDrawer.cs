using UnityEngine;
using UnityEditor;

public abstract class SerializableDictionaryDrawer : PropertyDrawer
{
    #region Constants

    private const string KEYS_PROPERTY_PATH = "_keys";
    private const string VALUES_PROPERTY_PATH = "_values";

    private const string EMPTY_LABEL = "Dictionary is empty";
    private const string ADD_ELEMENT_LABEL = "Add element";

    #endregion



    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
        {
            label = propertyScope.content;

            SerializedProperty keysProperty = property.FindPropertyRelative(KEYS_PROPERTY_PATH);
            SerializedProperty valuesProperty = property.FindPropertyRelative(VALUES_PROPERTY_PATH);

            position = EditorGUI.PrefixLabel(position, label);

            if (keysProperty.arraySize == 0)
            {
                Rect lineRect = position;

                lineRect.width = 150;
                EditorGUI.LabelField(position, new GUIContent(EMPTY_LABEL));

                lineRect.x += lineRect.width + 3;
                lineRect.width = position.width - 150 - 3;

                if (GUI.Button(lineRect, new GUIContent(ADD_ELEMENT_LABEL)))
                {
                    CreateNewElement(keysProperty, valuesProperty);
                }
            }
            else
            {
                Rect lineRect = position;

                int toDelete = -1;

                for (int i = 0; i < keysProperty.arraySize; i++)
                {
                    var key = keysProperty.GetArrayElementAtIndex(i);
                    var value = valuesProperty.GetArrayElementAtIndex(i);

                    // Draw key
                    lineRect.x = position.x;
                    lineRect.width = 100;
                    float keyHeight = EditorGUI.GetPropertyHeight(key);
                    lineRect.height = keyHeight;
                    EditorGUI.PropertyField(lineRect, key, GUIContent.none, true);

                    // Draw value
                    lineRect.x += lineRect.width + 3;
                    lineRect.width = position.width - 100 - 3 - 18 - 3;
                    float valueHeight = EditorGUI.GetPropertyHeight(value);
                    lineRect.height = valueHeight;
                    EditorGUI.PropertyField(lineRect, value, GUIContent.none, true);

                    // Draw delete button
                    lineRect.x += lineRect.width + 3;
                    lineRect.width = 18;
                    lineRect.height = EditorGUIUtility.singleLineHeight;
                    if (GUI.Button(lineRect, new GUIContent("-"), EditorStyles.miniButton))
                    {
                        toDelete = i;
                    }

                    // Line height
                    lineRect.y += Mathf.Max(keyHeight, valueHeight) + EditorGUIUtility.standardVerticalSpacing;
                }

                // Delete element
                if (toDelete > -1)
                {
                    keysProperty.DeleteArrayElementAtIndex(toDelete);
                    valuesProperty.DeleteArrayElementAtIndex(toDelete);
                }

                // Draw add button
                lineRect.x = position.x;
                lineRect.width = position.width;

                if (GUI.Button(lineRect, new GUIContent("Add element")))
                {
                    CreateNewElement(keysProperty, valuesProperty);
                }
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty keysProperty = property.FindPropertyRelative(KEYS_PROPERTY_PATH);
        SerializedProperty valuesProperty = property.FindPropertyRelative(VALUES_PROPERTY_PATH);

        if (keysProperty.arraySize == 0)
        {
            return EditorGUIUtility.singleLineHeight;
        }
        else
        {
            float totalHeight = EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                var key = keysProperty.GetArrayElementAtIndex(i);
                var value = valuesProperty.GetArrayElementAtIndex(i);
                totalHeight += Mathf.Max(EditorGUI.GetPropertyHeight(key), EditorGUI.GetPropertyHeight(value));
                totalHeight += EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }
    }

    #region Abstract

    protected abstract void SetKeyDefaultValue(SerializedProperty property);
    protected abstract void SetValueDefaultValue(SerializedProperty property);

    #endregion


    #region Private methods

    private void CreateNewElement(SerializedProperty keysProperty, SerializedProperty valuesProperty)
    {
        keysProperty.InsertArrayElementAtIndex(keysProperty.arraySize);
        SerializedProperty newElement = keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);
        SetKeyDefaultValue(newElement);

        valuesProperty.InsertArrayElementAtIndex(valuesProperty.arraySize);
        newElement = valuesProperty.GetArrayElementAtIndex(valuesProperty.arraySize - 1);
        SetValueDefaultValue(newElement);

    }

    #endregion

}