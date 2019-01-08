﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace DataStructures.Interfaces
{
    // The generic IEqualityComparer interface implements methods to if check two objects are equal
    // and generate Hashcode for an object.
    // It is used in Dictionary class.  
    public interface IEqualityComparer<in T>
    {
        bool Equals(T x, T y);
        int GetHashCode(T obj);
    }
}