using Modeler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Renderer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            Model model = Model.Cube();
            InitializeComponent();
            this.viewRoot.Children.Add(ToViewport3D(model));
        }


        static Viewport3D ToViewport3D(Model model) {
            // Define the camera
            PerspectiveCamera qCamera = new PerspectiveCamera();
            qCamera.Position = new Point3D(0, .25, 4.25);
            qCamera.LookDirection = new Vector3D(0, -.05, -1);
            qCamera.UpDirection = new Vector3D(0, 1, 0);
            qCamera.FieldOfView = 60;

            // Define a lighting model
            DirectionalLight qLight = new DirectionalLight();
            qLight.Color = Colors.Red;    
            qLight.Direction = new Vector3D(-0.5, -0.25, -0.5);

            MeshGeometry3D qFrontMesh = new MeshGeometry3D();
            qFrontMesh.Positions = new Point3DCollection(model.Vertices.Select(i => new Point3D(i.X, i.Y, i.Z)));
            qFrontMesh.TriangleIndices = new Int32Collection(model.FaceTriangleIndices);

            // Outer Tetrahedron (semi-transparent) : Define the mesh, material and transformation.
            GeometryModel3D qOuterGeometry = new GeometryModel3D();
            qOuterGeometry.Geometry = qFrontMesh;
            // *** Material ***
            DiffuseMaterial qDiffTransYellow =
                new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(64, 255, 255, 0)));
            SpecularMaterial qSpecTransWhite =
                new SpecularMaterial(new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)), 30.0);
            MaterialGroup qOuterMaterial = new MaterialGroup();
            qOuterMaterial.Children.Add(qDiffTransYellow);
            qOuterMaterial.Children.Add(qSpecTransWhite);
            qOuterGeometry.Material = qOuterMaterial;

            // Collect the components
            Model3DGroup qModelGroup = new Model3DGroup();
            qModelGroup.Children.Add(qLight);
            qModelGroup.Children.Add(qOuterGeometry);

            qOuterGeometry.Transform = new RotateTransform3D(new QuaternionRotation3D(new Quaternion(new Vector3D(1,1,1), 20)));
            ModelVisual3D qVisual = new ModelVisual3D();
            qVisual.Content = qModelGroup;
            Viewport3D qViewport = new Viewport3D();
            qViewport.Children.Add(qVisual);
            qViewport.Camera = qCamera;

            return qViewport;
        }


        private bool dragging = false;
        private Point lastMousePosition;
        private void viewRoot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.dragging = true;
            this.lastMousePosition = e.GetPosition(this.viewRoot);
        }

        private void viewRoot_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            this.dragging = false;
        }

        private void viewRoot_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (this.dragging) {
                var currentPosition = e.GetPosition(this.viewRoot);
                var diff = this.lastMousePosition - currentPosition;

                this.lastMousePosition = currentPosition;
            }
        }
    }
}