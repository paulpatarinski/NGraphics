﻿using System;

namespace NGraphics.Custom.Models.Transforms
{
    public struct Transform
    {
        public double A, B, C, D, E, F;

        public static readonly Transform Identity = new Transform(1, 0, 0, 1, 0, 0);

        public Transform(double a, double b, double c, double d, double e, double f)
        {
            A = a; B = b; C = c; D = d; E = e; F = f;
        }

        public override bool Equals(object obj)
        {
            if (obj is Transform)
            {
                var y = (Transform)obj;
                return A == y.A && B == y.B && C == y.C && D == y.D && E == y.E && F == y.F;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() + 2 * B.GetHashCode() + 3 * C.GetHashCode() + 5 * D.GetHashCode() + 7 * E.GetHashCode() + 11 * F.GetHashCode();
        }

        public static bool operator ==(Transform x, Transform y)
        {
            return x.A == y.A && x.B == y.B && x.C == y.C && x.D == y.D && x.E == y.E && x.F == y.F;
        }

        public static bool operator !=(Transform x, Transform y)
        {
            return x.A != y.A || x.B != y.B || x.C != y.C || x.D != y.D || x.E != y.E || x.F != y.F;
        }

        public override string ToString()
        {
            return string.Format("matrix({0}, {1}, {2}, {3}, {4}, {5})", A, B, C, D, E, F);
        }

        public Point TransformPoint(Point point)
        {
            return new Point(
                A * point.X + C * point.Y + E,
                B * point.X + D * point.Y + F);
        }

        public static Transform operator *(Transform x, Transform y)
        {
            return new Transform(
                x.A * y.A + x.C * y.B,
                x.B * y.A + x.D * y.B,
                x.A * y.C + x.C * y.D,
                x.B * y.C + x.D * y.D,
                x.A * y.E + x.C * y.F + x.E,
                x.B * y.E + x.D * y.F + x.F);
        }

        public static Transform Translate(double x, double y)
        {
            return new Transform(1, 0, 0, 1, x, y);
        }

        public static Transform Translate(Size size)
        {
            return Translate(size.Width, size.Height);
        }

        public static Transform Translate(Point point)
        {
            return Translate(point.X, point.Y);
        }

        public static Transform Scale(double x, double y)
        {
            return new Transform(x, 0, 0, y, 0, 0);
        }

        public static Transform Scale(double scale)
        {
            return new Transform(scale, 0, 0, scale, 0, 0);
        }

        public static Transform Scale(Size size)
        {
            return Scale(size.Width, size.Height);
        }

        public static Transform Scale(Point point)
        {
            return Scale(point.X, point.Y);
        }

        public static Transform Rotate(double angleInDegrees)
        {
            var a = angleInDegrees * (Math.PI / 180.0);
            var ca = Math.Cos(a);
            var sa = Math.Sin(a);
            return new Transform(ca, sa, -sa, ca, 0, 0);
        }

        public Transform GetInverse()
        {
            var det = A * D - B * C;
            if (det == 0)
            {
                throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
            }
            var r = 1.0 / det;
            return new Transform(D * r, -B * r, -C * r, A * r, -(D * E - C * F) * r, (B * E - A * F) * r);
        }
    }
}
