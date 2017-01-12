using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models.Animations
{
    public class AnimatedTexture
    {
        public MD20 Header { get; }
        private ModelAnimationBlock<Vector3, VectorInterpolator> Translation { get; }
        private ModelAnimationBlock<InvQuaternion16, Quaternion, QuaternionInterpolator> Rotation { get; }
        private ModelAnimationBlock<Vector3, VectorInterpolator> Scaling { get; }

        public Matrix PlacementMatrix { get; private set; }

        public AnimatedTexture(M2 file, MD20 header, ref M2TextureTransform texAnim)
        {
            Header = header;

            Translation = new ModelAnimationBlock<Vector3, VectorInterpolator>(file, header, texAnim.Translation, Vector3.Zero);
            Rotation = new ModelAnimationBlock<InvQuaternion16, Quaternion, QuaternionInterpolator>(file, header, texAnim.Rotation, Quaternion.Identity);
            Scaling = new ModelAnimationBlock<Vector3, VectorInterpolator>(file, header, texAnim.Scaling, Vector3.One);
        }

        public void UpdateMatrix(int animation, uint time)
        {
            var position = Translation.GetValue(animation, time);
            var scaling = Scaling.GetValue(animation, time);
            var rotation = Rotation.GetValue(animation, time);

            PlacementMatrix = Matrix.RotationQuaternion(rotation) * Matrix.Scaling(scaling) * Matrix.Translation(position);
        }
    }
}
