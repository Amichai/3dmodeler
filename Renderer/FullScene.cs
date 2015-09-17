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
        //public Light Lighting { get; private set; }
        public FullScene() {
            // Define the camera
            var camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, .25, 10);
            camera.LookDirection = new Vector3D(0, 0, -1);
            
            //camera.LookDirection = new Vector3D(0, -.05, -1);
            camera.UpDirection = new Vector3D(0, 1, 0);
            camera.FieldOfView = 60;



            //var camera = new OrthographicCamera();
            //camera.Position = new Point3D(0, 0, 85);
            //camera.LookDirection = new Vector3D(0, -.05, -4);   
            
            
            
            //camera.NearPlaneDistance = 0;
            //camera.FarPlaneDistance = 100;
            ColorConverter conv = new ColorConverter();
            var color = (Color)conv.ConvertFrom("#ffffff");
            //var ambient = new AmbientLight(color);
            var pos = new Point3D(0, 10, 10);
            var spotlight = new SpotLight();
            spotlight.Color = color;    
            spotlight.Position = pos;
            spotlight.InnerConeAngle = 180;
            spotlight.OuterConeAngle = 90;
            spotlight.Direction = new Vector3D(pos.X * -1, pos.Y * -1, pos.Z * -1);

            // Define a lighting model
            // Collect the components
            this.modelGroup = new Model3DGroup();
            this.modelGroup.Children.Add(ambientLight);
            this.modelGroup.Children.Add(spotlight);

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = this.modelGroup;
            Viewport3D viewport = new Viewport3D();
            
            viewport.Children.Add(visual);
            viewport.Camera = camera;

            this.Viewport = viewport;
        }

        private AmbientLight ambientLight = new AmbientLight() {
            Color = (Color)new ColorConverter().ConvertFrom("#3fffff")
        };

        private Model3DGroup modelGroup;
        public Viewport3D Viewport { get; private set; }
        GeometryModel3D geometry;

        public void DrawModel(Model model) {
            this.modelGroup.Children.Remove(this.geometry);
            this.Add(model);
            this.setRotation();
        }

        private MeshGeometry3D mesh;

        public void Add(Model model) {
            this.mesh = new MeshGeometry3D();
            this.mesh.Positions = new Point3DCollection(model.Vertices.Select(i => new Point3D(i.X, i.Y, i.Z)));
            this.mesh.TriangleIndices = new Int32Collection(model.FaceTriangleIndices);
            this.geometry = new GeometryModel3D();
            this.geometry.Geometry = this.mesh;
            this.modelGroup.Children.Add(this.geometry);

            var m1 = new DiffuseMaterial(Brushes.Red);
            MaterialGroup material = new MaterialGroup();
            material.Children.Add(m1);
            
            geometry.Material = material;
            geometry.BackMaterial = material;
        }

        private void setRotation() {
            var rotations = new Transform3DCollection();
            var transformGroup = new Transform3DGroup() {
                Children = rotations
            };

            geometry.Transform = transformGroup;
            rotations.Add(new RotateTransform3D(
                new QuaternionRotation3D(
                    new Quaternion(new Vector3D(0, 1, 0), AngleX)
                    )
                )
            );
            rotations.Add(new RotateTransform3D(
                new QuaternionRotation3D(
                    new Quaternion(new Vector3D(1, 0, 0), AngleY)
                    )
                )
           );
        }

        public double AngleX = 0, AngleY = 0;
        public void RotateModel(double dx, double dy) {
            AngleX -= dx / 10;
            AngleY -= dy / 10;
            this.setRotation();
        }
    }
}
