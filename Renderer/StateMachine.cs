using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer {
    class StateMachine {
        private RendererStateBase currentState;
        public void SetState(RendererStateBase newState) {
            this.currentState = newState;
        }

        internal void SetSliderValue(double val) {
            if (this.currentState == null) {
                return;
            }
            this.currentState.SetSliderValue(val);
        }

        internal Model GetModel() {
            return this.currentState.GetModel();
        }
    }
}
