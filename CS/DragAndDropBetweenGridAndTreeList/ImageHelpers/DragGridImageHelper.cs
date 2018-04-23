using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.Utils.Drawing;

namespace DragAndDropBetweenGridAndTreeList
{
    public class DragGridImageHelper : GridPainter
    {
        private readonly DevExpress.XtraGrid.Views.Grid.GridView _view;

        public DragGridImageHelper(DevExpress.XtraGrid.Views.Grid.GridView view)
            : base(view)
        {
            _view = view;
        }

        public Cursor GetDragCursor(int rowHandle, Point e)
        {
            GridViewInfo info = _view.GetViewInfo() as GridViewInfo;
            GridRowInfo rowInfo = info.GetGridRowInfo(rowHandle);
            Bitmap result = GetRowDragBitmap(rowHandle);
            Point offset = new Point(rowInfo.Bounds.X, e.Y - rowInfo.Bounds.Y);
            return CursorCreator.CreateCursor(result, offset);
        }

        public Bitmap GetRowDragBitmap(int rowHandle)
        {
            Bitmap bmpView = null;
            Bitmap bmpRow = null;
            GridViewInfo info = _view.GetViewInfo() as GridViewInfo;
            Rectangle totalBounds = info.Bounds;
            GridRowInfo ri = info.GetGridRowInfo(rowHandle);
            Rectangle imageBounds = new Rectangle(new Point(0, 0), ri.Bounds.Size);
            try
            {
                bmpView = new Bitmap(totalBounds.Width, totalBounds.Height);
                using (Graphics gView = Graphics.FromImage(bmpView))
                {
                    using (XtraBufferedGraphics grView = XtraBufferedGraphicsManager.Current.Allocate(gView, new Rectangle(Point.Empty, bmpView.Size)))
                    {
                        Color color = ri.Appearance.BackColor == Color.Transparent ? Color.White : ri.Appearance.BackColor;
                        grView.Graphics.Clear(color);
                        GridViewDrawArgs args = new GridViewDrawArgs(new GraphicsCache(grView.Graphics), info, totalBounds);
                        DrawRow(args, ri);
                        grView.Graphics.FillRectangle(args.Cache.GetSolidBrush(Color.Transparent), ri.Bounds);
                        grView.Render();
                        bmpRow = new Bitmap(ri.Bounds.Width, ri.Bounds.Height);
                        using (Graphics gRow = Graphics.FromImage(bmpRow))
                        {
                            using (XtraBufferedGraphics grRow = XtraBufferedGraphicsManager.Current.Allocate(gRow, new Rectangle(Point.Empty, bmpRow.Size)))
                            {
                                grRow.Graphics.Clear(color);
                                grRow.Graphics.DrawImage(bmpView, imageBounds, ri.Bounds, GraphicsUnit.Pixel);
                                grRow.Render();
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return bmpRow;
        }
    }
}
