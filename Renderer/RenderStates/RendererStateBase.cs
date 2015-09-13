using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Renderer {
    public abstract class RendererStateBase {
        public Action draw;

        public abstract string GetStateString();

        public List<UIElement> Widgets = new List<UIElement>();

        abstract internal Model GetModel();
    }
}
