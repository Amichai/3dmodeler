using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeler {
    public struct Vec3 {
        public Vec3(double x, double y, double z) : this() {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public static Vec3 operator/(Vec3 a, double b) {
            return new Vec3(a.X / b, a.Y / b, a.Z / b);
        }

        public static Vec3 operator*(Vec3 a, double b) {
            return new Vec3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vec3 operator +(Vec3 a, Vec3 b) {
            return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vec3 operator-(Vec3 a, Vec3 b) {
            return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static bool operator==(Vec3 a, Vec3 b)   {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }

        public static bool operator!=(Vec3 a, Vec3 b) {
            return !(a == b);
        }

        // override object.Equals
        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            return this.X == ((Vec3)obj).X
                   && this.Y == ((Vec3)obj).Y
                   && this.Z == ((Vec3)obj).Z;
        }

        // override object.GetHashCode
        public override int GetHashCode() {
            return this.X.GetHashCode() ^ this.Y.GetHashCode() ^ this.Z.GetHashCode();
        }

        public double Mag() {
            return Math.Sqrt(this.X.Sqrd() + this.Y.Sqrd() + this.Z.Sqrd());
        }

        public Vec3 Normamlized() {
            var mag = this.Mag();
            return new Vec3(this.X / mag, this.Y / mag, this.Z / mag);
        }

        public Vec3 Extend(double val) {
            return this.Normamlized() * val;
        }

        ///TODO: probably not necessary, because vec3 is a struct
        internal Vec3 Clone() {
            return new Vec3(this.X, this.Y, this.Z);
        }

        internal double Dist(Vec3 b) {
            return Math.Sqrt((b.X - this.X).Sqrd() + (b.Y - this.Y).Sqrd());
        }

        public override string ToString() {
            return string.Format("({0},{1},{2})", this.X, this.Y, this.Z);
        }

        internal Vec3 CrossProduct(Vec3 v) {
            double x = this.Y * v.Z - this.Z * v.Y;
            double y = this.Z * v.X - this.X * v.Z;
            double z = this.X * v.Y - this.Y * v.X;
            return new Vec3(x, y, z);
        }

        internal double DotProduct(Vec3 vec3) {
            return this.X * vec3.X + this.Y * vec3.Y + this.Z * vec3.Z;
        }
    }
}
