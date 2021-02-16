using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TotBase
{
    [Serializable]
    public abstract class DataTable<T> : ScriptableObject, IDataTable<T>, IAnonymousDataTable, ISerializationCallbackReceiver where T : struct
    {
        [SerializeField]
        private List<string> keys = new List<string>();

        [SerializeField]
        private List<T> values = new List<T>();

        private Dictionary<string, T> data = new Dictionary<string, T>();

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        public void DeleteEntry(string key)
        {
            data.Remove(key);
        }

        public void ForEachEntries(Action<string, T> action)
        {
            foreach (KeyValuePair<string, T> row in data)
                action(row.Key, row.Value);
        }

        public T GetEntry(string key)
        {
            return data[key];
        }

        public Type GetStructType()
        {
            return typeof(T);
        }

        public abstract T CreateStruct();


        public void OnAfterDeserialize()
        {
            data.Clear();

            for (int i = 0; i < keys.Count; i++)
                data.Add(keys[i], values[i]);
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<string, T> pair in data)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void SetEntry(string key, T entry)
        {
            data[key] = entry;
        }

        public bool TryGetEntry(string key, out T entry)
        {
            return data.TryGetValue(key, out entry);
        }

        public IEnumerable<string> GetKeys()
        {
            return data.Keys;
        }

        public int GetCount()
        {
            return data.Keys.Count;
        }
    }
}
