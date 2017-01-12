using System;
using System.Runtime.InteropServices;
using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models.Animations
{
    public interface IInterpolator<T, out TDest>
    {
        TDest Interpolate(float fac, ref T v1, ref T v2);
        TDest Interpolate(ref T v1);
        void SetInterpolationType(InterpolationType interpolation);
    }

    public class VectorInterpolator : IInterpolator<Vector3, Vector3>, IInterpolator<Vector4, Vector4>, IInterpolator<Vector2, Vector2>
    {
        #region Linear interpolation
        private static Vector3 InterpolateLinear(float fac, ref Vector3 v1, ref Vector3 v2) => Vector3.Lerp(v1, v2, fac);
        private static Vector4 InterpolateLinear(float fac, ref Vector4 v1, ref Vector4 v2) => Vector4.Lerp(v1, v2, fac);
        private static Vector2 InterpolateLinear(float fac, ref Vector2 v1, ref Vector2 v2) => Vector2.Lerp(v1, v2, fac);
        #endregion

        #region Hermite interpolation
        private static Vector3 InterpolateHermite(float fac, ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 v4)
        {
            var h1 = 2.0f * fac * fac * fac - 3.0f * fac * fac + 1.0f;
            var h2 = -2.0f * fac * fac * fac + 3.0f * fac * fac;
            var h3 = fac * fac * fac - 2.0f * fac * fac * fac;
            var h4 = fac * fac * fac - fac * fac;
            return v1 * h1 + v2 * h2 + v3 * h3 + v4 * h4;
        }

        private static Vector4 InterpolateHermite(float fac, ref Vector4 v1, ref Vector4 v2, ref Vector4 v3, ref Vector4 v4)
        {
            var h1 = 2.0f * fac * fac * fac - 3.0f * fac * fac + 1.0f;
            var h2 = -2.0f * fac * fac * fac + 3.0f * fac * fac;
            var h3 = fac * fac * fac - 2.0f * fac * fac * fac;
            var h4 = fac * fac * fac - fac * fac;
            return v1 * h1 + v2 * h2 + v3 * h3 + v4 * h4;
        }

        private static Vector2 InterpolateHermite(float fac, ref Vector2 v1, ref Vector2 v2, ref Vector2 v3, ref Vector2 v4)
        {
            var h1 = 2.0f * fac * fac * fac - 3.0f * fac * fac + 1.0f;
            var h2 = -2.0f * fac * fac * fac + 3.0f * fac * fac;
            var h3 = fac * fac * fac - 2.0f * fac * fac * fac;
            var h4 = fac * fac * fac - fac * fac;
            return v1 * h1 + v2 * h2 + v3 * h3 + v4 * h4;
        }
        #endregion

        #region Bezier interpolation
        private static Vector3 InterpolateBezier(float fac, ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 v4)
        {
            var inverseFactor = 1.0f - fac;
            var squaredFactor = fac * fac;
            var invSquaredFactor = inverseFactor * inverseFactor;

            var h1 = invSquaredFactor * inverseFactor;
            var h2 = 3.0f * fac * invSquaredFactor;
            var h3 = 3.0f * squaredFactor * invSquaredFactor;
            var h4 = squaredFactor * fac;
            return v1 * h1 + v2 * h2 + v3 * h3 + v4 * h4;
        }

        private static Vector4 InterpolateBezier(float fac, ref Vector4 v1, ref Vector4 v2, ref Vector4 v3, ref Vector4 v4)
        {
            var inverseFactor = 1.0f - fac;
            var squaredFactor = fac * fac;
            var invSquaredFactor = inverseFactor * inverseFactor;

            var h1 = invSquaredFactor * inverseFactor;
            var h2 = 3.0f * fac * invSquaredFactor;
            var h3 = 3.0f * squaredFactor * invSquaredFactor;
            var h4 = squaredFactor * fac;
            return v1 * h1 + v2 * h2 + v3 * h3 + v4 * h4;
        }

        private static Vector2 InterpolateBezier(float fac, ref Vector2 v1, ref Vector2 v2, ref Vector2 v3, ref Vector2 v4)
        {
            var inverseFactor = 1.0f - fac;
            var squaredFactor = fac * fac;
            var invSquaredFactor = inverseFactor * inverseFactor;

            var h1 = invSquaredFactor * inverseFactor;
            var h2 = 3.0f * fac * invSquaredFactor;
            var h3 = 3.0f * squaredFactor * invSquaredFactor;
            var h4 = squaredFactor * fac;
            return v1 * h1 + v2 * h2 + v3 * h3 + v4 * h4;
        }
        #endregion

        public Vector3 Interpolate(float fac, ref Vector3 v1, ref Vector3 v2)
        {
            switch (InterpolationType)
            {
                case InterpolationType.Linear:
                    return InterpolateLinear(fac, ref v1, ref v2);
            }
            return v1;
        }

        public Vector4 Interpolate(float fac, ref Vector4 v1, ref Vector4 v2)
        {
            switch (InterpolationType)
            {
                case InterpolationType.Linear:
                    return InterpolateLinear(fac, ref v1, ref v2);
            }
            return v1;
        }

        public Vector2 Interpolate(float fac, ref Vector2 v1, ref Vector2 v2)
        {
            switch (InterpolationType)
            {
                case InterpolationType.Linear:
                    return InterpolateLinear(fac, ref v1, ref v2);
                default:
                    Console.WriteLine("Unsupported interpolation type {0}!", InterpolationType);
                    break;
            }
            return v1;
        }

        public Vector2 Interpolate(ref Vector2 v1) => v1;
        public Vector3 Interpolate(ref Vector3 v1) => v1;
        public Vector4 Interpolate(ref Vector4 v1) => v1;

        public void SetInterpolationType(InterpolationType interpolation)
        {
            InterpolationType = interpolation;
        }

        private InterpolationType InterpolationType;
    }

    public class QuaternionInterpolator : IInterpolator<Quaternion16, Quaternion>, IInterpolator<InvQuaternion16, Quaternion>, IInterpolator<Quaternion, Quaternion>
    {
        public void SetInterpolationType(InterpolationType interpolation)
        {
            InterpolationType = interpolation;
        }

        private InterpolationType InterpolationType;

        private static Quaternion InterpolateLinear(float fac, ref Quaternion16 v1, ref Quaternion16 v2)
        {
            var q1 = v1.ToQuaternion();
            var q2 = v2.ToQuaternion();
            Quaternion ret;
            Quaternion.Slerp(ref q1, ref q2, fac, out ret);
            return ret;
        }

        private static Quaternion InterpolateLinear(float fac, ref InvQuaternion16 v1, ref InvQuaternion16 v2)
        {
            var q1 = v1.ToQuaternion();
            var q2 = v2.ToQuaternion();
            Quaternion ret;
            Quaternion.Slerp(ref q1, ref q2, fac, out ret);
            return ret;
        }

        private static Quaternion InterpolateLinear(float fac, ref Quaternion v1, ref Quaternion v2)
        {
            return Quaternion.Slerp(v1, v2, fac);
        }

        public Quaternion Interpolate(ref Quaternion v1) => v1;
        public Quaternion Interpolate(ref InvQuaternion16 v1) => v1.ToQuaternion();
        public Quaternion Interpolate(ref Quaternion16 v1) => v1.ToQuaternion();

        public Quaternion Interpolate(float fac, ref Quaternion v1, ref Quaternion v2)
        {
            switch (InterpolationType)
            {
                case InterpolationType.Linear:
                    return InterpolateLinear(fac, ref v1, ref v2);
            }
            return v1;
        }

        public Quaternion Interpolate(float fac, ref Quaternion16 v1, ref Quaternion16 v2)
        {
            switch (InterpolationType)
            {
                case InterpolationType.Linear:
                    return InterpolateLinear(fac, ref v1, ref v2);
            }
            return v1.ToQuaternion();
        }

        public Quaternion Interpolate(float fac, ref InvQuaternion16 v1, ref InvQuaternion16 v2)
        {
            switch (InterpolationType)
            {
                case InterpolationType.Linear:
                    return InterpolateLinear(fac, ref v1, ref v2);
            }
            return v1.ToQuaternion();
        }
    }

    public class NoInterpolate<T> : IInterpolator<T, T>
    {
        public T Interpolate(ref T v1) => v1;

        public T Interpolate(float fac, ref T v1, ref T v2) => v1;

        public void SetInterpolationType(InterpolationType interpolation) { }
    }

    public class NoInterpolateAlpha16 : IInterpolator<short, float>
    {
        public float Interpolate(float fac, ref short v1, ref short v2) => v1 / 32767.0f;

        public float Interpolate(ref short v1) => v1 / 32767.0f;

        public void SetInterpolationType(InterpolationType interpolation) { }
    }

    public class InterpolateAlpha16 : IInterpolator<short, float>
    {
        public float Interpolate(float fac, ref short v1, ref short v2) => ((1.0f - fac) * v1 + fac * v2) / 32767.0f;

        public float Interpolate(ref short v1) => v1 / 32767.0f;

        public void SetInterpolationType(InterpolationType interpolation) { }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quaternion16
    {
        private readonly short x;
        private readonly short y;
        private readonly short z;
        private readonly short w;

        public Quaternion ToQuaternion()
        {
            return new Quaternion(
                    (x < 0 ? x + 32768 : x - 32767) / 32767.0f,
                    (y < 0 ? y + 32768 : y - 32767) / 32767.0f,
                    (z < 0 ? z + 32768 : z - 32767) / 32767.0f,
                    (w < 0 ? w + 32768 : w - 32767) / 32767.0f);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InvQuaternion16
    {
        private readonly short x;
        private readonly short y;
        private readonly short z;
        private readonly short w;

        public Quaternion ToQuaternion()
        {
            return new Quaternion(
                    (x < 0 ? x + 32768 : x - 32767) / -32767.0f,
                    (y < 0 ? y + 32768 : y - 32767) / -32767.0f,
                    (z < 0 ? z + 32768 : z - 32767) / -32767.0f,
                    (w < 0 ? w + 32768 : w - 32767) / 32767.0f);
        }
    }
}
