using UnityEditor;

[CustomPropertyDrawer(typeof(StringDictionary))]
public class StringDictionaryDrawer : SerializableDictionaryDrawer
{
    protected override void SetKeyDefaultValue(SerializedProperty property)
    {
        property.stringValue = string.Empty;
    }

    protected override void SetValueDefaultValue(SerializedProperty property)
    {
        property.stringValue = string.Empty;
    }
}
