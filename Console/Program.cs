﻿using Contractor.Core;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading;

namespace Contractor.Console
{
    class Program
    {
        private Options options;
        private Dictionary<string, Graph> graphs;

        public static int Main(string[] args)
        {
#if DEBUG
            var TempPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
            var GraphPath = Path.Combine(Directory.GetCurrentDirectory(), "Graph");
            if (!Directory.Exists(GraphPath))
                Directory.CreateDirectory(GraphPath);

            var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));

            args = new string[]
			{
				"-i", ExamplesPath,
				"-g", GraphPath,
				"-tmp", TempPath,
				"-il=true",
				"-t", "Examples.FiniteStack",
                "-b", "Corral"
			};
#endif
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            var options = new Options();

            System.Console.WriteLine();
            System.Console.WriteLine("Contractor.NET Version {0}", assemblyName.Version);
            System.Console.WriteLine("Copyright (C) LaFHIS - UBA. All rights reserved.");
            System.Console.WriteLine();

            args = args ?? new string[] { };
            options.Parse(args);

            if (!args.Any() || options.HelpRequested)
            {
                System.Console.WriteLine("usage: <general-option>*");
                System.Console.WriteLine();
                System.Console.WriteLine("where <general-option> is one of");
                options.PrintOptions(string.Empty);
            }
            else if (options.HasErrors)
            {
                options.PrintErrorsAndExit(System.Console.Error);
            }
            else
            {
                try
                {
                    var program = new Program(options);

                    EpaGenerator.Backend backend = EpaGenerator.Backend.Corral;
                    if (program.options.backend.Equals("CodeContracts", StringComparison.InvariantCultureIgnoreCase))
                        backend = EpaGenerator.Backend.CodeContracts;

                    // epas is a mapping between Typename and the result of the analysis.
                    Dictionary<string, TypeAnalysisResult> epas = program.Execute(backend);
#if DEBUG
                    // Export the EPA as an XML
                    var serializer = new EpaXmlSerializer();
                    using (Stream oStream = new FileStream(GraphPath, FileMode.Create))
                    {
                        serializer.Serialize(oStream, epas.First().Value.EPA);
                    }
#endif
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Error: {0}", ex.Message);
                }

                System.Console.WriteLine("Done!");
            }

#if DEBUG
            //System.Console.WriteLine("Press any key to continue");
            //System.Console.ReadKey();
#endif
            return 0;
        }

        public Program(Options options)
        {
            this.options = options;
            this.graphs = new Dictionary<string, Graph>();

            var wrkdir = Directory.GetCurrentDirectory();
            Configuration.Initialize();

            if (string.IsNullOrEmpty(options.output))
                options.output = Path.Combine(wrkdir, "Output");

            if (string.IsNullOrEmpty(options.graph))
                options.graph = Path.Combine(wrkdir, "Graph");

            if (!string.IsNullOrEmpty(options.temp))
                Configuration.TempPath = options.temp;

            if (!string.IsNullOrEmpty(options.cccheck))
                Configuration.CheckerFileName = options.cccheck;

            if (!string.IsNullOrEmpty(options.cccheckArgs))
                Configuration.CheckerArguments = options.cccheckArgs;

            Configuration.InlineMethodsBody = options.inline;

            if (!Directory.Exists(options.graph))
                Directory.CreateDirectory(options.graph);

            if (!Directory.Exists(Configuration.TempPath))
                Directory.CreateDirectory(Configuration.TempPath);
        }

        public Dictionary<string, TypeAnalysisResult> Execute(EpaGenerator.Backend backend)
        {
            var epas = new Dictionary<string, TypeAnalysisResult>();
            using (var generator = new EpaGenerator(backend))
            {
                generator.LoadAssembly(options.input);
                generator.TypeAnalysisStarted += typeAnalysisStarted;
                generator.TypeAnalysisDone += typeAnalysisDone;
                generator.StateAdded += stateAdded;
                generator.TransitionAdded += transitionAdded;

                var cancellationSource = new CancellationTokenSource();
                if (string.IsNullOrEmpty(options.type))
                    epas = generator.GenerateEpas(cancellationSource.Token);
                else
                    epas = new Dictionary<string, TypeAnalysisResult>() { { options.type, generator.GenerateEpa(options.type, cancellationSource.Token) } };

                if (options.generateAssembly)
                {
                    System.Console.WriteLine("Generating strengthened output assembly");
                    generator.GenerateOutputAssembly(options.output);
                }
            }
            return epas;
        }

        private void typeAnalysisStarted(object sender, TypeAnalysisStartedEventArgs e)
        {
            System.Console.WriteLine("Starting analysis for type {0}", e.TypeFullName);

            var graph = new Graph();
            graph.Attr.OptimizeLabelPositions = true;
            graph.Attr.LayerDirection = LayerDirection.LR;
            graphs.Add(e.TypeFullName, graph);
        }

        private void typeAnalysisDone(object sender, TypeAnalysisDoneEventArgs e)
        {
            var graph = graphs[e.TypeFullName];
            graphs.Remove(e.TypeFullName);

            var renderer = new GraphRenderer(graph);
            renderer.CalculateLayout();

            var scale = 6.0f;
            var w = (int)(graph.Width * scale);
            var h = (int)(graph.Height * scale);

            using (var img = new Bitmap(w, h, PixelFormat.Format32bppPArgb))
            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                renderer.Render(g, 0, 0, img.Width, img.Height);

                var imgName = e.TypeFullName;

                //Borramos del nombre los parametros de generics
                var start = imgName.IndexOf('<');

                if (start != -1)
                {
                    imgName = imgName.Remove(start);
                }

                imgName = string.Format("{0}.png", imgName);
                imgName = Path.Combine(options.graph, imgName);
                img.Save(imgName);
            }

            System.Console.WriteLine(e.AnalysisResult.ToString());
        }

        private void stateAdded(object sender, StateAddedEventArgs e)
        {
            var graph = graphs[e.TypeFullName];
            var n = graph.AddNode(e.State.Name);

            n.UserData = e.State;
            n.DrawNodeDelegate += OnDrawNode;
            n.Attr.Shape = Shape.Circle;
            n.Attr.LabelMargin = 7;
            n.Label.FontName = "Cambria";
            n.Label.FontSize = 6;

            if (options.stateDescription)
            {
                n.LabelText = string.Join<string>(Environment.NewLine, e.State.EnabledActions);
            }
            else
            {
                n.LabelText = string.Format("S{0}", graph.NodeCount);
            }
        }

        private void transitionAdded(object sender, TransitionAddedEventArgs e)
        {
            var graph = graphs[e.TypeFullName];
            var label = e.Transition.Action;
            var createEdge = true;

            if (options.collapseTransitions)
            {
                var n = graph.FindNode(e.SourceState.Name);

                if (options.unprovenTransitions && e.Transition.IsUnproven)
                    label = string.Format("{0}?", label);

                if (n != null)
                {
                    var edges = n.OutEdges.Union(n.SelfEdges);

                    foreach (var ed in edges)
                        if (ed.Target == e.Transition.TargetState.Name)
                        {
                            ed.LabelText = string.Format("{0}\n{1}", ed.LabelText, label);
                            createEdge = false;
                            break;
                        }
                }
            }

            if (createEdge)
            {
                var edge = graph.AddEdge(e.SourceState.Name, label, e.Transition.TargetState.Name);

                edge.Label.FontName = "Cambria";
                edge.Label.FontSize = 6;
            }
        }

        private bool OnDrawNode(Node node, object graphics)
        {
            var g = graphics as Graphics;
            var w = node.Attr.Width;
            var h = node.Attr.Height;
            var x = node.Attr.Pos.X - (w / 2.0);
            var y = node.Attr.Pos.Y - (h / 2.0);

            g.FillEllipse(Brushes.AliceBlue, (float)x, (float)y, (float)w, (float)h);
            g.DrawEllipse(Pens.Black, (float)x, (float)y, (float)w, (float)h);

            if ((node.UserData as IState).IsInitial)
            {
                const double offset = 3.1;
                x += offset / 2.0;
                y += offset / 2.0;
                w -= offset;
                h -= offset;

                g.DrawEllipse(Pens.Black, (float)x, (float)y, (float)w, (float)h);
            }

            using (var m = g.Transform)
            using (var saveM = m.Clone())
            {
                var c = (float)(2.0 * node.Label.Center.Y);
                x = node.Label.Center.X;
                y = node.Label.Center.Y;

                using (var m2 = new Matrix(1f, 0f, 0f, -1f, 0f, c))
                    m.Multiply(m2);

                g.Transform = m;

                using (var font = new Font(node.Label.FontName, node.Label.FontSize))
                using (var format = new StringFormat(StringFormat.GenericTypographic))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString(node.LabelText, font, Brushes.Black, (float)x, (float)y, format);
                }

                g.Transform = saveM;
            }

            return true;
        }
    }
}
