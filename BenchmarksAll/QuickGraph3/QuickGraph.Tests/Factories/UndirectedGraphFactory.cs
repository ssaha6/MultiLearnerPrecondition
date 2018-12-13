// <copyright file="UndirectedGraphFactory.cs" company="MSIT">Copyright © MSIT 2007</copyright>

using System;
using Microsoft.Pex.Framework;
using QuickGraph;
using System.Collections.Generic;

namespace QuickGraph
{
    public static partial class UndirectedGraphFactory
    {
       /* [PexFactoryMethod(typeof(UndirectedGraph<int, SEdge<int>>))]
        public static UndirectedGraph<int, SEdge<int>> Create(bool allowParallelEdges)
        {
            UndirectedGraph<int, SEdge<int>> undirectedGraph
               = new UndirectedGraph<int, SEdge<int>>(allowParallelEdges);

            return undirectedGraph;
            // TODO: Edit factory method of UndirectedGraph`2<Int32,SEdge`1<Int32>>
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }*/
        [PexFactoryMethod(typeof(QuickGraph.UndirectedGraph<int, Edge<int>>))]
        public static UndirectedGraph<int, Edge<int>> CreateGraphGeneraKeyVal([PexAssumeNotNull]KeyValuePair<int, int[]>[] pairs)
        {
            PexAssume.IsTrue(pairs.Length < 11);
            //PexAssume.TrueForAll(pairs, p => p.Value != null && allLessThan(p.Value, 11));
            //PexAssume.TrueForAll(pairs, p => p.Key > -11 && p.Key < 11);
            UndirectedGraph<int, Edge<int>> g = new UndirectedGraph<int, Edge<int>>(false);

            for (int i = 0; i < pairs.Length; i++)
            {
                if (!g.ContainsVertex(pairs[i].Key))
                {
                    g.AddVertex(pairs[i].Key);
                }

                for (int j = 0; j < pairs[i].Value.Length; j++)
                {
                    if (!g.ContainsVertex(pairs[i].Value[j]))
                    {
                        g.AddVertex(pairs[i].Value[j]);
                    }
                    g.AddEdge(new Edge<int>(pairs[i].Key, pairs[i].Value[j]));
                }
            }
            return g;

        }
    }
}
