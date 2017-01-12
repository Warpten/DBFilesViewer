using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models.Animations
{
    public class AnimatedColor
    {
        public ModelAnimationBlock<Vector3, VectorInterpolator> Colors { get; }
        public ModelAnimationBlock<short, float, NoInterpolateAlpha16> Alpha { get; }

        public Vector4 Color { get; private set; }

        public AnimatedColor(M2 file, MD20 header, ref M2Color texAnim)
        {
            Colors = new ModelAnimationBlock<Vector3, VectorInterpolator>(file, header, texAnim.Colors, Vector3.Zero);
            Alpha = new ModelAnimationBlock<short, float, NoInterpolateAlpha16>(file, header, texAnim.Alpha, 1.0f);
        }

        public void UpdateValue(int animation, uint time)
        {
            Color = new Vector4(Colors.GetValue(animation, time), Alpha.GetValue(animation, time));
        }
    }

    public class AnimatedTransparency
    {
        public ModelAnimationBlock<short, float, NoInterpolateAlpha16> Alpha { get; }

        public float Transparency { get; private set; }

        public AnimatedTransparency(M2 file, MD20 header, ref M2TextureWeight texAnim)
        {
            Alpha = new ModelAnimationBlock<short, float, NoInterpolateAlpha16>(file, header, texAnim.Weight, 1.0f);
        }

        public void UpdateValue(int animation, uint time)
        {
            Transparency = Alpha.GetValue(animation, time);
        }
    }
}
