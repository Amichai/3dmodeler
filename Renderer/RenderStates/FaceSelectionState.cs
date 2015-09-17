using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer.RenderStates {
    class FaceSelectionState : RendererStateBase {
        public FaceSelectionState(Model model) {
            this.model = model;
        }

        private Model model;

        public override string GetStateString() {
            return "";
        }

        internal override Modeler.Model GetModel() {
            throw new NotImplementedException();
        }

        public override void HandleRotationChanged(double angleX, double angleY) {
            var a = new Vec3(0,0,0);
            var b = new Vec3(0,0,1);
            foreach(var f in this.model.Faces){
                var n = f.GetNormal();
                LinearAlgebra.PlaneVectorIntersection(a, b, n, f.GetVertexPositions().First());
            }
            base.HandleRotationChanged(angleX, angleY);
        }
    }
}
