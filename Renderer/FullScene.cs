using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Renderer {
    class FullScene {
        public FullScene() {
            // Define the camera
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, .25, 10);
            camera.LookDirection = new Vector3D(0, -.05, -1);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 60;

            // Define a lighting model
            DirectionalLight light = new DirectionalLight();
            light.Color = Colors.White;
            light.Direction = new Vector3D(-0.5, -0.25, -0.5);

            // Collect the components
            this.modelGroup = new Model3DGroup();
            this.modelGroup.Children.Add(light);

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = this.modelGroup;
            Viewport3D viewport = new Viewport3D();
            viewport.Children.Add(visual);
            viewport.Camera = camera;

            this.Viewport = viewport;
        }

        private Model3DGroup modelGroup;
        public Viewport3D Viewport { get; private set; }
        GeometryModel3D geometry;

        public void DrawModel(Model model) {
            this.modelGroup.Children.Remove(this.geometry);
            this.Add(model);
        }

        private MeshGeometry3D mesh;

        public void Add(Model model) {
            this.mesh = new MeshGeometry3D();
            this.mesh.Positions = new Point3DCollection(model.Vertices.Select(i => new Point3D(i.X, i.Y, i.Z)));
            this.mesh.TriangleIndices = new Int32Collection(model.FaceTriangleIndices);
            this.geometry = new GeometryModel3D();
            this.geometry.Geometry = this.mesh;
            this.modelGroup.Children.Add(this.geometry);

            // *** Material ***
            DiffuseMaterial diffTransYellow =
                new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(64, 255, 255, 0)));
            SpecularMaterial specTransWhite =
                new SpecularMaterial(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)), 30.0);
            MaterialGroup material = new MaterialGroup();
            material.Children.Add(diffTransYellow);
            material.Children.Add(specTransWhite);

            geometry.Material = material;
        }

        private double lastAngleX = 0, lastAngleY = 0;
        public void RotateModel(double dx, double dy) {
            var rotations = new Transform3DCollection();
            var transformGroup = new Transform3DGroup() {
                Children = rotations
            };

            geometry.Transform = transformGroup;

            lastAngleX -= dx / 10;
            rotations.Add(new RotateTransform3D(
                new QuaternionRotation3D(
                    new Quaternion(new Vector3D(0, 1, 0), lastAngleX)
                    )
                )
            );
            lastAngleY -= dy / 10;
            rotations.Add(new RotateTransform3D(
                new QuaternionRotation3D(
                    new Quaternion(new Vector3D(1, 0, 0), lastAngleY)
                    )
                )
           );
        }
    }
}
