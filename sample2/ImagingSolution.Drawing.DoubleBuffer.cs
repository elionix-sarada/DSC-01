using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImagingSolution
{
    namespace Drawing
    {
        class DoubleBuffer : IDisposable
        {
            BufferedGraphics myBuffer;

            Control _Control;

            public DoubleBuffer(Control control)
            {
                _Control = control;

                this.Dispose();

                // This example assumes the existence of a form called control.
                System.Drawing.BufferedGraphicsContext currentContext;

                // Gets a reference to the current BufferedGraphicsContext
                currentContext = BufferedGraphicsManager.Current;
                // Creates a BufferedGraphics instance associated with control, and with 
                // dimensions the same size as the drawing surface of control.
                myBuffer = currentContext.Allocate(control.CreateGraphics(),
                   control.DisplayRectangle);

                _Control.Paint += new System.Windows.Forms.PaintEventHandler(this.Paint);

            }

            ~DoubleBuffer()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (myBuffer != null)
                {
                    myBuffer.Dispose();
                    myBuffer = null;
                }
                _Control.Paint -= new System.Windows.Forms.PaintEventHandler(this.Paint);
            }

            private void Paint(object sender, PaintEventArgs e)
            {
                Refresh();
            }

            public void Refresh()
            {
                if (myBuffer != null)
                    myBuffer.Render();
            }

            public Graphics Graphics
            {
                get { return myBuffer.Graphics; }
            }
        }
    }
}
