using UnityEngine;
using System;

namespace TotBase.DataTable
{
    [Serializable]
    public class DataTablePointer
    {
        public ScriptableObject _datatable;
        public string _key;

        public bool TryGetEntry<T>(out T entry) where T : ScriptableObject
        {
            entry = default;
            if (_datatable == null) return false;
            IDataTable<T> dt = (IDataTable<T>)_datatable;
            if (dt == null) return false;
            return dt.TryGetEntry(_key, out entry);
        }

        public bool HasEntry<T>() where T : ScriptableObject
        {
            return ((IDataTable<T>)_datatable).ContainsKey(_key);
        }

        public override string ToString()
        {
            return $"{_datatable} [{_key}]";
        }
    }
}