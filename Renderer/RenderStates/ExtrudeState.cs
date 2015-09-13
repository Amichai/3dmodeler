using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Renderer.RenderStates {
    class ExtrudeState : RendererStateBase {
        public ExtrudeState(Model m, FullScene scene) {
            this.modelClone = m.Clone();
            var s = new Slider() {
                Minimum = -100,
                Maximum = 100,
                Value = 0,
                Orientation = Orientation.Vertical
            };
            s.ValueChanged += s_ValueChanged;
            this.Widgets.Add(s);
        }

        void s_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) {
            this.setSliderValue(e.NewValue);
            this.draw();
        }

        private void setSliderValue(double val) {
            this.toRender = this.modelClone.Clone();
            var faceToExtrude = this.toRender.Faces.First();
            this.toRender.RemoveFace(faceToExtrude);
            Vec3 normal = faceToExtrude.GetNormal();
            List<Vec3> newPositions = new List<Vec3>();
            List<int> indices = new List<int>();
            foreach (var v in faceToExtrude.GetVertexPositions()) {
                var newVertex = v + normal * val;
                newPositions.Add(newVertex);
                var idx = this.toRender.AddVertex(newVertex);
                indices.Add(idx);
            }
            this.toRender.AddFace(indices.ToArray());
        }

        public override string GetStateString() {
            return "Extrude State";
        }

        private Model toRender, modelClone;

        internal override Modeler.Model GetModel() {
            return this.toRender;
        }
    }
}
