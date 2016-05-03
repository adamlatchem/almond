#region License
// Copyright 2016 Adam Latchem
// 
//    Licensed under the Apache License, Version 2.0 (the "License"); 
//    you may not use this file except in compliance with the License. 
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0 
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//    See the License for the specific language governing permissions and 
//    limitations under the License. 
//
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Almond.SQLDriver
{
    /// <summary>
    /// An implementation of IDataParameterCollection
    /// </summary>
    public class DataParameterCollection : IDataParameterCollection
    {
        #region Members
        /// <summary>
        /// The list used to implement the collection
        /// </summary>
        private IList<DbDataParameter> _parameters = new List<DbDataParameter>();
        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public DataParameterCollection()
        {
        }

        #region IDataParameterCollection
        public object this[int index]
        {
            get
            {
                return _parameters[index];
            }

            set
            {
                if (value == null)
                    return;
                _parameters[index] = (DbDataParameter)value;
            }
        }

        public object this[string parameterName]
        {
            get
            {
                var result = from p in _parameters
                             where p.ParameterName.Equals(parameterName)
                             select p;
                return result.FirstOrDefault(null);
            }

            set
            {
                if (value == null)
                    return;
                DbDataParameter old = (DbDataParameter)this[parameterName];
                if (old != null)
                    Remove(old);
                _parameters.Add((DbDataParameter)value);
            }
        }

        public int Count
        {
            get
            {
                return _parameters.Count;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return _parameters;
            }
        }

        public int Add(object value)
        {
            if (!(value is DbDataParameter))
                return -1;
            if (value == null)
                return -1;
            int result = _parameters.Count;
            _parameters.Add((DbDataParameter)value);
            return result;
        }

        public void Clear()
        {
            _parameters.Clear();
        }

        public bool Contains(object value)
        {
            return _parameters.Contains(value);
        }

        public bool Contains(string parameterName)
        {
            var result = from p in _parameters
                         where p.ParameterName.Equals(parameterName)
                         select p;
            return result.Count() > 0;
        }

        public void CopyTo(Array array, int index)
        {
            int length = Math.Min(_parameters.Count, array.Length);
            Array.Copy(_parameters.ToArray(), 0, array, index, length);
        }

        public IEnumerator GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            if (!(value is DbDataParameter))
                return -1;
            return _parameters.IndexOf((DbDataParameter)value);
        }

        public int IndexOf(string parameterName)
        {
            int result = 0;
            foreach (DbDataParameter parameter in _parameters)
            {
                if (parameter.ParameterName.Equals(parameterName))
                    return result;
                result++;
            }
            return - 1;
        }

        public void Insert(int index, object value)
        {
            if (!(value is DbDataParameter))
                return;
            if (value == null)
                return;
            _parameters.Insert(index, (DbDataParameter)value);
        }

        public void Remove(object value)
        {
            if (!(value is DbDataParameter))
                return;
            _parameters.Remove((DbDataParameter)value);
        }

        public void RemoveAt(int index)
        {
            if (index >= _parameters.Count)
                return;
            _parameters.RemoveAt(index);
        }

        public void RemoveAt(string parameterName)
        {
            DbDataParameter item = (DbDataParameter)this[parameterName];
            if (item == null)
                return;
            _parameters.Remove(item);
        }
        #endregion
    }
}
