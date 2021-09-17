using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace sample2
{
    public class CompClass
    {
        public Complex[] x;
        private Complex[] w_tbl;
        private int[] b_tbl;
        private double f;
        public int n_abs;
       
 
        public void init_FFT(int n)
        {
            int n_half;
            n_abs = n;

            x     = new Complex[n];
            w_tbl = new Complex[n];
            b_tbl = new int[n];
            f = -6.283185307179586 / (double)n;
            Complex ff = new Complex(0, f);
            Complex arg = new Complex(ff.Real, ff.Imaginary);

            for (int j = 0; j < n / 2; j++)
            {
                Complex dj = new Complex((double)j, 0);
                Complex m = Complex.Multiply(arg, dj);

                w_tbl[j] = Complex.Exp(m);
            }
            n_half = n / 2;
            b_tbl[0] = 0;

            for (int ne = 1; ne < n; ne = ne << 1)
            {
                for (int jp = 0; jp < ne; jp++)
                    b_tbl[jp + ne] = b_tbl[jp] + n_half;

                n_half = n_half / 2;
            }
        }
        public void set_data(double[] xin)
        {
            for(int j=0; j < n_abs; j++)
            {
               x[j] = new Complex(xin[j],0);
            }
        }
        public void execute()
        {
            int j, jnh, jp, jx, ne, n_half, n_half2;

            n_half = n_abs/2;

            for (ne = 1; ne < n_abs; ne = ne * 2)
            {
                n_half2 = n_half * 2;

                for (jp = 0; jp < n_abs; jp = jp + n_half2)
                {
                    jx = 0;
                    for (j = jp; j < jp + n_half; j++)
                    {
                        jnh = j + n_half;
                        Complex xtemp = new Complex(x[j].Real, x[j].Imaginary);

                        x[j] = Complex.Add(xtemp, x[jnh]);
                        x[jnh] = Complex.Multiply(Complex.Subtract(xtemp, x[jnh]), w_tbl[jx]);
                        jx = jx + ne;
                    }
                }
                n_half = n_half / 2;
            }
            for (j = 0; j < n_abs; j++)
            {
                if (j < b_tbl[j])
                {
                    Complex a;
                    a = x[j];
                    x[j] = x[b_tbl[j]];
                    x[b_tbl[j]] = a;
                }
            }
        }
        public void GetAbsNorm( double[] xout)
        {
            for(int j=0;j<n_abs;j++)
                xout[j] = Complex.Abs(x[j]);
        }
        public void GetArg(double[] xout)
        {
            for (int j = 0; j < n_abs; j++)
                xout[j] = Math.Atan2(x[j].Real, x[j].Imaginary);
        }
        public void GetMax()
        {
            double xmax = 0.0;

            for (int j = 0; j < n_abs / 2; j++)
            {
                xmax = xmax > Complex.Abs(x[j]) ? xmax : Complex.Abs(x[j]);
            }
        }
       
    }






}
