using Modeler;
using Renderer.RenderStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Renderer {
    class StateMachine {
        public StateMachine(Grid grid, Action draw) {
            this.widgetsRoot = grid;
            this.draw = draw;
        }

        private Action draw;
        private Grid widgetsRoot;

        private RendererStateBase currentState;
        public void SetState(RendererStateBase newState) {
            this.widgetsRoot.Children.Clear();
            foreach (var w in newState.Widgets) {
                this.widgetsRoot.Children.Add(w);
            }
            this.currentState = newState;
            this.currentState.draw = this.draw;
        }

        internal Model GetModel() {
            return this.currentState.GetModel();
        }

        internal void Commit() {
            this.SetState(new ViewerState(this.currentState.GetModel().Clone()));
        }

        internal void HandleRotationChanged(double p1, double p2) {
            this.currentState.HandleRotationChanged(p1, p2);
        }
    }
}
