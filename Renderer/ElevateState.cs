using Modeler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Renderer {
    public class ElevateState : RendererStateBase {
        public ElevateState(Model m) {
            this.modelClone = m.Clone();
        }

        private Face faceToElevate;
        private Vec3 faceCenter;

        private Model modelClone;
        public override string GetStateString() {
            return "Elevate";
        }

        internal override Model GetModel() {
            return this.toRender;
        }

        private Model toRender;

        public override void SetSliderValue(double val) {
            this.toRender = this.modelClone.Clone();
            this.faceToElevate = this.toRender.Faces.First();
            this.faceCenter = this.toRender.GetFaceCenter(this.faceToElevate);
            this.toRender.RemoveFace(this.faceToElevate);
            Vec3 newVertex = this.faceCenter.Extend(val);
            var faceVertexIndices = this.faceToElevate.GetVertexIndices();
            int ct = faceVertexIndices.Count;
            for (int i = 0; i < ct; i++) {
                int idx1 = faceVertexIndices[i];
                int idx2 = faceVertexIndices[(i + 1) % ct];
                int vIndex = this.toRender.AddVertex(newVertex);
                this.toRender.AddFace(idx1, idx2, vIndex);
                this.toRender.AddFace(vIndex, idx2, idx1);

            }
            //Get the center of the face
            //Determine the new vertex position
            //Add n new triangular faces
            //remove the old face
        }
    }
}
