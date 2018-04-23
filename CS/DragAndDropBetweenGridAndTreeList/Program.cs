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
using System.Windows.Forms;

namespace DragAndDropBetweenGridAndTreeList
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
    }
}
