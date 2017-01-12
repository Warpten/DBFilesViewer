using DBFilesViewer.Graphics.Files.Models.Animations;
using DBFilesViewer.Graphics.Scene;
using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models.Attachments
{
    public sealed class ModelAttachment
    {
        public ModelRenderer ParentModel { get; }
        public ModelRenderer ChildModel { get; }

        public Matrix PlacementMatrix { get; }

        public AnimatedBone AttachmentBone { get; }

        public ModelAttachment(ModelRenderer parentFile, ModelRenderer childFile, M2Attachment attachment)
        {
            ParentModel = parentFile;
            ChildModel = childFile;

            AttachmentBone = ParentModel.Model.MD20.Bones[attachment.Bone];

            PlacementMatrix = Matrix.Translation(attachment.Position);
        }

        public void Render(BillboardParameters parameters)
        {
            // Bone should have already updated
            var placementMatrix = PlacementMatrix * AttachmentBone.PositionMatrix;

            ChildModel.UpdatePlacementMatrix(placementMatrix);
            ChildModel.Render(parameters);
        }
    }
}
