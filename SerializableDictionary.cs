using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private TKey[] _keys;

    [SerializeField] private TValue[] _values;

    public void OnBeforeSerialize()
    {
        _keys = Keys.ToArray();
        _values = Values.ToArray();
    }

    public void OnAfterDeserialize()
    {
        Clear();

        for (int i = 0; i < _keys.Length; i++)
        {
            Add(_keys[i], _values[i]);
        }
    }
}