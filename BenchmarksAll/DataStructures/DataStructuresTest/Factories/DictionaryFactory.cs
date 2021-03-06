﻿using System;
using System.Text;
using DataStructures;
using Microsoft.Pex.Framework;

namespace DataStructures.Test.Factories
{
    public static class DictionaryFactory
    {
        /*[PexFactoryMethod(typeof(DataStructures.Dictionary<int, int>))]
        public static Dictionary<int, int> Create([PexAssumeNotNull]int[] keys,[PexAssumeNotNull] int[] values)
        {

            PexAssume.IsTrue( keys.Length < 11 && keys.Length == values.Length);
            PexAssume.TrueForAll(0, keys.Length, _i => keys[_i] > -11 && keys[_i] < 11);
            PexAssume.TrueForAll(0, values.Length, _j => values[_j] > -11 && values[_j] < 11);
            //DataStructures.Utility.Int32EqualityComparer comparer = new DataStructures.Utility.Int32EqualityComparer();

            DataStructures.Dictionary<int, int> ret = new DataStructures.Dictionary<int, int>(keys.Length, EqualityComparer<int>.Default);// DataStructure has big enough capacity for Commutativity Test
            for (int i = 0; i < keys.Length; i++)
            {
                // For stack, add any element. 
                if (!ret.ContainsKey(keys[i]))
                    ret.Add(keys[i], values[i]);
            }
            return ret;

        }*/

        [PexFactoryMethod(typeof(DataStructures.Dictionary<int, int>))]
        public static Dictionary<int, int> CreateKeyValPair([PexAssumeNotNull]System.Collections.Generic.KeyValuePair<int,int>[] pairs)
        {

            PexAssume.IsTrue(pairs.Length < 11);
            
            PexAssume.TrueForAll(pairs, p => (p.Key > -11 && p.Key < 11) && (p.Value > -11 && p.Value < 11));
            //DataStructures.Utility.Int32EqualityComparer comparer = new DataStructures.Utility.Int32EqualityComparer();

            DataStructures.Dictionary<int, int> ret = new DataStructures.Dictionary<int, int>(pairs.Length+2);// DataStructure has big enough capacity for Commutativity Test
            for (int i = 0; i < pairs.Length; i++)
            {
                 
                if (!ret.ContainsKey(pairs[i].Key))
                    ret.Add(pairs[i].Key, pairs[i].Value);
            }
            return ret;

        }

    }
}