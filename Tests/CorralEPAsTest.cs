using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Contractor.Core;
using System.IO;

namespace Tests
{
    [TestClass]
    public class CorralEPAsTest
    {
        private static EpaGenerator epaGenerator;

        [ClassInitialize()]
        public static void GenerateEPAs(TestContext tc)
        {
            Configuration.Initialize();
            Configuration.TempPath = Directory.GetParent(tc.TestDir).ToString();
            Configuration.InlineMethodsBody = true;

            epaGenerator = new EpaGenerator(EpaGenerator.Backend.Corral);
            var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));
            epaGenerator.LoadAssembly(ExamplesPath);
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            epaGenerator.Dispose();
        }

        [TestMethod]
        public void TestStack()
        {
            var epa = epaGenerator.GenerateEpa("Examples.FiniteStack").EPA;
            
            #region Graph representation
            var graph = new List<List<int>>();

            graph.Add(new List<int>());
            graph[0].Add(1);
            graph[0].Add(1);

            graph.Add(new List<int>());
            graph[1].Add(1);
            graph[1].Add(1);
            graph[1].Add(2);

            graph.Add(new List<int>());
            graph[2].Add(1);
            graph[2].Add(2);
            graph[2].Add(2);
            graph[2].Add(2);
            graph[2].Add(2);
            graph[2].Add(3);

            graph.Add(new List<int>());
            graph[3].Add(2);
            graph[3].Add(3);
            graph[3].Add(3);
            #endregion

            Traverse(graph, epa);
        }

        [TestMethod]
        public void TestLinear()
        {
            var epa = epaGenerator.GenerateEpa("Examples.Linear").EPA;

            #region Graph representation
            var graph = new List<List<int>>();
            for (int i = 0; i < epa.States.Count; i++)
            {
                graph.Add(new List<int>());
                graph[i] = new List<int>();

                if (i > 0)
                {
                    // Get Property
                    graph[i].Add(i);
                }
                if (i < epa.States.Count - 1)
                {
                    // a_{i+1}
                    graph[i].Add(i + 1);
                }
            }
            #endregion

            Traverse(graph, epa);
        }

        [TestMethod]
        public void TestDoor()
        {
            var epa = epaGenerator.GenerateEpa("Examples.DoorPost").EPA;

            #region Graph representation
            var graph = new List<List<int>>();

            graph.Add(new List<int>());
            graph[0].Add(1);

            graph.Add(new List<int>());
            graph[1].Add(2);
            graph[1].Add(3);
            graph[1].Add(4);

            graph.Add(new List<int>());
            graph[2].Add(1);
            graph[2].Add(3);
            graph[2].Add(4);

            graph.Add(new List<int>());
            graph[3].Add(1);
            graph[3].Add(5);

            graph.Add(new List<int>());
            graph[4].Add(2);
            graph[4].Add(5);

            graph.Add(new List<int>());
            graph[5].Add(4);
            graph[5].Add(6);

            graph.Add(new List<int>());
            graph[6].Add(2);
            graph[6].Add(3);
            graph[6].Add(5);

            #endregion

            Traverse(graph, epa);
        }

        /// <summary>
        /// Traverse the EPA while comparing its structure with a graph
        /// </summary>
        /// <param name="nodes">Graph representation</param>
        /// <param name="epa">EPA representation</param>
        private void Traverse(List<List<int>> nodes, Epa epa)
        {
            //TODO: use an isomorphism algorithm to compare the epa and the graph
            
            // Same amount of states?
            Assert.AreEqual(nodes.Count, epa.States.Count);

            // Check whether exists a bijection in the states/nodes. However, we are not considering
            // a proper isomorphism.
            var transitions = new List<int>();
            foreach (var s in epa.States)
            {
                transitions.Add(epa[s].Count);
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                CollectionAssert.Contains(transitions, nodes[i].Count);
                transitions.Remove(nodes[i].Count);
            }
        }
    }
}
