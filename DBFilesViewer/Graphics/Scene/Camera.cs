using System;
using SharpDX;

namespace DBFilesViewer.Graphics.Scene
{
    public class Camera
    {
        private Matrix mView;
        private Matrix mProj;

        private Matrix mViewInverse;
        private Matrix mProjInverse;

        private Vector3 mTarget;
        private Vector3 mUp;
        private Vector3 mRight;
        private Vector3 mForward;
        private readonly ViewFrustum mFrustum = new ViewFrustum();

        public event Action<Camera, Matrix> ViewChanged, ProjectionChanged;

        public bool LeftHanded { get; set; }

        public Matrix View => mView;
        public Matrix Projection => mProj;
        public Matrix ViewProjection { get; private set; }

        public Matrix ViewInverse => mViewInverse;
        public Matrix ProjectionInverse => mProjInverse;

        public Vector3 Position { get; private set; }

        public Vector3 Up => mUp;
        public Vector3 Right => mRight;
        public Vector3 Forward => mForward;

        protected Camera()
        {
            mUp = Vector3.UnitZ;
            Position = new Vector3();
            mTarget = Vector3.UnitX;
            mRight = -Vector3.UnitY;
            mForward = Vector3.UnitX;

            UpdateView();
        }

        public bool Contains(ref BoundingBox box)
        {
            return mFrustum.Contains(ref box) != ContainmentType.Disjoint;
        }

        public bool Contains(ref BoundingSphere sphere)
        {
            return mFrustum.Contains(ref sphere) != ContainmentType.Disjoint;
        }

        public virtual void Update()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            mForward = mTarget - Position;
            mForward.Normalize();

            mView = (LeftHanded == false)
                ? Matrix.LookAtRH(Position, mTarget, mUp)
                : Matrix.LookAtLH(Position, mTarget, mUp);
            Matrix.Invert(ref mView, out mViewInverse);

            mFrustum.Update(mView, mProj);
            ViewProjection = mView * mProj;

            ViewChanged?.Invoke(this, mView);
        }

        protected void OnProjectionChanged(ref Matrix matProj)
        {
            mProj = matProj;
            Matrix.Invert(ref mProj, out mProjInverse);

            mFrustum.Update(mView, mProj);
            ViewProjection = mView * mProj;

            ProjectionChanged?.Invoke(this, mProj);
        }

        public void SetPosition(Vector3 position)
        {
            Position = position;
            UpdateView();
        }

        public void SetTarget(Vector3 target)
        {
            mTarget = target;
            UpdateView();
        }

        public void SetParameters(Vector3 eye, Vector3 target, Vector3 up, Vector3 right)
        {
            mTarget = target;
            Position = eye;
            mUp = up;
            mRight = right;

            UpdateView();
        }

        public void Move(Vector3 amount)
        {
            Position += amount;
            mTarget += amount;
            UpdateView();
        }

        public void MoveUp(float amount) => Move(Vector3.UnitZ * amount);
        public void MoveDown(float amount) => Move(Vector3.UnitZ * -amount);
        public void MoveForward(float amount) => Move(mForward * amount);
        public void MoveRight(float amount) => Move(mRight * amount * (LeftHanded ? -1 : 1));
        public void MoveLeft(float amount) => Move(mRight * amount * (LeftHanded ? 1 : -1));

        public void Pitch(float angle)
        {
            var matRot = Matrix.RotationAxis(mRight, MathUtil.DegreesToRadians(angle));
            mUp = Vector3.TransformCoordinate(mUp, matRot);
            mUp.Normalize();

            if (mUp.Z < 0)
                mUp.Z = 0;

            mForward = Vector3.Cross(mUp, mRight);
            mTarget = Position + mForward;

            UpdateView();
        }

        public void Yaw(float angle)
        {
            var matRot = Matrix.RotationAxis(Vector3.UnitZ, MathUtil.DegreesToRadians(angle));
            mForward = Vector3.TransformCoordinate(mForward, matRot);
            mForward.Normalize();

            mTarget = Position + mForward;
            mUp = Vector3.TransformCoordinate(mUp, matRot);
            mUp.Normalize();

            mRight = Vector3.TransformCoordinate(mRight, matRot);
            mRight.Normalize();

            UpdateView();
        }

        public void Roll(float angle)
        {
            var matRot = Matrix.RotationAxis(mForward, MathUtil.DegreesToRadians(angle));
            mUp = Vector3.TransformCoordinate(mUp, matRot);
            mUp.Normalize();

            mRight = Vector3.TransformCoordinate(mRight, matRot);
            mRight.Normalize();

            UpdateView();
        }
    }

    public class PerspectiveCamera : Camera
    {
        private float mAspect = 1.0f;
        private float mFov = 55.0f;

        public float NearClip { get; private set; }

        public float FarClip { get; private set; }

        public PerspectiveCamera()
        {
            NearClip = 0.2f;
            FarClip = 900.0f;
            UpdateProjection();
        }

        public override void Update()
        {
            base.Update();
            UpdateProjection();
        }

        private void UpdateProjection()
        {
            var matProjection = (LeftHanded == false)
                ? Matrix.PerspectiveFovRH(MathUtil.DegreesToRadians(mFov), mAspect, NearClip, FarClip)
                : Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(mFov), mAspect, NearClip, FarClip);

            OnProjectionChanged(ref matProjection);
        }

        public void SetFarClip(float clip)
        {
            FarClip = clip;
            UpdateProjection();
        }

        public void SetNearClip(float clip)
        {
            NearClip = clip;
            UpdateProjection();
        }

        public void SetClip(float near, float far)
        {
            NearClip = near;
            FarClip = far;
            UpdateProjection();
        }

        public void SetAspect(float aspect)
        {
            mAspect = aspect;
            UpdateProjection();
        }

        public void SetFieldOfView(float fov)
        {
            mFov = fov;
            UpdateProjection();
        }
    }
}
