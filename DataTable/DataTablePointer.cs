using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TotBase
{
    [Serializable]
    public class DataTablePointer
    {
        public ScriptableObject _datatable;
        public string _key;

        public bool TryGetEntry<T>(out T entry) where T : struct
        {
            entry = default;
            if (_datatable == null) return false;
            IDataTable<T> dt = (IDataTable<T>)_datatable;
            if (dt == null) return false;
            return dt.TryGetEntry(_key, out entry);
        }

        public bool HasEntry<T>() where T : struct
        {
            return ((IDataTable<T>)_datatable).ContainsKey(_key);
        }

        public override string ToString()
        {
            return $"{_datatable} [{_key}]";
        }
    }

}
