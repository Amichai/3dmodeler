using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer {
    public abstract class RendererStateBase {
        public abstract string GetStateString();

        public virtual void SetSliderValue(double val) {
            return;
        }

        abstract internal Model GetModel();
    }
}
