using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Contractor.Core.Model;
using Microsoft.Msagl.Drawing;
using Color = System.Drawing.Color;

namespace Contractor.Gui
{
    internal class EpaViewer : IEpaViewer
    {
        protected Graph graph;

        public EpaViewer()
        {
            graph = new Graph
            {
                Attr =
                {
                    OptimizeLabelPositions = true,
                    LayerDirection = LayerDirection.LR
                }
            };
        }

        public void AddState(State state)
        {
            var n = graph.AddNode(state.Name);

            n.UserData = state;
            n.DrawNodeDelegate += OnDrawNode;
            n.Attr.Shape = Shape.Circle;
            n.Attr.LabelMargin = 7;
            n.Label.FontName = "Cambria";
            n.Label.FontSize = 6;

            // TODO: Permitir elegir la descripcion
            //if (_Options.StateDescription)
            //{
            n.LabelText = state.ToString();
            //}
            //else
            //{
            //    n.LabelText = string.Format("S{0}", graph.NodeCount);
            //}
        }

        public void AddTransition(Transition transition)
        {
            var label = transition.Action.ToString();
            var createEdge = true;
            var lineStyle = transition.IsUnproven ? Style.Dashed : Style.Solid;


            var n = graph.FindNode(transition.SourceState.Name);
            Contract.Assert(n != null);

            // TODO: Permitir elegir
            if ( /*_Options.UnprovenTransitions &&*/ transition.IsUnproven)
                label = string.Format("{0}?", label);

            // TODO: Permitir elegir
            if ( /*_Options.CollapseTransitions*/ true)
            {
                var edges = n.OutEdges.Union(n.SelfEdges);

                foreach (var ed in edges)
                {
                    if (ed.Target == transition.TargetState.Name && ed.Attr.Styles.Contains(lineStyle))
                    {
                        ed.LabelText = string.Format("{0}{1}{2}", ed.LabelText, Environment.NewLine, label);
                        createEdge = false;
                        break;
                    }
                }
            }

            if (createEdge)
            {
                var edge = graph.AddEdge(transition.SourceState.Name, label, transition.TargetState.Name);

                edge.Label.FontName = "Cambria";
                edge.Label.FontSize = 6;
                edge.Attr.AddStyle(lineStyle);
            }
        }

        public Graph Graph
        {
            get { return graph; }
        }

        protected bool OnDrawNode(Node node, object graphics)
        {
            var g = graphics as Graphics;
            var w = node.Attr.Width;
            var h = node.Attr.Height;
            var x = node.Attr.Pos.X - w/2.0;
            var y = node.Attr.Pos.Y - h/2.0;

            g.FillEllipse(Brushes.AliceBlue, (float) x, (float) y, (float) w, (float) h);

            var penWidth = /*(_SelectedGraphNode != null && _SelectedGraphNode.Node == node ? 2f : 1f);*/ 1f;
            using (var pen = new Pen(Color.Black, penWidth))
                g.DrawEllipse(pen, (float) x, (float) y, (float) w, (float) h);

            // TODO: arreglar que el inicial se vea distinto
            //if ((node.UserData as State).Equals(this.epa.Initial))
            //{
            //    const double offset = 3.1;
            //    x += offset / 2.0;
            //    y += offset / 2.0;
            //    w -= offset;
            //    h -= offset;

            //    g.DrawEllipse(Pens.Black, (float)x, (float)y, (float)w, (float)h);
            //}

            using (var m = g.Transform)
            using (var saveM = m.Clone())
            {
                var c = (float) (2.0*node.Label.Center.Y);
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

                    g.DrawString(node.LabelText, font, Brushes.Black, (float) x, (float) y, format);
                }

                g.Transform = saveM;
            }

            return true;
        }
    }
}