
using UnityEditor;

[CustomPropertyDrawer(typeof(ObjectDictionary))]
public class ObjectDictionaryDrawer : SerializableDictionaryDrawer
{
    protected override void SetKeyDefaultValue(SerializedProperty property)
    {
        property.stringValue = string.Empty;
    }

    protected override void SetValueDefaultValue(SerializedProperty property)
    {
        property.objectReferenceValue = null;
    }
}