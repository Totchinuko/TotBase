using System;
using System.Collections.Generic;

namespace TotBase
{
    public interface IDataTable<T> where T : struct
    {
        T GetEntry(string key);
        void DeleteEntry(string key);
        void SetEntry(string key, T entry);
        bool TryGetEntry(string key, out T entry);
        void ForEachEntries(Action<string, T> action);
        bool ContainsKey(string key);
        IEnumerable<string> GetKeys();
        T CreateStruct();
        int GetCount();
        Type GetStructType();
    }

    public interface IAnonymousDataTable
    {
        bool ContainsKey(string key);
        IEnumerable<string> GetKeys();
        Type GetStructType();
        int GetCount();
    }
}
