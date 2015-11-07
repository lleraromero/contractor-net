using System;
using Contractor.Core.Model;
using Microsoft.Msagl.Drawing;

namespace Contractor.Gui
{
    internal interface IEpaViewer
    {
        Graph Graph { get; }
        void AddState(State state);

        void AddTransition(Transition transition);
    }
}