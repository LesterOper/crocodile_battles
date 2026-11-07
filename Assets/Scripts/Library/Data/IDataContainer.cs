using System;
using System.Collections.Generic;

namespace Library.Data
{
    public interface IDataContainer
    {
        void DeleteDataFromTable<T>(T dataObject) where T : new();
        void Save<T>(T dataObject) where T : new();

        void Delete<T>(T dataObject) where T : new();
        void DeleteAll<T>(T dataObject, bool greater) where T : new();
        void DeleteAll<T>(T dataObject, bool greater, List<Tuple<string, object>> additionalParams) where T : new();

        T[] GetData<T>() where T : new();

        void Clear();

        void OnDestroy();
        void OnPause();
        
    }
}