using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBFilesViewer.Graphics.Files
{
    public enum BlendMode : int
    {
        Opaque,
        AlphaKey,
        Alpha,
        Add,
        Mod,
        Mod2x,
        ModAdd,
        InvSrcAlphaAdd,
        InvSrcAlphaOpaque,
        SrcAlphaOpaque,
        NoAlphaAdd,
        ConstantAlpha,
        Screen,
        BlendAdd
    }
}
