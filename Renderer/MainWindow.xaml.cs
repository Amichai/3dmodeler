using Modeler;
using Renderer.RenderStates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            this.model = Model.Cube();
            this.scene = new FullScene();
            scene.Add(this.model);
            InitializeComponent();
            this.viewRoot.Children.Add(scene.Viewport);
            this.sm = new StateMachine(this.widgetsRoot, this.draw);
        }

        private Model model;
        private FullScene scene;

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
                this.scene.RotateModel(diff.X, diff.Y);
            }
        }

        private StateMachine sm;

        private void ElevateState_Click(object sender, RoutedEventArgs e) {
            sm.SetState(new ElevateState(this.model));
        }

        private void draw() {
            var toDraw = sm.GetModel();
            this.scene.DrawModel(toDraw);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            var toDraw = sm.GetModel();
            this.scene.DrawModel(toDraw);
        }

        private void LightingState_Click(object sender, RoutedEventArgs e) {
            sm.SetState(new LightingState(this.model, this.scene));
        }

        private void ExtrudeState_Click(object sender, RoutedEventArgs e) {
            sm.SetState(new ExtrudeState(this.model, this.scene));
        }
    }
}