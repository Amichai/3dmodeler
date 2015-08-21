using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeler {
    class Model {
        private ListHash<Edge> edges = new ListHash<Edge>();
        private List<Face> faces = new List<Face>();
        private List<Vec3> vertices = new List<Vec3>();
        private Dictionary<int, List<Face>> vertexToFace = new Dictionary<int, List<Face>>();
        private Dictionary<int, List<Edge>> vertexToEdge = new Dictionary<int, List<Edge>>();

        public void AddEdge(Edge e) {
            if (this.edges.Contains(e)) {
                Debug.Assert(this.vertexToEdge[e.Vert1].Contains(e));
                Debug.Assert(this.vertexToEdge[e.Vert2].Contains(e));
                return;
            }
            this.edges.Add(e);
            this.addEdgeVert(e, e.Vert1);
            this.addEdgeVert(e, e.Vert2);
        }

        public void Normalize() {
            var maxDist = this.vertices.Max(i => i.Mag());
            vertices = vertices.Select(i => i /= maxDist).ToList();
        }

        private void addEdgeVert(Edge e, int v1) {
            List<Edge> edges;
            if (this.vertexToEdge.TryGetValue(v1, out edges)) {
                edges.Add(e);
            } else {
                this.vertexToEdge[v1] = new List<Edge>() { e };
            }
        }

        public bool Contains(Edge e) {
            return this.edges.Contains(e);
        }

        public void AssertConsistency() {
            Debug.Assert(this.vertexToEdge.Count == this.vertexToFace.Count);
            HashSet<int> seen = new HashSet<int>();
            foreach (var v in this.vertexToEdge) {
                Debug.Assert(seen.Contains(v.Key));
                foreach (var e in v.Value) {
                    Debug.Assert(this.edges.Contains(e));
                }
            }
        }

        public static Model ConstructFromVerticesAndIndices(List<Vec3> vertices, params List<int>[] indices) {
            throw new NotImplementedException();
        }

        public static Model ConstructFromFaces(List<Face> faces) {
            Model toReturn = new Model();
            toReturn.faces = faces;
            foreach (var f in faces) {
                var edges = f.GetEdges();
                edges.ForEach(e => toReturn.AddEdge(e));
            }
            return toReturn;
        }

        public static Model Cube() {
            List<Vec3> vertices = new List<Vec3>() { 
                new Vec3(-1, -1, -1),
                new Vec3(-1, 1, 1),
                new Vec3(-1, -1, 1),
                new Vec3(-1, 1, -1),
                new Vec3(1, -1, -1),
                new Vec3(1, 1, 1),
                new Vec3(1, -1, 1),
                new Vec3(1, 1, -1),
            };
            Model.ConstructFromVerticesAndIndices(vertices, 
                new List<int> { 0, 1, 2, 3 }, 
                new List<int> { 4, 5, 6, 7 },
                new List<int> { 0, 1, 4, 5 },
                new List<int> { 2, 3, 6, 7 },
                new List<int> { 1, 2, 5, 6 },
                new List<int> { 3, 0, 4, 7 }
                );
            throw new NotImplementedException();
        }
    }

    class Face {
        private List<int> vertices = new List<int>();
        public List<Edge> GetEdges() {
            List<Edge> toReturn = new List<Edge>();
            int ct = vertices.Count;
            for (int i = 0; i < ct; i++) {
                Edge e = new Edge(i, i + 1 % ct);
                toReturn.Add(e);
            }
            return toReturn;
        }
    }

    class Edge {
        public Edge(int a, int b) {
            this.Vert1 = a;
            this.Vert2 = b;
        }
        public int Vert1 { get; private set; }
        public int Vert2 { get; private set; }
        
        public static bool operator ==(Edge e1, Edge e2) {
            return e1.Vert1 == e2.Vert1 && e1.Vert2 == e2.Vert2;
        }
        
        // override object.Equals
        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }

            Edge e = (Edge)obj;
            return this.Vert1 == e.Vert1 && this.Vert2 == e.Vert2;
        }

        public override int GetHashCode() {
            return this.Vert1.GetHashCode() ^ this.Vert2.GetHashCode();
        }
    }
}
