// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Validation
{
    internal class DynamicStack
    {
        private const ushort CUTOFF = 20;

        private List<object> _list;
        private HashSet<object> _hashSet;
        private bool _useHashSet = false;

        public DynamicStack()
        {
            _list = new List<object>();
        }

        public int Count
        {
            get
            {
                if (_useHashSet)
                {
                    return _hashSet.Count;
                }
                else
                {
                    return _list.Count;
                }
            }
        }

        public void Push(object model)
        {
            if (_useHashSet)
            {
                _hashSet.Add(model);
            }
            else
            {
                _list.Add(model);
            }

            if (!_useHashSet && _list.Count > CUTOFF)
            {
                _hashSet = new HashSet<object>(_list);
                _useHashSet = true;
            }
        }

        public bool Contains(object model)
        {
            if (_useHashSet)
            {
                return _hashSet.Contains(model);
            }
            else
            {
                for (var i = 0; i < _list.Count; i++)
                {
                    if (ReferenceEquals(model, _list[i]))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public void Pop(object model)
        {
            if (_useHashSet)
            {
                _hashSet.Remove(model);
            }
            else
            {
                _list.RemoveAt(_list.Count - 1);
            }
        }
    }
}
