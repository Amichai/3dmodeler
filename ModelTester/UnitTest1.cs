using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modeler;
using System.Diagnostics;
using System.Collections.Generic;

namespace ModelTester {
    [TestClass]
    public class ModelTest {
        [TestMethod]
        public void TestOrederVertices1() {
            var vertices = new List<Vec3>() { 
                new Vec3(-1, -1, -1),
                new Vec3(-1, 1, 1),
                new Vec3(-1, -1, 1),
                new Vec3(-1, 1, -1),
            };
            var result = LinearAlgebra.OrderVertices(vertices);
            for (int i = 0; i < 4; i++) {
                var a = result[i];
                var b = result[(i + 1) % 4];
                var totalDiff = a - b;
                Assert.IsTrue(totalDiff.Mag() <= 2);
            }
        }

        [TestMethod]
        public void TestOrederVertices2() {
            var vertices = new List<Vec3>() { 
                new Vec3(0, 1, 1),
                new Vec3(0, -1, 1),
                new Vec3(0, -1, -1),
                new Vec3(0, 1, -1),
            };
            var result = LinearAlgebra.OrderVertices(vertices);
            for (int i = 0; i < 4; i++) {
                var a = result[i];
                var b = result[(i + 1) % 4];
                var totalDiff = a - b;
                Assert.IsTrue(totalDiff.Mag() <= 2);
            }
        }
    }
}
