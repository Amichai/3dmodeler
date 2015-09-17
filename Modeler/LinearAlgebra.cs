using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeler {
    public static class LinearAlgebra {
        /// <summary>
        /// b is connected to a and c
        /// </summary>
        public static double Angle(Vec3 a, Vec3 b, Vec3 c) {
            double ba = b.Dist(a);
            double bc = b.Dist(c);
            double ac = a.Dist(c);
            return Math.Acos((ba.Sqrd() + bc.Sqrd() - ac.Sqrd()) / (2 * ba * bc));
        }

        private const double EPS = 1e-9;
        public enum SegmentPlaneIntersection { NoIntersection, SegmentLiesInPlane, Intersection };

        public static SegmentPlaneIntersection PlaneVectorIntersection(Vec3 a, Vec3 b, Vec3 planeNormal, Vec3 planePoint) {
            Vec3 u = b - a;
            Vec3 w = a - planePoint;
            double D = planeNormal.DotProduct(u);
            double N = -planeNormal.DotProduct(w);

            if (Math.Abs(D) < EPS) {
                if (N == 0) {
                    return SegmentPlaneIntersection.SegmentLiesInPlane;
                } else {
                    return SegmentPlaneIntersection.NoIntersection;
                }
            }
            double sI = N / D;
            if (sI < 0 || sI > 1) {
                return SegmentPlaneIntersection.NoIntersection;
            }
            return SegmentPlaneIntersection.Intersection;
        }

        //http://geomalgorithms.com/a13-_intersect-4.html
        public static SegmentPlaneIntersection PolygonVectorIntersection(Vec3 p0, Vec3 p1, Model model) {
            throw new Exception();
            var tE = 0;
            var tL = 1;
            var dS = p1 - p0;
            foreach (var f in model.Faces) {
                //var N = -(p0 - v)
            }
        }

        private static List<int> sortVertexIndices(List<Vec3> vertices) {
            Dictionary<int, Vec3> indexPt = new Dictionary<int, Vec3>();
            for (int i = 0; i < vertices.Count; i++) {
                indexPt[i] = vertices[i];
            }
            return indexPt.OrderBy(i => i.Value.X).ThenBy(i => i.Value.Y).ThenBy(i => i.Value.Z).Select(i => i.Key).ToList();
        }

        public static Vec3 GetCenter(List<Vec3> positions) {
            double xSum = 0, ySum = 0, zSum = 0;
            foreach (var pos in positions) {
                xSum += pos.X;
                ySum += pos.Y;
                zSum += pos.Z;
            }
            int ct = positions.Count;
            return new Vec3(xSum / ct, ySum / ct, zSum / ct);
        }

        private class comparer : IComparer<Vec3> {
            private Vec3 center, n;
            public comparer(Vec3 center, Vec3 normal) {
                this.center = center;
                this.n = normal;
            }
            public int Compare(Vec3 x, Vec3 y) {
                var a = x - center;
                var b = y - center;
                var det = this.n.DotProduct(a.CrossProduct(b));
                if (det < 0)
                    return 1;
                else if (det > 0)
                    return -1;
                return 0;
            }
        }

        public static Vec3 GetNormal(Vec3 v1, Vec3 v2, Vec3 v3) {
            var a = v2 - v1;
            var b = v3 - v2;
            var cross = a.CrossProduct(b);
            return cross.Normamlized();
        }

        public static List<Vec3> OrderVertices(List<Vec3> vertices, Vec3? normal = null) {
            Vec3 centerPt = GetCenter(vertices);
            if (normal == null) {
                normal = GetNormal(vertices[0], vertices[1], vertices[2]);
            }
            return vertices.OrderBy(i => i, new comparer(centerPt, normal.Value)).ToList();
        }
    }
}
