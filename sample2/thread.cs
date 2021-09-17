using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Forms;


public delegate void deleDraw(ref int num);

namespace sample2
{
   
    public class threadClass
    {
        public Int32 gCount;
        private deleDraw drawMethod;
        public Boolean bContinue;
        private IntPtr hWnd;


        public threadClass(IntPtr hWnd , deleDraw indraw)
        {
            this.hWnd = hWnd;
            this.bContinue = false;
           
            this.gCount = 0;

            drawMethod = indraw;
        }

        [System.Runtime.InteropServices.DllImport("USER32.DLL")]
        static extern int PostMessage(int hwnd, int msg, int wparam, int lparam);


        public void wThread()
        {
            try
            {
                Int32 msgCounter = 100;

                while (bContinue==true)
                {
                    if (msgCounter-- < 0)
                    {
                        PostMessage(hWnd.ToInt32(), 0x8001, 0, 0);
                    }
                    else
                    {
                        PostMessage(hWnd.ToInt32(), 0x8002, 0, msgCounter);
                    }
                    drawMethod(ref gCount);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                MessageBox.Show("thread stop");
            }
        }
    }
}
