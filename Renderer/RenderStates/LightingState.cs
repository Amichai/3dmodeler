using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Renderer.RenderStates {
    class LightingState : RendererStateBase {
        public LightingState(Model model, FullScene scene) {
            this.model = model;
            this.scene = scene;
            this.addWidgets();
        }

        private FullScene scene;

        private void addWidgets() {
            StackPanel sp = new StackPanel();
            //sp.Children.Add(WidgetFactory.Button(
            //    "Ambient Light", 
            //    this.scene.ToggleLight1
            //    ));

            //sp.Children.Add(WidgetFactory.Button(
            //    "Directional Light",
            //    this.scene.ToggleLight2
            //    ));
            this.Widgets.Add(sp);
        }

        private Model model;

        public override string GetStateString() {
            throw new NotImplementedException();
        }

        internal override Modeler.Model GetModel() {
            throw new NotImplementedException();
        }
    }
}