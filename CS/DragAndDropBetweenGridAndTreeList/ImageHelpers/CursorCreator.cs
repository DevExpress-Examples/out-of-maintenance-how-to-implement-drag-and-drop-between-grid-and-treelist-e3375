// Developer Express Code Central Example:
// How to implement drag-and-drop between Grid and TreeList
// 
// This example demonstrates how to create helper classes for implementing
// drag-and-drop between Grid and TreeList. For more information please refer
// to:
// http://www.devexpress.com/scid=A2343
// http://www.devexpress.com/scid=K18056
// http://www.devexpress.com/scid=A1444
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E3375

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;

namespace DragAndDropBetweenGridAndTreeList
{
    class CursorCreator
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);

        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }

        public static Cursor CreateCursor(Bitmap bmp, Point hotspot)
        {
            if (bmp == null)
                return Cursors.Default;
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.fIcon = false;
            tmp.xHotspot = hotspot.X;
            tmp.yHotspot = hotspot.Y;
            ptr = CreateIconIndirect(ref tmp);
            return new Cursor(ptr);
        }
    }
}
