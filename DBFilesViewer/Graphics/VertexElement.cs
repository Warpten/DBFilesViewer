﻿using System;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DBFilesViewer.Graphics
{
    public enum DataType
    {
        Float,
        Byte
    }

    /// <summary>
    /// An easy wrapper class for vertex information.
    /// </summary>
    public class VertexElement
    {
        private InputElement mDescription;

        public InputElement Element => mDescription;

        public VertexElement(string semantic, int index, int components, DataType dataType = DataType.Float, bool normalized = false, int slot = 0, bool instanceData = false)
        {
            mDescription = new InputElement
            {
                AlignedByteOffset = InputElement.AppendAligned,
                Classification = instanceData ? InputClassification.PerInstanceData : InputClassification.PerVertexData,
                InstanceDataStepRate = instanceData ? 1 : 0,
                SemanticIndex = index,
                SemanticName = semantic,
                Slot = slot
            };

            if (dataType == DataType.Byte)
            {
                switch (components)
                {
                    case 1:
                        mDescription.Format = normalized ? Format.R8_UNorm : Format.R8_UInt;
                        break;

                    case 2:
                        mDescription.Format = normalized ? Format.R8G8_UNorm : Format.R8G8_UInt;
                        break;

                    case 4:
                        mDescription.Format = normalized ? Format.R8G8B8A8_UNorm : Format.R8G8B8A8_UInt;
                        break;

                    default:
                        throw new ArgumentException("Invalid combination of data type and component count");
                }
            }
            else
            {
                switch (components)
                {
                    case 1:
                        mDescription.Format = Format.R32_Float;
                        break;

                    case 2:
                        mDescription.Format = Format.R32G32_Float;
                        break;

                    case 3:
                        mDescription.Format = Format.R32G32B32_Float;
                        break;

                    case 4:
                        mDescription.Format = Format.R32G32B32A32_Float;
                        break;

                    default:
                        throw new ArgumentException("Invalid combination of data type and component count");
                }
            }
        }
    }
}
