using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models.Animations
{
    public sealed class AnimatedBone
    {
        public MD20 Header { get; }

#if DEBUG
        public Vector3 Position { get; private set; }
#endif
        public Matrix PositionMatrix { get; private set; }

        public AnimatedBone(M2 file, MD20 header, ref M2Bone bone, int boneIndex)
        {
            Header = header;

            ParentBoneID = bone.ParentBone;
            IsBillboarded = (bone.Flags & 0x08) != 0;  // Some billboards have 0x40 for cylindrical?
            IsTransformed = (bone.Flags & 0x200) != 0;

            _pivot = Matrix.Translation(bone.Pivot);
            _invPivot = Matrix.Translation(-bone.Pivot);

            Translation = new ModelAnimationBlock<Vector3, VectorInterpolator>(file, header, bone.Translation, Vector3.Zero);
            Rotation = new ModelAnimationBlock<Quaternion16, Quaternion, QuaternionInterpolator>(file, header, bone.Rotation, Quaternion.Identity);
            Scaling = new ModelAnimationBlock<Vector3, VectorInterpolator>(file, header, bone.Scaling, Vector3.One);
        }

        public void UpdateMatrix(uint time, int animation, ModelAnimator animator, BillboardParameters billboard, bool applyPivot = true)
        {
            PositionMatrix = Matrix.Identity;

            if (IsBillboarded && billboard != null)
            {
                var billboardMatrix = Matrix.Identity;
                billboardMatrix.Row1 = new Vector4(billboard.Forward, 0);
                billboardMatrix.Row2 = new Vector4(billboard.Right, 0);
                billboardMatrix.Row3 = new Vector4(billboard.Up, 0);
                PositionMatrix = billboardMatrix * billboard.InverseRotation;
            }

            if (IsTransformed)
            {
                var translation = Matrix.Translation(Translation.GetValue(animation, time, animator.Duration));
                var scaling = Matrix.Scaling(Scaling.GetValue(animation, time, animator.Duration));
                var rotation = Matrix.RotationQuaternion(Rotation.GetValue(animation, time, animator.Duration));

                PositionMatrix *= rotation * scaling * translation;
            }

            if (applyPivot)
                PositionMatrix = _invPivot * PositionMatrix * _pivot;

            if (ParentBoneID >= 0)
            {
                Header.Bones[ParentBoneID].UpdateMatrix(time, animation, animator, billboard);
                PositionMatrix *= Header.Bones[ParentBoneID].PositionMatrix;
            }
#if DEBUG
            Position = new Vector3(PositionMatrix.M41, PositionMatrix.M42, PositionMatrix.M43);
#endif
        }

        public bool IsBillboarded { get; }
        public bool IsTransformed { get; }

        public ModelAnimationBlock<Quaternion16, Quaternion, QuaternionInterpolator> Rotation { get; }
        public ModelAnimationBlock<Vector3, VectorInterpolator> Scaling { get; }
        public ModelAnimationBlock<Vector3, VectorInterpolator> Translation { get; }

        private Matrix _invPivot;
        private Matrix _pivot;

        public int ParentBoneID { get; }
    }
}
