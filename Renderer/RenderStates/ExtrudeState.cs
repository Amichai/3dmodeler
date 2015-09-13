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
            List<int> newIndices = new List<int>();
            foreach (var v in faceToExtrude.GetVertexPositions()) {
                var newVertex = v + normal * val;
                newPositions.Add(newVertex);
                var idx = this.toRender.AddVertex(newVertex);
                newIndices.Add(idx);
            }
            this.toRender.AddFace(newIndices.ToArray());
            var indices = faceToExtrude.GetVertexIndices();
            int ct = newIndices.Count;
            for (int i = 0; i < ct; i++) {
                var a1 = newIndices[i];
                var a2 = newIndices[(i + 1) % ct];
                var b1 = indices[i];
                var b2 = indices[(i + 1) % ct];
                this.toRender.AddFace(a1, a2, b1, b2);
            }
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
