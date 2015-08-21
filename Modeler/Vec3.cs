using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modeler {
    struct Vec3 {
        public Vec3(double x, double y, double z) {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public static Vec3 operator /(Vec3 a, double b) {
            return new Vec3(a.X / b, a.Y / b, a.Z / b);
        }

        public static bool operator==(Vec3 a, Vec3 b)   {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
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
    }
}
