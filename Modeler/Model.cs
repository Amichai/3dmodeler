using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeler {
    public class Model {
        private Model() { }
        private ListHash<Edge> edges = new ListHash<Edge>();
        private List<Face> faces = new List<Face>();
        private List<Vec3> vertices = new List<Vec3>();
        private Dictionary<int, List<Face>> vertexToFace = new Dictionary<int, List<Face>>();
        private Dictionary<int, List<Edge>> vertexToEdge = new Dictionary<int, List<Edge>>();

        public List<Face> Faces {
            get {
                return this.faces;
            }
        }

        public List<int> FaceTriangleIndices {
            get {
                return this.faces.SelectMany(i => i.GetTriangleIndices).ToList();
            }
        }

        public List<Vec3> Vertices {
            get {
                return this.vertices;
            }
        }

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

        private void addFaceVert(Face f, int v) {
            List<Face> faces;
            if (this.vertexToFace.TryGetValue(v, out faces)) {
                faces.Add(f);
            } else {
                this.vertexToFace[v] = new List<Face>() { f };
            }
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
            Model toReturn = new Model();
            toReturn.vertices = vertices;
            foreach (var faceSet in indices) {
                var newFace = new Face(faceSet, toReturn);
                toReturn.faces.Add(newFace);
                for (int i = 0; i < faceSet.Count; i++) {
                    var idx1 = i;
                    var idx2 = (i + 1) % faceSet.Count;
                    var e = new Edge(idx1, idx2);
                    toReturn.edges.Add(e);
                    toReturn.addEdgeVert(e, i);
                    toReturn.addFaceVert(newFace, i);
                }
            }
            return toReturn;
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
            return Model.ConstructFromVerticesAndIndices(vertices,
                new List<int> { 0, 1, 2, 3 },
                new List<int> { 7, 6, 5, 4 },
                new List<int> { 6, 0, 2, 4 },
                new List<int> { 5, 3, 7, 1 },
                new List<int> { 1, 6, 2, 5 },
                new List<int> { 7, 0, 4, 3 }
                );
        }

        public Vec3 GetFaceCenter(Face faceToExtrude) {
            return LinearAlgebra.GetCenter(faceToExtrude.GetVertexPositions());            
        }

        public int AddVertex(Vec3 newVertex) {
            this.vertices.Add(newVertex);
            return this.vertices.Count - 1;
        }

        public void AddFace(params int[] indices) {
            this.faces.Add(new Face(indices.ToList(), this));
        }

        public void RemoveFace(Face face) {
            this.faces.Remove(face);
        }

        public Model Clone() {
            Model toReturn = new Model();
            toReturn.vertices = this.vertices.Select(i => i.Clone()).ToList();
            toReturn.vertexToEdge = this.vertexToEdge.ToDictionary(i => i.Key, i => i.Value.Select(j => j.Clone()).ToList());
            toReturn.vertexToFace = this.vertexToFace.ToDictionary(i => i.Key, i => i.Value.Select(j => j.Clone()).ToList());
            toReturn.edges = new ListHash<Edge>(this.edges.ToList());
            toReturn.faces = this.faces.Select(i => i.Clone()).ToList();
            return toReturn;
        }

        public void Scale(double p) {
            vertices = vertices.Select(i => i /= p).ToList();
        }
    }

    public class Face {
        public Face(List<int> vertices, Model m) {
            this.vertices = vertices;
            this.model = m;

            Dictionary<Vec3, int> mapping = new Dictionary<Vec3, int>();
            for (int i = 0; i < vertices.Count; i++) {
                int idx = vertices[i];
                mapping[this.getVertexPosition(idx)] = idx;
            }
            var ordered = LinearAlgebra.OrderVertices(this.GetVertexPositions());
            this.vertices = ordered.Select(i => mapping[i]).ToList();
        }

        private Model model;

        private Vec3 getVertexPosition(int idx) {
            return this.model.Vertices[idx];
        }

        public List<Vec3> GetVertexPositions() {
            return this.vertices.Select(i => this.model.Vertices[i]).ToList();
        }

        public int VertexCount {
            get {
                return this.vertices.Count;
            }
        }

        public List<int> GetVertexIndices() {
            return this.vertices;
        }

        public List<int> GetTriangleIndices {
            get {
                if (this.vertices.Count == 3) {
                    return this.vertices;
                }
                List<int> toReturn = new List<int>();
                int sourceIdx = 0;
                for (int i = 0; i < this.vertices.Count - 2; i++) {
                    int idx1 = 1 + i;
                    int idx2 = 2 + i;
                    toReturn.Add(vertices[sourceIdx]);
                    toReturn.Add(vertices[idx1]);
                    toReturn.Add(vertices[idx2]);
                }
                return toReturn;
            }
        }

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

        internal Face Clone() {
            return new Face(this.vertices.ToList(), this.model);
        }

        public Vec3 GetNormal() {
            var v1 = this.getVertexPosition(0);
            var v2 = this.getVertexPosition(1);
            var v3 = this.getVertexPosition(2);
            return LinearAlgebra.GetNormal(v1, v2, v3);
        }
    }

    public class Edge {
        public Edge(int a, int b) {
            this.Vert1 = a;
            this.Vert2 = b;
        }
        public int Vert1 { get; private set; }
        public int Vert2 { get; private set; }
        
        public static bool operator ==(Edge e1, Edge e2) {
            return e1.Vert1 == e2.Vert1 && e1.Vert2 == e2.Vert2;
        }

        public static bool operator !=(Edge e1, Edge e2) {
            return !(e1 == e2);
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

        internal Edge Clone() {
            return new Edge(this.Vert1, this.Vert2);
        }
    }
}
