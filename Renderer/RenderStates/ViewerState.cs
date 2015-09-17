using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer.RenderStates {
    class ViewerState : RendererStateBase {

        private Model model;
        public ViewerState(Model model) {
            this.model = model;
        }
        public override string GetStateString() {
            return "";
        }

        internal override Modeler.Model GetModel() {
            return this.model;
        }
    }
}
