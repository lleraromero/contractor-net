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

        /// <summary>
        /// Traverse the EPA while comparing its structure with a graph
        /// </summary>
        /// <param name="nodes">Graph representation</param>
        /// <param name="epa">EPA representation</param>
        private void Traverse(List<List<int>> nodes, Epa epa)
        {
            // Same amount of states?
            Assert.AreEqual(nodes.Count, epa.States.Count);

            if (nodes.Count == 0)
            {
                return;
            }

            var nodesToVisit = new Queue<int>();
            var statesToVisit = new Queue<IState>();
            var nodesSeen = new HashSet<int>();
            var statesSeen = new HashSet<IState>();

            nodesToVisit.Enqueue(0);
            nodesSeen.Add(0);
            statesToVisit.Enqueue(epa.Initial);
            statesSeen.Add(epa.Initial);

            while (nodesToVisit.Count > 0 && statesToVisit.Count > 0)
            {
                var currentNode = nodesToVisit.Dequeue();
                var currentState = statesToVisit.Dequeue();

                // Same amount of transitions in this state?
                Assert.AreEqual(nodes[currentNode].Count, epa[currentState].Count);

                foreach (var n in nodes[currentNode])
                {
                    if (!nodesSeen.Contains(n))
                    {
                        nodesToVisit.Enqueue(n);
                        nodesSeen.Add(n);
                    }
                }
                foreach (var s in epa[currentState])
                {
                    if (!statesSeen.Contains(s.TargetState))
                    {
                        statesToVisit.Enqueue(s.TargetState);
                        statesSeen.Add(s.TargetState);
                    }
                }

                // Same amount of new nodes/states to visit?
                Assert.AreEqual(nodesToVisit.Count, statesToVisit.Count);
                // Same amount of new nodes/states seen?
                Assert.AreEqual(nodesSeen.Count, statesSeen.Count);
            }
        }
    }
}
