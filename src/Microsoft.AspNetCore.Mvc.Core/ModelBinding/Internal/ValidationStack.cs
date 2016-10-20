// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Internal;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Internal
{
    internal class ValidationStack
    {
        // We tested the performance of a list at size 15 and found it still better than hashset, but to avoid a costly
        // O(n) search at larger n we set the cutoff to 20. If someone finds the point where they intersect feel free to change this number.
        protected virtual int CutOff
        {
            get { return 20; }
        }

        protected readonly List<object> _list = new List<object>();
        protected HashSet<object> _hashSet;

        public int Count => _hashSet?.Count ?? _list.Count;

        public bool Push(object model)
        {
            if (_hashSet != null)
            {
                return _hashSet.Add(model);
            }
            else
            {
                if (ListContains(model))
                {
                    return false;
                }
                else
                {
                    _list.Add(model);

                    if (_hashSet == null && _list.Count > CutOff)
                    {
                        _hashSet = new HashSet<object>(_list, ReferenceEqualityComparer.Instance);
                    }

                    return true;
                }
            }
        }

        private bool ListContains(object model)
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

        public void Pop(object model)
        {
            if (_hashSet != null)
            {
                _hashSet.Remove(model);
            }
            else
            {
                if (model != null)
                {
                    Debug.Assert(ReferenceEquals(_list[_list.Count - 1], model));
                    _list.RemoveAt(_list.Count - 1);
                }
            }
        }
    }
}
