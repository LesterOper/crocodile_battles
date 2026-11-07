using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using SimpleSQL;
using UnityEngine;

namespace Library.Data
{
    public class SqliteDataContainer : IDataContainer
    {
        private readonly ReadOnlyCollection<Type> _dataTypes;

        private readonly object _lockObject = new object();
        private readonly object _pauseLockObject = new object();

        private readonly Thread _thread;

        private volatile Queue<Command> _commandsQueue = new Queue<Command>();
        private volatile bool _destroying;

        public SqliteDataContainer(SimpleSQLManager sqlManager, params Type[] dataTypes)
        {
            SqlManager = sqlManager;
            _dataTypes = dataTypes.ToList().AsReadOnly();

            var method = SqlManager.GetType().GetMethod("CreateTable");

            foreach (var type in dataTypes)
            {
                var generic = method.MakeGenericMethod(type);
                generic.Invoke(SqlManager, null);
            }

            BeforeThreadStart();

            _thread = new Thread(ExecuteCommands);
            _thread.IsBackground = true;
            _thread.Start();
        }

        protected SimpleSQLManager SqlManager { get; }

        public void Save<T>(T dataObject) where T : new()
        {
            var properties = dataObject.GetType().GetProperties()
                .Where(p => p.GetCustomAttributes(typeof(IgnoreAttribute), true).Length == 0);
            var command = new Command(
                "INSERT OR REPLACE INTO '" + dataObject.GetType().Name + "' " +
                "(" + string.Join(", ", properties.Select(p => p.Name)) + ") " +
                "VALUES('" + string.Join("', '",
                    properties.Select(p =>
                        p.PropertyType.IsEnum
                            ? (int) p.GetValue(dataObject)
                            : p.PropertyType == typeof(bool)
                                ? (bool) p.GetValue(dataObject) ? 1 : 0
                                : p.PropertyType == typeof(float)
                                    ? ((float) p.GetValue(dataObject)).ToString(CultureInfo.InvariantCulture)
                                    : p.PropertyType == typeof(double)
                                        ? ((double) p.GetValue(dataObject)).ToString(CultureInfo.InvariantCulture)
                                        : p.GetValue(dataObject))) + "');");

            lock (_lockObject)
            {
                _commandsQueue.Enqueue(command);
            }
        }

        public void DeleteDataFromTable<T>(T dataObject) where T : new()
        {
            var command = new Command("DELETE FROM " + dataObject.GetType().Name);
            
            lock (_lockObject)
            {
                _commandsQueue.Enqueue(command);
            }
        }
        
        public void Delete<T>(T dataObject) where T : new()
        {
            var property = dataObject.GetType().GetProperties()
                .First(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0);
            var command = new Command(
                "DELETE FROM '" + dataObject.GetType().Name + "' " +
                "WHERE " + property.Name + "='" + property.GetValue(dataObject) + "';");

            lock (_lockObject)
            {
                _commandsQueue.Enqueue(command);
            }
        }

        public void DeleteAll<T>(T dataObject, bool greater) where T : new()
        {
            var property = dataObject.GetType().GetProperties()
                .First(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0);

            var command = new Command(
                "DELETE FROM '" + dataObject.GetType().Name + "' " +
                "WHERE " + property.Name + (greater ? ">" : "<") + "'" + property.GetValue(dataObject) + "';");

            lock (_lockObject)
            {
                _commandsQueue.Enqueue(command);
            }
        }

        public void DeleteAll<T>(T dataObject, bool greater, List<Tuple<string, object>> additionalParams)
            where T : new()
        {
            var property = dataObject.GetType().GetProperties()
                .First(p => p.GetCustomAttributes(typeof(PrimaryKeyAttribute), true).Length > 0);

            var commandText =
                "DELETE FROM '" + dataObject.GetType().Name + "' " +
                "WHERE " + property.Name + (greater ? ">" : "<") + "'" + property.GetValue(dataObject) + "'";

            if (additionalParams != null && additionalParams.Count > 0)
            {
            }

            commandText += string.Join("",
                additionalParams
                    .Select(tuple => " AND " + tuple.Item1 + "=" +
                                     "'" + (tuple.Item2.GetType().IsEnum ? (int) tuple.Item2 : tuple.Item2) + "'"));

            commandText += ";";

            lock (_lockObject)
            {
                _commandsQueue.Enqueue(new Command(commandText));
            }
        }

        public T[] GetData<T>() where T : new()
        {
            var tableData = new List<T>(SqlManager.Table<T>());

            return tableData.ToArray();
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _commandsQueue.Clear();
                foreach (var type in _dataTypes)
                {
                    var command = new Command("DELETE FROM '" + type.Name + "';");
                    _commandsQueue.Enqueue(command);
                }
            }
        }

        public void OnDestroy()
        {
            _destroying = true;
            OnPause();
        }

        public void OnPause()
        {
            if (_commandsQueue.Count > 0)
                lock (_lockObject)
                {
                    lock (_pauseLockObject)
                    {
                        while (_commandsQueue.Count > 0)
                        {
                            var command = _commandsQueue.Dequeue();
                            try
                            {
                                SqlManager.Execute(command.Sql, command.Params);
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }
                }
        }

        public virtual void BeforeThreadStart()
        {
        }

        private void ExecuteCommands()
        {
            while (!_destroying || _commandsQueue.Count > 0)
            {
                if (_commandsQueue.Count > 0)
                {
                    var queueCopy = new Queue<Command>();

                    lock (_pauseLockObject)
                    {
                        lock (_lockObject)
                        {
                            while (_commandsQueue.Count > 0) queueCopy.Enqueue(_commandsQueue.Dequeue());
                        }

                        while (queueCopy.Count > 0)
                        {
                            var command = queueCopy.Dequeue();

                            try
                            {

                                SqlManager.Execute(command.Sql, command.Params);
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                        }
                    }
                }

                if (!_destroying) Thread.Sleep(100);
            }
        }

        public void AddIntColumnIfNeed(Type type, string colName)
        {
            AddColumnIfNeed(type, colName, "INTEGER", "0");
        }


        public void AddStringColumnIfNeed(Type type, string colName)
        {
            AddColumnIfNeed(type, colName, "TEXT", "");
        }

        public void AddColumnIfNeed(Type type, string colName, string colType, string defaultVal)
        {
            var rowData = SqlManager.QueryGeneric("PRAGMA table_info(" + type.Name + ")");
            var hasResultRateRow = rowData.rows.Any(r => r.fields.Any(f => f != null && f.ToString().Equals(colName)));

            if (!hasResultRateRow)
            {
                var sql = "ALTER  TABLE " + type.Name + " ADD COLUMN \"" + colName + "\" " + colType + " DEFAULT " +
                          defaultVal;
                SqlManager.Execute(sql);
            }
        }

        private class Command
        {
            public readonly object[] Params;
            public readonly string Sql;

            public Command(string sql, params object[] @params)
            {
                Sql = sql;
                Params = @params;
            }
        }
    }
}