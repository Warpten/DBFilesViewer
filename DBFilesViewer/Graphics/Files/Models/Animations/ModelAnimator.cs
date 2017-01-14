using System;
using DBFilesViewer.Graphics.Scene;
using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models.Animations
{
    public class BillboardParameters
    {
        public Vector3 Forward;
        public Vector3 Right;
        public Vector3 Up;
        public Matrix InverseRotation;
    }

    public sealed class ModelAnimator
    {
        public ModelRenderer ModelRenderer { get; }

        public AnimatedBone[] Bones => ModelRenderer.Model.MD20.Bones;
        public AnimatedTexture[] Textures => ModelRenderer.Model.MD20.TextureTransforms;
        public AnimatedColor[] Colors => ModelRenderer.Model.MD20.Colors;
        public AnimatedTransparency[] Transparencies => ModelRenderer.Model.MD20.Transparencies;

        private int _animationID;
        private AnimationSequence? _animation;

        private int _animationStartTime;
        private bool _animationCompleted;
        private bool _updateRequired;

        public uint Duration => _animation?.Duration ?? 0;

        public ModelAnimator(ModelRenderer renderer)
        {
            ModelRenderer = renderer;
        }

        public bool SetAnimation(uint animationIndex)
        {
            if (animationIndex >= ModelRenderer.Model.MD20.SequenceLookups.Length)
                return false;

            if (ModelRenderer.Model.MD20.SequenceLookups[animationIndex] < 0)
                return false;

            _animationID = ModelRenderer.Model.MD20.SequenceLookups[animationIndex];
            _animation = ModelRenderer.Model.MD20.Sequences[_animationID];

            while ((_animation.Value.Flags & 0x40) != 0)
            {
                // This animation is an alias.
                // Skip to the next animation until we find one that is not aliased.
                _animationID = _animation.Value.NextAlias;
                _animation = ModelRenderer.Model.MD20.Sequences[_animationID];

                //! TODO Make sure this does not go out of scope or loops forever.
            }

            ResetAnimationTimes();
            return true;
        }

        private void ResetAnimationTimes()
        {
            _animationStartTime = Environment.TickCount;
        }

        public void Update(BillboardParameters billboard)
        {
            if (!_animation.HasValue)
                return;

            var now = Environment.TickCount;

            #region Animate bones
            var time = (uint) (now - _animationStartTime);
            if (time >= _animation.Value.Duration &&
                ((_animation.Value.Flags & 0x20) == 0 || _animation.Value.NextAnimation >= 0)) // If looped or next animation exists
            {
                if (_animationCompleted)
                {
                    if (_animation.Value.NextAnimation < 0)
                        return;

                    if (_animation.Value.NextAnimation >= ModelRenderer.Model.MD20.Sequences.Length)
                        return;

                    _animationCompleted = false;
                    _animationStartTime = now;
                    _animationID = _animation.Value.NextAlias;
                    _animation = ModelRenderer.Model.MD20.Sequences[_animationID];
                    return;
                }

                time = _animation.Value.Duration;
                _animationCompleted = true;
            }
            else if (_animation.Value.Duration > 0)
                time %= _animation.Value.Duration;

            foreach (var animationBone in Bones)
                animationBone.UpdateMatrix(time, _animationID, this, billboard);

            #endregion

            #region Animate textures
            time = (uint) (now - _animationStartTime);
            foreach (var textureAnim in Textures)
                textureAnim.UpdateMatrix(0, time);

            foreach (var colorAnim in Colors)
                colorAnim.UpdateValue(0, time);

            foreach (var transparencyAnim in Transparencies)
                transparencyAnim.UpdateValue(0, time);
            #endregion

            _updateRequired = true;
        }

        public bool GetBonesAnimationMatrix(Matrix[] animationMatrices)
        {
            if (!_updateRequired)
                return false;

            for (var i = 0; i < Math.Min(animationMatrices.Length, Bones.Length); ++i)
                animationMatrices[i] = Bones[i].PositionMatrix;

            _updateRequired = false;
            return true;    
        }

        public bool GetTextureAnimationMatrix(int texAnimIndex, ref Matrix uvAnim)
        {
            uvAnim = Matrix.Identity;
            if (texAnimIndex < 0 || texAnimIndex >= Textures.Length)
                return false;

            uvAnim = Textures[texAnimIndex].PlacementMatrix;
            return true;
        }

        public void GetColor(int colorAnimIndex, out Vector4 animatedColor)
        {
            animatedColor = Vector4.One;

            if (colorAnimIndex >= 0 && colorAnimIndex < Colors.Length)
                animatedColor = Colors[colorAnimIndex].Color;
        }

        public void GetTransparency(int alphaAnimIndex, out float animatedColor)
        {
            animatedColor = 1.0f;

            if (alphaAnimIndex >= 0 && alphaAnimIndex < Transparencies.Length)
                animatedColor = Transparencies[alphaAnimIndex].Transparency;
        }
    }
}
