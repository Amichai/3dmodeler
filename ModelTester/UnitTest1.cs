using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modeler;
using System.Diagnostics;
using System.Collections.Generic;

namespace ModelTester {
    [TestClass]
    public class ModelTest {
        [TestMethod]
        public void TestConvexHull() {
            var vertices = new List<Vec3>() { 
                new Vec3(-1, -1, -1),
                new Vec3(-1, 1, 1),
                new Vec3(-1, -1, 1),
                new Vec3(-1, 1, -1),
            };
            var result = LinearAlgebra.OrderVertices(vertices);
            return;
            var m = Model.Cube();
            //foreach (var f in m.Faces) {
            //    var indices = f.GetVertexIndices();
            //    Debug.Print(string.Join(", ", indices));
            //}

            var newFace = new Face(new List<int> { 0, 1, 2, 3}, m);
            var a = newFace.GetVertexIndices();
            var positions = newFace.GetVertexPositions();
            foreach (var idx in a) {
                var p = positions[idx];
                Debug.Print(p.ToString());
            }
            
            newFace = new Face(new List<int> { 0, 1, 3, 2 }, m);
            var b = newFace.GetVertexIndices();

        }
    }
}
