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
            

            // Define a lighting model
            // Collect the components
            this.modelGroup = new Model3DGroup();
            this.modelGroup.Children.Add(dir1);
            this.modelGroup.Children.Add(dir2);
            this.modelGroup.Children.Add(dir3);
            //this.modelGroup.Children.Add(dir4);

            //this.Lighting = light;

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = this.modelGroup;
            Viewport3D viewport = new Viewport3D();
            
            viewport.Children.Add(visual);
            viewport.Camera = camera;

            this.Viewport = viewport;
        }


        private AmbientLight ambientLight = new AmbientLight() {
            Color = Colors.White,
        };

        private PointLight dir1 = new PointLight() {
            Color = Colors.White,
            Position = new Point3D(0, 0, 10),
            LinearAttenuation = 0,
            QuadraticAttenuation = 0,
            Range = 20,
        };
        private PointLight dir2 = new PointLight() {
            Color = Colors.White,
            Position = new Point3D(0, 10, 0),
            LinearAttenuation = 0,
            QuadraticAttenuation = 0,
            Range = 20,
        };

        private PointLight dir3 = new PointLight() {
            Color = Colors.White,
            Position = new Point3D(10, 0, 0),
            LinearAttenuation = 0,
            QuadraticAttenuation = 0,
            Range = 20,
        };
        private PointLight dir4 = new PointLight() {
            Color = Colors.Yellow,
            //Direction = new Vector3D(-1, 1, 1)
        };
        private bool ambientLightOn = false;
        public void ToggleLight1() {
            if (!ambientLightOn) {
                this.modelGroup.Children.Add(ambientLight);
            } else {
                this.modelGroup.Children.Remove(ambientLight);
            }
        }

        public void ToggleLight2() {
            throw new Exception();
        }

        public void ToggleLight3() {
            throw new Exception();
        }

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

            
            // *** Material ***
            var mat = new SpecularMaterial(new SolidColorBrush(Colors.Red), 2);
            //DiffuseMaterial diffTransYellow =
            //    new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 255, 255, 0)));
            MaterialGroup material = new MaterialGroup();
            material.Children.Add(mat);

            geometry.Material = material;
        }

        private void setRotation() {
            var rotations = new Transform3DCollection();
            var transformGroup = new Transform3DGroup() {
                Children = rotations
            };

            geometry.Transform = transformGroup;
            rotations.Add(new RotateTransform3D(
                new QuaternionRotation3D(
                    new Quaternion(new Vector3D(0, 1, 0), lastAngleX)
                    )
                )
            );
            rotations.Add(new RotateTransform3D(
                new QuaternionRotation3D(
                    new Quaternion(new Vector3D(1, 0, 0), lastAngleY)
                    )
                )
           );
        }

        private double lastAngleX = 0, lastAngleY = 0;
        public void RotateModel(double dx, double dy) {
            lastAngleX -= dx / 10;
            lastAngleY -= dy / 10;
            this.setRotation();
        }
    }
}
