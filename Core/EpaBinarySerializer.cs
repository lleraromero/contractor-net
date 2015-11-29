using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using Contractor.Core.Model;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = System.Drawing.Color;

namespace Contractor.Core
{
    public class EpaBinarySerializer : ISerializer
    {
        protected Epa epa;

        public void Serialize(Stream stream, Epa epa)
        {
            this.epa = epa;

            var graph = new Graph
            {
                Attr =
                {
                    OptimizeLabelPositions = true,
                    LayerDirection = LayerDirection.LR
                }
            };

            foreach (var s in epa.States)
            {
                AddState(s, graph);
            }

            foreach (var t in epa.Transitions)
            {
                AddTransition(t, graph);
            }

            RenderGraph(stream, graph);
        }

        private void RenderGraph(Stream stream, Graph graph)
        {
            var renderer = new GraphRenderer(graph);
            renderer.CalculateLayout();

            const float scale = 6.0f;
            var w = (int) (graph.Width*scale);
            var h = (int) (graph.Height*scale);

            using (var img = new Bitmap(w, h, PixelFormat.Format32bppPArgb))
            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                DrawGraph(g, w, h, scale, graph, renderer);

                renderer.Render(img);
                img.Save(stream, ImageFormat.Png);
            }
        }

        private void DrawGraph(Graphics g, int w, int h, float scale, Graph graph, GraphRenderer renderer)
        {
            var num1 = (float) (0.5*w - scale*(graph.Left + 0.5*graph.Width));
            var num2 = (float) (0.5*h + scale*(graph.Bottom + 0.5*graph.Height));

            using (var brush = new SolidBrush(Draw.MsaglColorToDrawingColor(graph.Attr.BackgroundColor)))
                g.FillRectangle(brush, 0, 0, w, h);

            using (var matrix = new Matrix(scale, 0f, 0f, -scale, num1, num2))
            {
                g.Transform = matrix;
                Draw.DrawPrecalculatedLayoutObject(g, renderer);
            }
        }

        private void AddState(State s, Graph graph)
        {
            var n = graph.AddNode(s.Name);

            n.UserData = s;
            n.DrawNodeDelegate += OnDrawNode;
            n.Attr.Shape = Shape.Circle;
            n.Attr.LabelMargin = 7;
            n.Label.FontName = "Cambria";
            n.Label.FontSize = 6;

            // TODO: Permitir elegir la descripcion
            //if (_Options.StateDescription)
            //{
            n.LabelText = s.ToString();
            //}
            //else
            //{
            //    n.LabelText = string.Format("S{0}", graph.NodeCount);
            //}
        }

        private bool OnDrawNode(Node node, object graphics)
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

            if ((node.UserData as State).Equals(epa.Initial))
            {
                const double offset = 3.1;
                x += offset/2.0;
                y += offset/2.0;
                w -= offset;
                h -= offset;

                g.DrawEllipse(Pens.Black, (float) x, (float) y, (float) w, (float) h);
            }

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

        private void AddTransition(Transition t, Graph graph)
        {
            var label = t.Action.ToString();
            var createEdge = true;
            var lineStyle = t.IsUnproven ? Style.Dashed : Style.Solid;


            var n = graph.FindNode(t.SourceState.Name);
            Contract.Assert(n != null);

            // TODO: Permitir elegir
            if ( /*_Options.UnprovenTransitions &&*/ t.IsUnproven)
                label = string.Format("{0}?", label);

            // TODO: Permitir elegir
            if ( /*_Options.CollapseTransitions*/ true)
            {
                var edges = n.OutEdges.Union(n.SelfEdges);

                foreach (var ed in edges)
                {
                    if (ed.Target == t.TargetState.Name && ed.Attr.Styles.Contains(lineStyle))
                    {
                        ed.LabelText = string.Format("{0}{1}{2}", ed.LabelText, Environment.NewLine, label);
                        createEdge = false;
                        break;
                    }
                }
            }

            if (createEdge)
            {
                var edge = graph.AddEdge(t.SourceState.Name, label, t.TargetState.Name);

                edge.Label.FontName = "Cambria";
                edge.Label.FontSize = 6;
                edge.Attr.AddStyle(lineStyle);
            }
        }
    }
}