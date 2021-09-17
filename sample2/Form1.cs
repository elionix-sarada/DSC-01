using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InterfaceCorpDllWrap;
using System.Threading;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;


namespace sample2
{
    public partial class Form1 : Form
    {

        private AnalogCon.AD_DA ana;

        private ColumnHeader column_No;
        private ColumnHeader column_FILENAME;

        public int lines;
        public int vertex;
        public int wait_time;
        public int ioffset, ioffset2, iSamplingNum;
        public uint disp_range;
        public int m_cal=0;



        public int Count;
        decimal pre_NUMERIC;
        SaveFileDialog sfd;
        string[] file_name;

        private ImagingSolution.Drawing.DoubleBuffer db1, db2, db3, db4, db5, db6;
        //public int Width;

        public Object pAd, pAd2;			// Area to store sampled data

        double start_raser, start_raser2;
        double pre_raser, pre_raser2;
        double f_max, f_max2;
        int fg, fg2;
        public string f_current;

        double[] xn = null;
        double[] wn = null;
        double[] ynr = null;
        double[] ynw = null;
        double[] iso = null;

        double[] xn2 = null;
        double[] wn2 = null;
        double[] ynr2 = null;
        double[] ynw2 = null;
        double[] iso2 = null;
        double[] xx = null;

        double[] henni_range_haba = new double[3];
        double saisho_a, saisho_b;

        double A, B;

        private System.Threading.Thread thread;

        int f_maxnum, f_maxnum2;
        //double f_max;
        double P1, P2;
        double P1_2, P2_2;
        double pFG = 0;
        double pFG2 = 0;
        int S_CT;


        double[] henni = null;
        double[] pvolt = null;
        double[] PH = null;
        double[,] henni2 = null;
        double[] k_henni = null;
        double[] k_henni2 = null;
        double[] pvolt2 = null;
        double[] PH2 = null;

        double PHA0 = 0;
        double PHA0_2 = 0;

        double r_henni, r_henni2;
        int pcount;
        int iso_offset;

        LinearGradientBrush gb;

        double XMAX, YMAX, CX, CY;

        Boolean kousei;
        Boolean PIEZO;
        Boolean bZero;
        Boolean GraphAll;
        Boolean TAB2;
        Boolean EmSTOP;


        int pi_count;
        int sign;
        int iso_count;

        ushort gmin, gmin2;
        int nData;
        int gCount;

        CompClass CP = new CompClass();
        CompClass CP2 = new CompClass();


        public const int default_lines = 4;
        public const int default_vertex = 4;
        public const int default_width = 6;
        public const int default_height = 480;
        public const int default_wait = 0;

        private threadClass objThread;
        private threadClass objThread2;
        private Thread th1, th2;
        public int sokutei_num = 3;


        Graphics g1, pg;
        Bitmap canvas, canvas2, canvas3, canvas4, canvas5, canvas6;

        const string parameter_file = "parameter.xml";

        double ve;
        double R1, R2, Ra, Rb, R3, R4, RL;
        double vc;
        double vb, V3;
        double V1, V2, Vin;
        double Ic;
        int fs = 1800;
        int size = 1800;
        int Success_Num;


        public Form1()
        {
            InitializeComponent();
            ReadParameter();

           
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //this.WindowState = FormWindowState.Maximized;

            this.Width = 1250;
            this.Height = 430;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            db1 = new ImagingSolution.Drawing.DoubleBuffer(pictureBox1);
           // db2 = new ImagingSolution.Drawing.DoubleBuffer(pictureBox5);

            ana = new AnalogCon.AD_DA();

           
            //canvas = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            //g = Graphics.FromImage(canvas);

            g1 = db1.Graphics;
          //  g2 = db2.Graphics;

            Pen p = Pens.SkyBlue;

            //this.Size = new Size(1600, 1000);//330);


            ana.init_dac();
            ana.init_adc();
            ana.init_adc2();


            kousei = false;
            PIEZO = false;
            GraphAll = false;

            sign = 1;
            r_henni = 0;
            pcount = 0;
            iso_count = 0;

            XMAX = 1000; //[nm]
            YMAX = 1;//[V]

            EmSTOP = false;
            TAB2 = false;
            iso_offset = 0;
            nData = 1024;

            //      init_adc2();

            column_No = new ColumnHeader();
            column_FILENAME = new ColumnHeader();

            column_No.Text = "No";
            column_No.Width = 50;
            column_FILENAME.Text = "ファイル名";
            column_FILENAME.Width = 250;

            ColumnHeader[] colHeaderRegValue = { this.column_No, this.column_FILENAME };

            sfd = new SaveFileDialog();

            sfd.FileName = "新しいファイル.csv";
            sfd.InitialDirectory = @"C:\";
            f_current = @"C:\";

            sfd.Filter =
                "csvファイル(*.csv)|*.csv;*.csv|csv(*.csv)|*.csv";
            //[ファイルの種類]ではじめに
            //「すべてのファイル」が選択されているようにする
            sfd.FilterIndex = 2;
            //タイトルを設定する
            sfd.Title = "保存先のファイルを選択してください";
            //ダイアログボックスを閉じる前に現在のディレクトリを復元するようにする
            sfd.RestoreDirectory = false;
            //既に存在するファイル名を指定したとき警告する
            //デフォルトでTrueなので指定する必要はない
            sfd.OverwritePrompt = true;
            //存在しないパスが指定されたとき警告を表示する
            //デフォルトでTrueなので指定する必要はない
            sfd.CheckPathExists = true;
            sfd.AddExtension = true;


            Count = 0;
            //draw_grid(g, p);

            //g.Dispose();

            //PictureBox1に表示する
            //pictureBox1.Image = canvas;
            file_name = new string[300];


            pre_NUMERIC = 10;
            //this.Refresh();
            //GC.Collect();
            // DispRange_Combo.Items.Add("Aレンジ");
            // DispRange_Combo.Items.Add("Bレンジ");
            // DispRange_Combo.Items.Add("Cレンジ");
            // DispRange_Combo.SelectedIndex = 0;

            disp_range = 2;
            ana.adOutDO(ana.hDevice2, 0x02);

            henni_range_haba[0] = 4;
            henni_range_haba[2] = 20;
            henni_range_haba[1] = 100;


            toolStripStatusLabel1.Text = string.Format("ステータス:" + "待機中");
            tb_sokutei.Text = sokutei_num.ToString("d");
        }


        private void draw_grid(Graphics gg, Pen p)
        {
            int w = 1024;

            gg.Clear(pictureBox1.BackColor);

            for (int i = 0; i < w; i += 50)
            {
                gg.DrawLine(p, i, 0, i, 250);
            }
            gg.DrawLine(p, 0, 210, w, 210);

        }

        private void draw_grid2()
        {
            int TIME = 750;
            int VOL = 50;

            Pen Pen1 = new Pen(Brushes.DeepSkyBlue);

            // Set the pen's width.
            Pen1.Width = Width;

            // Set the LineJoin property.
            Pen1.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;

            pg.DrawLine(Pen1, 0, 0, TIME, 0);
            pg.DrawLine(Pen1, TIME, 0, TIME, VOL * 2);
            pg.DrawLine(Pen1, TIME, VOL * 2, 0, VOL * 2);
            pg.DrawLine(Pen1, 0, VOL * 2, 0, 0);

            //int dys = 20;
            //int gy1 = 20;

            for (int j = 0; j < 4; j++)
            {
                for (double fr = 1 * Math.Pow(10, j); fr <= 10.0 * Math.Pow(10, j); fr += (1.0 * Math.Pow(10, j)))
                {
                    double l1 = Math.Log10(fr);
                    int lo = (int)(l1 * 250);

                    if (fr <= 5000)
                    {
                        pg.DrawLine(Pen1, lo, 0, lo, 100);

                    }
                }
            }
            //Dispose of the pen.
            Pen1.Dispose();
        }
        private void ReadParameter()
        {
            try
            {
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(parameter_file);

                System.Xml.XmlElement root = doc.DocumentElement;

                this.Width = Int32.Parse(root.GetElementsByTagName("Width").Item(0).InnerText);
                this.Height = Int32.Parse(root.GetElementsByTagName("Height").Item(0).InnerText);
                this.lines = Int32.Parse(root.GetElementsByTagName("Lines").Item(0).InnerText);
                this.vertex = Int32.Parse(root.GetElementsByTagName("Vertex").Item(0).InnerText);
                this.wait_time = Int32.Parse(root.GetElementsByTagName("WaitTime").Item(0).InnerText);
            }
            catch (Exception)
            {
                this.Width = default_width;
                this.Height = default_height;
                this.lines = default_lines;
                this.vertex = default_vertex;
                this.wait_time = default_wait;
            }
        }//ReadParameter


        public void SignalGen()
        {
            for (int n = 0; n < nData; n++)
                xn[n] = (double)(((ushort[])ana.pAd)[n] - gmin) * 0.005;

            for (int n = 0; n < nData; n++)
                xn2[n] = (double)(((ushort[])ana.pAd2)[n] - gmin2) * 0.005;
        }
        public void FindMaxh()
        {
            int m = 0;
            double max;

            max = -1000;

            for (int j = 0; j <= nData / 2; j++)
            {
                if (ynr[j] > max && j > 3)
                {
                    max = ynr[j];
                    m = j;
                }
            }
            f_maxnum = m;

            max = -1000;

            for (int j = 0; j <= nData / 2; j++)
            {
                if (ynr2[j] > max && j > 3)
                {
                    max = ynr2[j];
                    m = j;
                }
            }
            f_maxnum2 = m;

        }

        public void Spectrum(bool w_yn, double[] m, double[] p, int inout)
        {
            double[] xw = new double[1024];

            if (w_yn)
            {
                for (int n = 0; n < nData; n++)
                    xw[n] = xn[n] * wn[n];

                CP.set_data(xw);
                CP.execute();
                CP.GetAbsNorm(m);
                CP.GetArg(p);
            }
            else
            {
                CP.set_data(xn);
                CP.execute();
                if (inout == 1) CP.GetMax();
                CP.GetAbsNorm(m);
                CP.GetArg(p);
            }
        }
        public void Spectrum2(bool w_yn, double[] m, double[] p, int inout)
        {
            double[] xw = new double[1024];

            if (w_yn)
            {
                for (int n = 0; n < nData; n++)
                    xw[n] = xn2[n] * wn2[n];

                CP2.set_data(xw);
                CP2.execute();
                CP2.GetAbsNorm(m);
                CP2.GetArg(p);
            }
            else
            {
                CP2.set_data(xn2);
                CP2.execute();
                if (inout == 1) CP2.GetMax();
                CP2.GetAbsNorm(m);
                CP2.GetArg(p);
            }
        }
        public void draw_fringe1(int num, Graphics g1)
        {
            ushort da, pre;
            int cy = 210;
            pre = 0;

            for (int i = 0; i < 1024; i++)
            {
                da = ((ushort[])ana.pAd)[i];

                if (num < 5)
                {
                    if (da < gmin) gmin = da;
                }
                da -= gmin;

                da /= 40;

                if (i != 0)
                    g1.DrawLine(Pens.Red, i, cy - pre, i + 1, cy - da);

                pre = da;
            }
            pre = 0;
        }
        public void draw_fringe2(int num, Graphics g2)
        {
            ushort da, pre;
            int cy = 210;
            pre = 0;

            for (int i = 0; i < 1024; i++)
            {
                da = ((ushort[])ana.pAd2)[i];

                if (num < 5)
                {
                    if (da < gmin2) gmin2 = da;
                }
                da -= gmin2;

                da /= 50;

                if (i != 0)
                    g2.DrawLine(Pens.Blue, i, cy - pre, i + 1, cy - da);

                pre = da;
            }
        }
        public void draw_graph3()
        {
            double sc = 45;

            Pen P1 = new Pen(System.Drawing.Color.Blue, 1);
            Pen P3 = new Pen(System.Drawing.Color.SkyBlue, 1);
            Pen P4 = new Pen(System.Drawing.Color.Red, 1);

          

            Point[] pts = new Point[iso_count + 1];
            Point[] pts2 = new Point[iso_count + 1];


            iso_offset -= 4;

            for (int i = 0; i < iso_count; i++)
                pts[i] = new Point(300 + i * 4 + iso_offset, 150 - (int)(iso[i] * sc));
            for (int i = 0; i < iso_count; i++)
                pts2[i] = new Point(300 + i * 4 + iso_offset, 150 - (int)(iso2[i] * sc));

        
         

            P1.Dispose();
            P3.Dispose();

        }

        public void draw_graph2()
        {
            //g3.FillRectangle(gb, g3.VisibleClipBounds);
/*
            CX = 600 / XMAX;
            CY = 300 / YMAX;


            Pen P1 = new Pen(System.Drawing.Color.Blue, 2);
            Pen P2 = new Pen(System.Drawing.Color.Black, 1);
            Pen P3 = new Pen(System.Drawing.Color.SkyBlue, 1);
            Pen P4 = new Pen(System.Drawing.Color.Red, 2);

            if (henni[pcount] * CX >= 600)
            {
                XMAX += 1000;
                CX = 600 / XMAX;
            }
            if (pvolt[pcount] * CY >= 300)
            {
                YMAX += 1;
                CY = 300 / YMAX;
            }
            //int x2 = 0;


            int X0, Y0;
          //  X0 = pictureBox2.Location.X - pictureBox3.Location.X - 1;
           // Y0 = pictureBox2.Location.Y - pictureBox3.Location.Y + pictureBox2.Height - 1;


            Font font1 = new Font("MS UI Gothic", 8);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            Font font2 = new Font("MS UI Gothic", 11);
            SolidBrush drawBrush2 = new SolidBrush(Color.Black);

            string drawString;

            double dx = 200;
            double dy = 0.2;

            double kaix = XMAX / dx;
            double kaiy = YMAX / dy;


            double DX = 600 / ((XMAX) / dx);
            double DY = 300 / ((YMAX) / dy);

            if (kaix > 20)
            {
                dx = 2000;
                kaix = XMAX / dx;
                DX = 600 / ((XMAX) / dx);

            }

            if (kaiy > 20)
            {
                dy = 1;
                kaiy = YMAX / dy;
                DY = 300 / ((YMAX) / dy);
            }

            P3.DashStyle = DashStyle.Dot;

            for (int j = 0; j <= kaix; j++)
            {
                g3.DrawLine(P2, (int)(X0 + DX * j), Y0, (int)(X0 + DX * j), Y0 + 5);
                drawString = ((dx * j) / 1000).ToString("F1");
                g3.DrawString(drawString, font1, drawBrush, (int)(X0 + DX * j - 8), Y0 + 10);
                g2.DrawLine(P3, (int)(DX * j - 1), 0, (int)(DX * j - 1), 300);

            }
            for (int j = 0; j <= kaiy; j++)
            {
                g3.DrawLine(P2, X0, (int)(Y0 - DY * j - 1), X0 - 5, (int)(Y0 - DY * j - 1));
                drawString = (dy * j).ToString("F1");
                g3.DrawString(drawString, font1, drawBrush, X0 - 30, (int)(Y0 - DY * j - 5));
                g2.DrawLine(P3, 0, (int)(300 - DY * j - 1), 600, (int)(300 - DY * j - 1));

            }
            drawString = "変位[um]";
            g3.DrawString(drawString, font2, drawBrush, X0 + 250, Y0 + 30);
            //////////////////
            string s = "電圧[V]";
            //大きさを調べるためのダミーのBitmapオブジェクトの作成
            Bitmap img0 = new Bitmap(1, 1);
            //imgのGraphicsオブジェクトを取得
            Graphics bg0 = Graphics.FromImage(img0);
            //使用するFontオブジェクトを作成
            Font fnt = new Font("MS UI Gothic", 11);
            //文字列を描画したときの大きさを計算する
            int w = (int)bg0.MeasureString(s, fnt).Width;
            int h = (int)fnt.GetHeight(bg0);
            //Bitmapオブジェクトを作り直す
            Bitmap img = new Bitmap(w, h);
            //imgに文字列を描画する
            Graphics bg = Graphics.FromImage(img);
            bg.DrawString(s, fnt, Brushes.Black, 0, 0);

            //90度で回転するための座標を計算
            //ラジアン単位に変換
            double d = -90 / (180 / Math.PI);
            //新しい座標位置を計算する
            float xx = 2F;
            float yy = 200F;
            float xx1 = xx + img.Width * (float)Math.Cos(d);
            float yy1 = yy + img.Width * (float)Math.Sin(d);
            float xx2 = xx - img.Height * (float)Math.Sin(d);
            float yy2 = yy + img.Height * (float)Math.Cos(d);
            //PointF配列を作成
            PointF[] destinationPoints = {new PointF(xx, yy),
                                    new PointF(xx1, yy1),
                                    new PointF(xx2, yy2)};

            //画像を描画
            g3.DrawImage(img, destinationPoints);

            //リソースを解放する
            fnt.Dispose();
            bg0.Dispose();
            img0.Dispose();
            img.Dispose();
            bg.Dispose();

            //////////////////
            font1.Dispose();
            font2.Dispose();
            drawBrush.Dispose();
            drawBrush2.Dispose();

            for (int i = 0; i < pcount; i++)
            {

                int px = (int)(CX * henni[i]);
                int py = 300 - (int)(CY * pvolt[i]);
                //int px2 = (int)(CX * henni2[i]);

                g2.DrawEllipse(P1, px, py, 1, 1);
                //g2.DrawEllipse(P4, px2, py, 1, 1);
            }

            P1.Dispose();
            P2.Dispose();
            P4.Dispose();

            */
        }
        void gauss(double[,] a)
        {
            int j, k, l, pivot;
            double[] x = new double[3];
            double p, q, m;
            double[,] b = new double[1, 4];


            int N = 3;


            for (int i = 0; i < N; i++)
            {
                m = 0;
                pivot = i;

                for (l = i; l < N; l++)
                {
                    if (Math.Abs(a[l, i]) > m)
                    {   //i列の中で一番値が大きい行を選ぶ
                        m = Math.Abs(a[l, i]);
                        pivot = l;
                    }
                }

                if (pivot != i)
                {                          //pivotがiと違えば、行の入れ替え
                    for (j = 0; j < N + 1; j++)
                    {
                        b[0, j] = a[i, j];
                        a[i, j] = a[pivot, j];
                        a[pivot, j] = b[0, j];
                    }
                }
            }
            for (k = 0; k < N; k++)
            {
                p = a[k, k];              //対角要素を保存
                a[k, k] = 1;              //対角要素は１になることがわかっているから

                for (j = k + 1; j < N + 1; j++)
                {
                    a[k, j] /= p;
                }
                for (int i = k + 1; i < N; i++)
                {
                    q = a[i, k];
                    for (j = k + 1; j < N + 1; j++)
                    {
                        a[i, j] -= q * a[k, j];
                    }
                    a[i, k] = 0;              //０となることがわかっているところ
                }
            }
            //解の計算
            for (int i = N - 1; i >= 0; i--)
            {
                x[i] = a[i, N];
                for (j = N - 1; j > i; j--)
                {
                    x[i] -= a[i, j] * x[j];
                }
            }
            for (int i = 0; i < N; i++)
            {
                xx[i] = x[i];
            }
        }

        void sai2(int ct, double[] x, double[] y)
        {
            int i, j, k;

            int n = 3;
            int S = ct;

            double[,] A = new double[3, 4];



            //初期化
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n + 1; j++)
                {
                    A[i, j] = 0.0;
                }
            }
            //ガウスの消去法で解く行列の作成
            for (i = 0; i < n; i++)
            {
                for (j = 0; j < n; j++)
                {
                    for (k = 0; k < S; k++)
                    {
                        A[i, j] += Math.Pow(x[k], i + j);
                    }
                }
            }
            for (i = 0; i < n; i++)
            {
                for (k = 0; k < S; k++)
                {
                    A[i, n] += Math.Pow(x[k], i) * y[k];
                }
            }
            gauss(A);

        }
        public void sai12(int num, double[] x, double[] y)
        {
            double a = 0;
            double b = 0;
            double sum_xy = 0;
            double sum_x = 0;
            double sum_y = 0;
            double sum_x2 = 0;

            for (int i = 0; i < num; i++)
            {
                sum_xy += x[i] * y[i];
                sum_x += x[i];
                sum_y += y[i];
                sum_x2 += System.Math.Pow(x[i], 2);
            }
            a = (num * sum_xy - sum_x * sum_y) / (num * sum_x2 - System.Math.Pow(sum_x, 2));
            b = (sum_x2 * sum_y - sum_xy * sum_x) / (num * sum_x2 - System.Math.Pow(sum_x, 2));

            saisho_a = a;
            saisho_b = b;

        }
        public int sai13(int st,double[] h)
        {
	        int i;
	        double A00,A01,A02,A11,A12,A,B;

	        A00=A01=A02=A11=A12=0.0;
	
	        for (i=st;i<100+st;i++) {
		        A00+=1.0;
		        A01+=(float)(i+1);
		        A02+=h[i];
		        A11+=(float)(i+1)*(float)(i+1);
                A12 += (h[i]) * (i + 1);
	        }
            /*１次式の係数の計算*/
	        A=(A02*A11-A01*A12)/(A00*A11-A01*A01);
	        B=(A00*A12-A01*A02)/(A00*A11-A01*A01);

	        double p,q;
            p = A % B;
	        //p = modf(A/B,&q);
	        int n=(int)(A/B);

            if (System.Math.Abs(p) > 0.5) n = System.Math.Abs(n) + 1;
            else n = System.Math.Abs(n);

	        return (n-1);
	    }       
        public void sai1(double[] x, double[] y)
        {
            double A00, A01, A02, A11, A12;

            A00 = A01 = A02 = A11 = A12 = 0.0;

            for (int i = 0; i < S_CT - 10; i++)
            {
                A00 += 1.0;
                A01 += x[i];
                A02 += y[i];
                A11 += x[i] * x[i];
                A12 += x[i] * y[i];
            }
            /*１次式の係数の計算*/
            A = (A02 * A11 - A01 * A12) / (A00 * A11 - A01 * A01);
            B = (A00 * A12 - A01 * A02) / (A00 * A11 - A01 * A01);

            /*   double p,q;
               p = modf(A/B,&q);
               int n=(int)(A/B);
               if(fabs(p)>0.5)n=abs(n)+1;
               else n=abs(n);

               return (n-1);
               */
        }
        public void draw_text(Graphics b)
        {
            /*
            int dx = pictureBox4.Left - pictureBox3.Left;
            int dy = pictureBox4.Top - pictureBox3.Top - 1;

            Pen p = new Pen(Color.Black, 1);

            b.DrawLine(p, dx, dy, dx - 10, dy);

            if (numericUpDown1.Value == 10)
            {
                //フォントオブジェクトの作成
                Font fnt = new Font("MS UI Gothic", 20);
                //文字列を位置(0,0)、青色で表示
                b.DrawString("これはテストです。", fnt, Brushes.Blue, dx - 10, dy);
            }
            p.Dispose();
            */
        }
        public void draw_graph4(Graphics b3)
        {
            int prex, prey, px, py;
            double zansa, kaiki;
            int k;
            double divx = 1000 / (iSamplingNum);

            prex = 0;
            prey = 0;

            Pen P1 = new Pen(System.Drawing.Color.Blue, 1);
            Pen P2 = new Pen(System.Drawing.Color.Black, 1);
            Pen P3 = new Pen(System.Drawing.Color.Gray, 1);
            Pen P4 = new Pen(System.Drawing.Color.Red, 1);

            Pen Pen1 = new Pen(Brushes.DeepSkyBlue);

            // Set the pen's width.
            Pen1.Width = 1;

            // Set the LineJoin property.
            //Pen1.LineJoin = System.Drawing.Drawing2D.LineJoin.Bevel;
            Pen1.DashStyle = DashStyle.Dash;
            Pen p = Pens.SkyBlue;
            k = 0;
            for (int i = 10; i < S_CT; i++)
            {
                pvolt[k] = i;
                k_henni[k] = henni[i];
                k++;
            }
            sai1(pvolt, k_henni);
            //xx = new double[4];

            //sai2(S_CT, pvolt, k_henni);

            //b3.DrawLine(Pen1, 0, 150, 1000, 150);
            for (int i = 0; i < 10; i++)
            {
                b3.DrawLine(p, 0, 300 - i * 30, 1000, 300 - i * 30);
            }
            for (int i = 0; i < S_CT - 10; i++)
            {
                px = (int)(i * divx);//(int)(CX * henni[i]);
                kaiki = A + B * pvolt[i];
                // kaiki = xx[0] + xx[1] * pvolt[i] + xx[2] * pvolt[i] * pvolt[i];


                zansa = k_henni[i] - kaiki;
                py = 150 + (int)(zansa * 125);
                if (i >= 1)
                {
                    b3.DrawLine(P4, prex, prey, px, py);
                }
                b3.DrawEllipse(P4, px - 2, py - 2, 4, 4);
                prex = px;
                prey = py;
            }
            k = 0;
            for (int i = 10; i < S_CT; i++)
            {
               // k_henni2[k] = henni2[i];
                k++;

            }
            sai1(pvolt, k_henni2);

            for (int i = 0; i < S_CT - 10; i++)
            {
                px = (int)(i * divx);//(int)(CX * henni[i]);
                kaiki = A + B * pvolt[i];
                //kaiki = xx[0] + xx[1] * pvolt[i] + xx[2] * pvolt[i] * pvolt[i] + xx[3] * pvolt[i] * pvolt[i] * pvolt[i];

                zansa = k_henni2[i] - kaiki;
                py = 150 + (int)(zansa * 125);

                if (i >= 1)
                {
                    b3.DrawLine(P1, prex, prey, px, py);

                }
                b3.DrawEllipse(P1, px - 2, py - 2, 4, 4);
                prex = px;
                prey = py;
            }

            // g6.DrawLine(P2, 0, 65, 600, 65);
            // g6.DrawLine(P3, 0, 35, 600, 35);
            // g6.DrawLine(P3, 0, 95, 600, 95);

            /*     pcount3 = pcount - 100;

                 sai1(henni, henni2);
                 for (int i = 0; i < pcount3; i++)
                 {
                     px = i;//(int)(CX * henni[i]);
                     kaiki = A + B * henni[i];

                     zansa = henni2[i] - kaiki;
                     py = 65 - (int)(zansa * 3);
                     if (i != 0)
                     {
                         //     g6.DrawLine(P4, prex, prey, px, py);
                     }

                     prex = px;
                     prey = py;
                 }
                 */
            string s;
            Font font = new Font("MS UI Gothic", 12);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            // s = "計算結果";
            // g2.DrawString(s, font, drawBrush, 100, 50);

            s = "計算結果" + B.ToString("F4");
            // g6.DrawString(s, font, drawBrush, 100, 10);


            P1.Dispose();
            P4.Dispose();
            P3.Dispose();
            P2.Dispose();
            font.Dispose();
            drawBrush.Dispose();

            //	MessageBox.Show(this, A.ToString()+"+"+B.ToString());


        }
        public void fft_cal(int CT)
        {
            if (CT != 0)
            {
                double d = ynw[f_maxnum] - PHA0;

                if (Math.Abs(d) >= 5.0)
                {
                    if (d >= 4.0) //(d >= 5.0)
                    {
                        pFG -= 2;
                    }
                    else if (d <= -4.0)//(d <= -5.0)
                    {
                        pFG += 2;
                    }
                }
                if (kousei)
                    PH[CT] = ynw[f_maxnum] + Math.PI * pFG;
                else
                    P2 = ynw[f_maxnum] + Math.PI * pFG;

            }
            else
                P1 = ynw[f_maxnum];

            PHA0 = ynw[f_maxnum];

            if (kousei)
                henni[CT] = (PH[CT] - P1) * 316.4 / (2 * Math.PI);
            else
                r_henni = (P2 - P1) * 316.4 / (2 * Math.PI);


            if (kousei)
                laser_henni.Text = henni[CT].ToString("0.00");
            else
                laser_henni.Text = r_henni.ToString("0.00");

            // iso[iso_count] = ynw[f_maxnum];
        }

        public void fft_cal2(int CT)
        {
            if (CT != 0)
            {
                double d = ynw2[f_maxnum2] - PHA0_2;

                if (Math.Abs(d) >= 5.0)
                {
                    if (d >= 4.0) //(d >= 5.0)
                    {
                        pFG2 -= 2;
                    }
                    else if (d <= -4.0)//(d <= -5.0)
                    {
                        pFG2 += 2;
                    }
                }
                if (kousei)
                    PH2[CT] = ynw2[f_maxnum2] + Math.PI * pFG2;
                else
                    P2_2 = ynw2[f_maxnum2] + Math.PI * pFG2;

            }
            else
                P1_2 = ynw2[f_maxnum2];

            PHA0_2 = ynw2[f_maxnum2];

            /*
            if (kousei)
                henni2[CT] = (PH2[CT] - P1_2) * 316.4 / (2 * Math.PI);
            else
                r_henni2 = -1 * (P2_2 - P1_2) * 316.4 / (2 * Math.PI);
            */

            //   if (kousei)
            //       laser_henni2.Text = henni2[CT].ToString("0.00");
            //   else
            //       laser_henni2.Text = r_henni2.ToString("0.00");
            //
            //   iso2[iso_count] = ynw2[f_maxnum2];
        }

        public void draw_FFT_graph()
        {
            double fdx = 1;
            //int gy1 = 0;

            for (float j = 1; j < nData / 2; j++)
            {
                pg.DrawLine(Pens.Black, (int)((Math.Log10(j * fdx)) * 250), 100, (int)((Math.Log10(j * fdx)) * 250), 100 - (int)(ynr[(int)j] * 0.02));
                // pg.DrawLine(Pens.Black, (int)((Math.Log10(j * fdx)) * 250),0, (int)((Math.Log10(j * fdx)) * 250), 20);
                //pg.DrawLine(Pens.Black, 20, 0, 100, 20);
            }
            pg.DrawLine(Pens.Red, (int)((Math.Log10(f_maxnum * fdx)) * 250), 100,
                (int)((Math.Log10(f_maxnum * fdx)) * 250), 100 - (int)(ynr[f_maxnum] * 0.02));
        }

        public void reset_raser()
        {
            CP.init_FFT(nData);
            CP2.init_FFT(nData);

            SignalGen();

            Spectrum(false, ynr, ynw, 1);
            Spectrum2(false, ynr2, ynw2, 1);

            FindMaxh();

            start_raser = ynw[f_maxnum];
            start_raser2 = ynw2[f_maxnum2];

            pre_raser = start_raser;
            pre_raser2 = start_raser2;

        }
        public double get_raser2()
        {
            int m;
            double l, p, d;


            CP2.init_FFT(nData);

            SignalGen();

            Spectrum2(false, ynr2, ynw2, 1);
            //f_maxnum2 = FindMaxh(ynr2);
            f_max2 = ynr2[f_maxnum2];


            m = f_maxnum2;


            d = ynw2[m] - pre_raser2;


            if (Math.Abs(d) >= 4.0)
            {
                if (d >= 4.0)
                    fg2 -= 2;

                else if (d <= -4.0)
                    fg2 += 2;
            }
            p = ynw2[m] + Math.PI * fg2;

            l = (p - start_raser2) * 632.82 * 0.5 / (2 * Math.PI);

            pre_raser2 = ynw2[m];


            return (l);

        }
        public double get_raser()
        {
            int m;
            double l, p, d;
            double th = 4;


            CP.init_FFT(nData);

            SignalGen();

            Spectrum(false, ynr, ynw, 1);

            //f_maxnum = FindMaxh(ynr);
            f_max = ynr[f_maxnum];


            m = f_maxnum;


            d = ynw[m] - pre_raser;


            if (Math.Abs(d) >= th)
            {
                if (d >= th)
                    fg -= 2;

                else if (d <= -1 * th)
                    fg += 2;
            }
            p = ynw[m] + Math.PI * fg;

            l = (p - start_raser) * 632.82 * 0.5 / (2 * Math.PI);


            //	str.Format("%5.2lf",f_max);
            //	wnd->SetDlgItemText(IDC_LASER_MAX, str);
            //	str.Format("%3d",f_maxnum);
            //	wnd->SetDlgItemText(IDC_SPECTLE_NUM, str);

            pre_raser = ynw[m];


            return (l);

        }

        public void fake_sampling(int S_CT)
        {
            for (double i = 0; i < 1024; i++)
            {
                double j = (i - ioffset) / 128;
                double sig = 1.2;
                //double j = (i - 300) / 128;
                //double sig = 1.2;

                double se = (1 / (Math.Sqrt(2 * Math.PI) * sig));
                se *= Math.Exp(-j * j / (2 * sig * sig));

                ((ushort[])ana.pAd)[(int)i] = (ushort)(se * (15000 * Math.Sin((2 * Math.PI * i) / 128 - S_CT * Math.PI / 10) + 15000));
                ((ushort[])ana.pAd)[(int)i] += 32700;
            }
            for (double i = 0; i < 1024; i++)
            {
                double j = (i - ioffset2) / 128;
                double sig = 1.2;
                //double j = (i - 300) / 128;
                //double sig = 1.2;

                double se = (1 / (Math.Sqrt(2 * Math.PI) * sig));
                se *= Math.Exp(-j * j / (2 * sig * sig));

                ((ushort[])ana.pAd2)[(int)i] = (ushort)(se * (15000 * Math.Sin((2 * Math.PI * i) / 128 - S_CT * Math.PI / 10) + 15000));
                ((ushort[])ana.pAd2)[(int)i] += 32700;
            }
        }
        public void plot(int num, double[] h)
        {
            Series series1 = new Series("評価装置");
            Series series2 = new Series("DSC-01");


            // 波形生成
            for (int i = 1; i < num; i++)
            {

                series1.Points.AddXY(i, henni[i]);
            }

            series1.ChartType = SeriesChartType.Line; // グラフ形状
            series2.ChartType = SeriesChartType.Line; // グラフ形状


            // chart1.Series.Clear();
            // chart1.Series.Add(series1);
            //   chart1.Series.Add(series2);


            // Axis ax = chart1.ChartAreas[0].AxisX;
            // ax.MajorGrid.LineColor = Color.LightGray;
            // ax.Maximum = -1000;

            // Axis ay = chart1.ChartAreas[0].AxisY;
            // ay.MajorGrid.LineColor = Color.LightGray;


            //ay.Minimum = 0;
        }
        private void threadSub()
        {

         
            double ld;

            int TEN;
            int data_num;

            sokutei_num = int.Parse(tb_sokutei.Text);
           // string strSamplingNum = SamplingNum.Text;
            iSamplingNum = 4000;// int.Parse(strSamplingNum);


            fg2 = 0;

            ana.pAd = new ushort[1024];
            ana.pAd2 = new ushort[1024];

            ana.pAd = new ushort[1024];
            ana.pAd2 = new ushort[1024];

            xn = new double[1024];
            xn2 = new double[1024];

            ynr = new double[1024];
            ynr2 = new double[1024];

            ynw = new double[1024];
            ynw2 = new double[1024];

            xx = new double[iSamplingNum + 11];

            PH = new double[500];

            henni = new double[iSamplingNum + 11];
            henni2 = new double[20, iSamplingNum + 11];

            pvolt = new double[iSamplingNum + 11];

            k_henni = new double[iSamplingNum + 11];
            k_henni2 = new double[iSamplingNum + 11];

           

          

            Pen p1 = new Pen(Brushes.SkyBlue);
            p1.DashStyle = DashStyle.DashDot;


            try
            {
                if (MessageBox.Show("校正をスタートします", "DSC-01", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    button2.Enabled = false;
                    button_laserRead.Enabled = false;
                    gmin = 65535;
                 
                    for (int sk = 0; sk < sokutei_num; sk++)
                    {
                        toolStripStatusLabel1.Text = string.Format("ステータス: " + (sk+1).ToString("d")+"回目 校正中");

                        fg = 0;
                     

                        //  button_laserRead.Enabled = false;
                        //button_start.Enabled = false;
                        TEN = 0;
                        data_num = 0;

                        ana.adOutDO(ana.hDevice, 0x05);


                        for (S_CT = 0; S_CT < 4000; S_CT++)
                        {

                            ana.ad_sampling(ana.hDevice);

                            draw_grid(g1, p1);
                            draw_fringe1(0, g1);


                            if (S_CT < 5)
                            {
                                reset_raser();
                                ld = 0;

                            }
                            else
                            {
                                ld = get_raser();
                                db1.Refresh();
                            }
                            if (ld > 1000.0) TEN = 1;

                            if ((TEN == 1 && ld < 100)||(m_cal==1 && ld>20))
                            {
                                data_num = S_CT;
                                break;
                            }

                           
                          
                            laser_henni.Text = ld.ToString("0.00");


                            laser_henni.Refresh();


                            //System.Threading.Thread.Sleep(20);
                            henni[S_CT] = ld;


                        }
                        ana.adOutDO(ana.hDevice, 0x08);
                        toolStripStatusLabel1.Text = string.Format("ステータス: " + (sk + 1).ToString("d") + "回目 校正終了");

                        int st, start;
                        st = 0;
                        for (int k = 10; k < 3500; k++)
                        {
                            if (System.Math.Abs(henni[k]) > 1000)
                            {
                                st = k;
                                break;
                            }
                        }
                        start = sai13(st, henni);

                        if (m_cal == 0)
                        {
                            for (int i = start; i < start + 1900; i++)
                            {
                                henni2[sk, i - start] = henni[i];
                            }
                        }
                        else
                        {
                            for (int i = 0; i <2000; i++)
                            {
                                henni2[sk, i] = henni[i];
                            }
                        }

                        DateTime now = DateTime.Now;
                        Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

                        StreamWriter writer =
                            new StreamWriter(@"C:\ENT_DSC01\LOG\laser-" + now.ToString("MMdd-HHmmss") + ".csv", true, sjisEnc);

                        writer.WriteLine("*1.0(Win10miniPC)");
                        writer.WriteLine("Calibration");
                        writer.WriteLine("Date:" + now.ToString("yy/MM/dd"));
                        writer.WriteLine("Time:" + now.ToString("hh:mm:ss"));
                        writer.WriteLine("Model:  Elionix DSC-01 ");
                        writer.WriteLine("*");
                        writer.WriteLine("DSC-01[μm]");

                        for (int i = 0; i < 1900; i++)
                        {
                            writer.WriteLine((henni2[sk,i] / 1000).ToString("F6"));
                        }
                        writer.Close();



                        if (sk != sokutei_num - 1)
                            System.Threading.Thread.Sleep(4000);


                    }
                    MessageBox.Show("校正が終了しました。", "DSC-01");
                   
                }
                button2.Enabled = true;
                button_laserRead.Enabled = true;
                thread = null;
            }
            catch (System.Threading.ThreadAbortException)
            {
                MessageBox.Show("停止しました", "DSC-01");
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (thread == null)
            {
                thread = new System.Threading.Thread(new System.Threading.ThreadStart(threadSub));

                thread.Start();
            }
            

       
            //MessageBox.Show("終了しました");
          //  p1.Dispose();
         
            
           // button_laserRead.Enabled = true;
            //button_start.Enabled = true;
           

        }
        private void sokutei_tab_SelectedIndexChanged(object sender, EventArgs e)
        {
        /*    canvas3 = new Bitmap(pictureBox3.Width, pictureBox3.Height);
            g3 = Graphics.FromImage(canvas3);
            canvas4 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            g4 = Graphics.FromImage(canvas4);
            canvas5 = new Bitmap(pictureBox4.Width, pictureBox4.Height);
            g5 = Graphics.FromImage(canvas5);
            canvas6 = new Bitmap(pictureBox6.Width, pictureBox6.Height);
            g6 = Graphics.FromImage(canvas6);

            gb = new LinearGradientBrush(
                     g3.VisibleClipBounds,
                     Color.SkyBlue,
                     Color.White,
                     LinearGradientMode.Vertical);

            g3.FillRectangle(gb, g3.VisibleClipBounds);

            Pen p = Pens.SkyBlue;
            draw_grid(g4, p);
            draw_fringe1(0, g4);
            draw_fringe2(0, g4);
            draw_graph4(g5);
            draw_text(g3);


            g3.Dispose();
            g4.Dispose();
            g5.Dispose();
            g6.Dispose();


            //PictureBox1に表示する
            pictureBox3.Image = canvas3;
            pictureBox2.Image = canvas4;
            pictureBox4.Image = canvas5;
            pictureBox6.Image = canvas6;



            this.Refresh();
            GC.Collect();


            listView1.Items.Clear();

            for (int i = 0; i < Count; i++)
            {
                int n = i + 1;
                string[] item3 = { n.ToString(), file_name[i] };
                listView1.Items.Add(new ListViewItem(item3));
            }

*/
        }



        private void 線ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("aho");
        }

        //      private void StartCal_Click(object sender, EventArgs e)
        //      {
        //        int sokutei_num;
        //      sokutei_num = Convert.ToInt32(text_sokutei.Text);

        //    DialogResult result = MessageBox.Show("校正を開始します","校正開始",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button2);

        //何が選択されたか調べる
        //  if (result == DialogResult.OK)
        // {
        //   button_start.Enabled = false;

        // string[] fn = new string[10];
        // string[] date = new string[10];

        //DateTime dt = DateTime.Now;

        // canvas2 = new Bitmap(pictureBox2.Width, pictureBox2.Height);
        // fg = Graphics.FromImage(canvas2);

        // string drawString;

        //  Font drawFont = new Font("MS UI Gothic", 11);
        //  SolidBrush drawBrush = new SolidBrush(Color.Black);

        //     for(int sk=0; sk < sokutei_num; sk++)
        //     {
        //    fn[sk]="c:\\LOG\\laser_"+dt.ToString("MM")+dt.ToString("dd")+"-"+dt.ToString("HH")+dt.ToString("mm")+dt.ToString("ss");
        //    drawString = fn[sk];
        //    fg.DrawString(drawString, drawFont, drawBrush, 0, sk*25);
        //       }
        //
        // fg.Dispose();

        // pictureBox2.Image = canvas2;


        // drawFont.Dispose();
        // drawBrush.Dispose();

        //    }
        //     button_start.Enabled = true;

        //    }

        private void button_stop_Click(object sender, EventArgs e)
        {
            objThread.bContinue = false;
            EmSTOP = true;
        }


        private void t1_sub(ref int Count)
        {
            double p_vout = 0;

            double dac = 0;

            double ld, ld2;

            gmin = 50000;
            gmin2 = 50000;

            button_laserRead.Enabled = false;
            //button_start.Enabled = false;


            ana.adOutDO(ana.hDevice, 0x00);
            ana.adOutDO(ana.hDevice2, 0x00);

            try
            {

                if (PIEZO)
                {
                    ushort[] wData = new ushort[2];
                    uint[] ghChannel = new uint[2];


                    if (sign == 1)
                        dac = 32768 + pi_count * 40;
                    else if (pi_count < (int)(32768 / 40))
                        dac = 65535 - pi_count * 40;
                    else
                    {
                        dac = 32768;
                        PIEZO = false;
                        GraphAll = true;
                    }

                    if (dac >= 65500)
                    {
                        pi_count = 0;
                        sign = -1;
                    }

                    wData[0] = (ushort)(1 * dac);

                    p_vout = (dac * 20 / 65535) - 10;

                //    SamplingNum.Text = p_vout.ToString("f");

                    pvolt[pcount] = p_vout;

                    //  IFCDA_ANY.DaOutputDA(ana.hDA, 1, ana.DA_SmplChInf, wData);                 

                }
                if (bZero)
                {
                    Count = 0;
                    pFG = 0;

                    bZero = false;
                }
                /*
             
			
                 */

                //textBox1.Text = Count.ToString("d");
                /*ana.start_sampling(ana.hDevice);
                ana.start_sampling(ana.hDevice2);

                ana.adOutDO(ana.hDevice, 0x00);
                ana.adOutDO(ana.hDevice2, 0x00);

                ana.get_data1(ana.hDevice);
                ana.get_data2(ana.hDevice2);
                */

                ana.ad_sampling(ana.hDevice);


                Pen p = Pens.SkyBlue;
                draw_grid(g1, p);
          

                draw_fringe1(0, g1);
           

                /*    CP.init_FFT(nData); 
                    CP2.init_FFT(nData);

                    SignalGen();
                    Spectrum(false, ynr, ynw, 1);

                    Spectrum2(false, ynr2, ynw2, 1);

                    FindMaxh();
                  //  draw_FFT_graph();
                    fft_cal(Count);
                    fft_cal2(Count);
                */
                if (Count < 5)
                {
                    reset_raser();
                    ld = 0;
                    ld2 = 0;
                }
                else
                {
                    ld = get_raser();
                    //  ld2 = get_raser2();
                }
                henni[pcount] = r_henni;
              //  henni2[pcount] = r_henni2;


                if (PIEZO)
                {
                    iso_count++;
                    draw_graph3();
                }



                draw_graph2();

                if (PIEZO || GraphAll)
                {
                    // draw_graph2();
                    if (PIEZO) pcount++;
                }

                db1.Refresh();
                db2.Refresh();


                Count++;
                pi_count++;

            }
            catch (ThreadAbortException)
            {
                ana.close();

                button_laserRead.Enabled = true;
                // button_start.Enabled = true;

                MessageBox.Show("スレッドが停止した", "メッセージ");
            }
        }
        private void t2_sub(ref int Count)
        {
            double v;

            try
            {
                ushort[] wData = new ushort[2];
                uint[] ghChannel = new uint[2];
                int gnErrCode = -1;

                // Output one analog data on the board.
                for (double i = 32768; i < 65535; i += 0.04)
                {
                    wData[0] = (ushort)(1 * i);
                    v = (i * 20 / 65535) - 10;
                //    SamplingNum.Text = v.ToString("f");

                    if (objThread2.bContinue == false) break;
                    //   gnErrCode = IFCDA_ANY.DaOutputDA(ana.hDA, 1, ana.DA_SmplChInf, wData);
                    Count++;
                }
                for (double i = 65535; i > 32768; i -= 0.04)
                {
                    wData[0] = (ushort)(1 * i);
                    v = (i * 20 / 65535) - 10;
                  //  SamplingNum.Text = v.ToString("f");

                    if (objThread2.bContinue == false) break;

                    // gnErrCode = IFCDA_ANY.DaOutputDA(ana.hDA, 1, ana.DA_SmplChInf, wData);
                    Count++;
                }
                objThread2.bContinue = false;


            }
            catch (ThreadAbortException)
            {

                //IFCDA_ANY.DaClose(ana.hDA);

                MessageBox.Show("スレッドが停止した", "メッセージ");
            }
            // IFCDA_ANY.DaClose(hDA);
        }
        private void threadSub3()
        {
            try
            {
                Success_Num= 0;

                ana.adOutDO(ana.hDevice, 0x05);

                for (int i = 0; i < 10; i++)
                {
                    int ret = ana.Test_ad_sampling(ana.hDevice);
                    
                    if (ret == 0)
                         Success_Num++;
              
                    toolStripStatusLabel1.Text = string.Format("データ通信中:{0}", i.ToString());
                    System.Threading.Thread.Sleep(50);
                }
                ana.adOutDO(ana.hDevice, 0x08);

                if (Success_Num <= 1)
                {
                    MessageBox.Show("コントローラーと通信できません。電源をONしてください。","DSC-01");
                    toolStripStatusLabel1.Text = string.Format("ステータス:" + " error");
                }
                else
                    toolStripStatusLabel1.Text = string.Format("ステータス:" + " OK");

              
            }
            catch(System.Threading.ThreadAbortException)
            {
               // MessageBox.Show("スレッドが停止した","メッセージ");
            }
         
        }
        protected override void WndProc(ref Message msg)
        {
            try
            {
                switch (msg.Msg)
                {
                    case 0x8002:
                        this.Text = "aho" + msg.LParam.ToString();
                        break;
                    default:
                        base.WndProc(ref msg);
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void threadSub2()
        {

            double ld, ld2;
            int data_num = 5000;
            ushort[] wData = new ushort[2];

            //Axis ax = chart1.ChartAreas[0].AxisX;
            //Axis ay = chart1.ChartAreas[0].AxisY;
            //chart1.Series.Clear();


            // chart1.Series.Add("0");
            // chart1.Series.Add("1");

            // chart1.Series["0"].ChartType = SeriesChartType.Line;
            // chart1.Series["0"].MarkerStyle = MarkerStyle.Circle;
            // chart1.Series["0"].MarkerSize = 2;

            // chart1.Series["1"].ChartType = SeriesChartType.Line;
            // chart1.Series["1"].MarkerStyle = MarkerStyle.Circle;
            // chart1.Series["1"].MarkerSize = 2;

            // ax.Maximum = 100;
            // ax.Minimum = 0;
            // ay.Maximum = 0.2;
            // ay.Minimum = 0;


        //    string stroffset = offset.Text;
        //    ioffset = int.Parse(stroffset);
        //    string stroffset2 = offset2.Text;
        //    ioffset2 = int.Parse(stroffset2);
        //    string strSamplingNum = SamplingNum.Text;
        //    iSamplingNum = int.Parse(strSamplingNum);

            fg = 0;
            fg2 = 0;

            pAd = new ushort[1024];
            pAd2 = new ushort[1024];

            ana.pAd = new ushort[1024];
            ana.pAd2 = new ushort[1024];

            xn = new double[1024];
            xn2 = new double[1024];

            ynr = new double[1024];
            ynr2 = new double[1024];

            ynw = new double[1024];
            ynw2 = new double[1024];

            xx = new double[data_num * 2];

            PH = new double[500];

            henni = new double[data_num * 2];
            henni2 = new double[2,data_num * 2];

            pvolt = new double[data_num * 2];

            k_henni = new double[data_num * 2];
            k_henni2 = new double[data_num * 2];


            ////////////////

            //ana.daOutDO(ana.hDA, 0x0001);
            ana.adOutDO(ana.hDevice2, 0x00);


            //ana.daOutDO(ana.hDA, 0x003);

            //////////

            gmin = 50000;
            gmin2 = 50000;


            button_laserRead.Enabled = false;
            //button_start.Enabled = false;


            ana.adOutDO(ana.hDevice, 0x10);
            ana.adOutDO(ana.hDevice, 0x15);


            Pen p1 = new Pen(Brushes.SkyBlue);
            p1.DashStyle = DashStyle.DashDot;

            uint OUT1 = disp_range | 0x04;




            // int hEvent = CreateEvent(0, TRUE, FALSE, NULL);
            double h2 = 0.0;
            double h0 = 0.0;
            int num = 0;


            ana.adOutDO(ana.hDevice2, OUT1);//piezo on

            for (S_CT = 0; S_CT < 2000 + 5; S_CT++)
            {
                //textBox1.Text = S_CT.ToString("d");
                ana.ad_sampling3(ana.hDevice2, ana.hDevice);

                draw_grid(g1, p1);
                //   draw_grid(g2, p1);

                //  draw_grid2();
                draw_fringe1(S_CT, g1);
                //  draw_fringe2(S_CT,g2);

                if (S_CT == 5)
                {
                    h0 = ((ushort[])ana.pAd2)[500] * 2 * henni_range_haba[disp_range] / 65535;
                }
                else if (S_CT > 5)
                {
                    h2 = ((ushort[])ana.pAd2)[500] * 2 * henni_range_haba[disp_range] / 65535;
                }

                if (S_CT < 5)
                {
                    reset_raser();
                    ld = 0;
                    ld2 = 0;
                }
                else
                {
                    ld = get_raser();
                    ld2 = get_raser2();
                }
                if (S_CT > 5)
                {
                    db1.Refresh();
                    db2.Refresh();
                    laser_henni.Text = ld.ToString("0.00");
                    //   laser_henni2.Text = (h0 - h2).ToString("0.00");
                    //   textBox1.Text = S_CT.ToString("0");
                    laser_henni.Refresh();
                    //   laser_henni2.Refresh();
                    //   textBox1.Refresh();

                    //   if (S_CT > ax.Maximum)
                    //       ax.Maximum += 100;

                    //   if (Math.Abs(ld / 1000) > ay.Maximum)
                    //       ay.Maximum += 0.2;

                    //  chart1.Series["0"].Points.AddXY(S_CT, -1 * (h2 - h0));//3100->*-1
                    //  chart1.Series["1"].Points.AddXY(S_CT, Math.Abs(ld / 1000));
                    henni[num] = -1 * (h2 - h0);
                   // henni2[num] = Math.Abs(ld / 1000);

                    //System.Threading.Thread.Sleep(20);
                    // henni[S_CT] = ld;
                    //henni2[S_CT] = ld2;
                    num++;
                    //plot(S_CT, henni);
                }

            }
            OUT1 = disp_range;
            ana.adOutDO(ana.hDevice2, OUT1);//piezo off

            for (int i = 0; i < 1000; i++)
            {
                ana.ad_sampling3(ana.hDevice2, ana.hDevice);

                draw_grid(g1, p1);
                //  draw_grid(g2, p1);

                //  draw_grid2();
                draw_fringe1(S_CT, g1);
                //  draw_fringe2(S_CT,g2);


                h2 = ((ushort[])ana.pAd2)[500] * 2 * henni_range_haba[disp_range] / 65535;

                ld = get_raser();
                ld2 = get_raser2();


                db1.Refresh();
                db2.Refresh();
                laser_henni.Text = ld.ToString("0.00");
                // laser_henni2.Text = ld2.ToString("0.00");
                //textBox1.Text = S_CT.ToString("0");
                //laser_henni.Refresh();
                // laser_henni2.Refresh();
                //textBox1.Refresh();


                // chart1.Series["0"].Points.AddXY(S_CT, -1*(h2 - h0));
                // chart1.Series["1"].Points.AddXY(S_CT, Math.Abs(ld / 1000));

                // henni[num] = -1*(h2 - h0);
                // henni2[num] = Math.Abs(ld / 1000);

                // num++;
                S_CT--;
            }
            p1.Dispose();
            //ana.close();

            button_laserRead.Enabled = true;
            //button_start.Enabled = true;
            //ana.adOutDO(ana.hDevice, 0x08);
            // ana.adOutDO(ana.hDevice2, 0x08);


            // ana.daOutDO(ana.hDA, 0x0000);
            //ana.adOutDO(ana.hDevice, 0x00);
            /*   pi_count = 0;
               PIEZO = true;
               sign = 1;
               pFG = 0;
               r_henni = 0;
               pcount = 0;

               XMAX = 1000; //[nm]
               YMAX = 1;//[V]

               bZero = true;
             */
        }
        private void button5_Click(object sender, EventArgs e)
        {
            thread = new System.Threading.Thread(new System.Threading.ThreadStart(threadSub3));

            thread.Start();

        }
  
        private void button3_Click(object sender, EventArgs e)
        {
            thread = new System.Threading.Thread(new System.Threading.ThreadStart(threadSub2));

            thread.Start();

        }
        private void dig_out_Click(object sender, EventArgs e)
        {
            ana.adOutDO(ana.hDevice2, 0x10);
        }
        private void ZERO_Click(object sender, EventArgs e)
        {
            ana.adOutDO(ana.hDevice2, 0x00);

            //bZero = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ana.adOutDO(ana.hDevice2, 0x00);
        }

        private void dig_out2_Click(object sender, EventArgs e)
        {
            ana.adOutDO(ana.hDevice2, 0x20);
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TAB2 = true;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            //ana.close();
            Application.Exit();
        }

        private void w_apply_Click(object sender, EventArgs e)
        {
            Pen p = Pens.Black;
            draw_grid(g1, p);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
         /*   decimal s = numericUpDown1.Value;

            if (s < pre_NUMERIC)
            {
                switch ((int)s)
                {
                    case 9:
                        numericUpDown1.Value = 4;
                        break;
                    //piezo_test_show();
                    case 3:
                        numericUpDown1.Value = 2;
                        break;
                }
            }
            else if (s > pre_NUMERIC)
            {
                switch ((int)s)
                {
                    case 3:
                        numericUpDown1.Value = 4;
                        break;
                    case 5:
                        numericUpDown1.Value = 10;
                        break;
                    default:
                        break;
                }
            }
            pre_NUMERIC = (int)numericUpDown1.Value;

            
                        canvas3 = new Bitmap(pictureBox3.Width, pictureBox3.Height);
                        g3 = Graphics.FromImage(canvas3);
                        canvas5 = new Bitmap(pictureBox4.Width, pictureBox4.Height);
                        g5 = Graphics.FromImage(canvas5);
                        canvas6 = new Bitmap(pictureBox6.Width, pictureBox6.Height);
                        g6 = Graphics.FromImage(canvas6);

                        gb = new LinearGradientBrush(
                                 g3.VisibleClipBounds,
                                 Color.SkyBlue,
                                 Color.White,
                                 LinearGradientMode.Vertical);

                        g3.FillRectangle(gb, g3.VisibleClipBounds);

                        Pen p = Pens.SkyBlue;
		
                        //draw_graph4(g5);
                        draw_text(g3);


                        g3.Dispose();
                        g4.Dispose();
                        g5.Dispose();
                        g6.Dispose();


                        //PictureBox1に表示する
                        pictureBox3.Image = canvas3;
                        pictureBox4.Image = canvas5;
                        pictureBox6.Image = canvas6;



                        this.Refresh();
                        GC.Collect();

                        draw_text(g3);
             */
        }

        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {

         //   int inc = (int)numericUpDown1.Increment;


        }

        private void numericUpDown1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                xn = new double[1024];
                ynr = new double[1024];
                ynw = new double[1024];
                PH = new double[500];
                henni = new double[2000];
                pvolt = new double[2000];
                iso = new double[2000];

                xn2 = new double[1024];
                ynr2 = new double[1024];
                ynw2 = new double[1024];
                PH2 = new double[500];
                henni2 = new double[2,2000];
                pvolt2 = new double[2000];
                iso2 = new double[2000];

                k_henni = new double[2000];
                k_henni2 = new double[2000];

                ana.pAd = new ushort[1024];
                ana.pAd2 = new ushort[1024];

                // time_prof = new double[2000];


                bZero = false;
                // bFileWrite = true;

                objThread = new threadClass(this.Handle, new deleDraw(t1_sub));

                objThread.gCount = 0;
                pcount = 0;
                objThread.bContinue = true;

                th1 = new Thread(new ThreadStart(objThread.wThread));

                th1.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
        public double Sin(int freq, int i)
        {
            double sin = Math.Sin(2 * (freq * fs / 180) * i * Math.PI / fs);
            return sin;
        }


        private void Async_laser_Click(object sender, EventArgs e)
        {
            try
            {
                if (thread != null)
                {
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            toolStripStatusLabel1.Text = string.Format("ステータス:" + " 通信停止");

            thread = null;

            button2.Enabled = true;
            button_laserRead.Enabled = true;

            ana.adOutDO(ana.hDevice, 0x08);

        }

        private void DispRange_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* int range = DispRange_Combo.SelectedIndex;

             switch (range)
             {
                 case 0:
                     disp_range = 0;
                     ana.adOutDO(ana.hDevice2, 0x00);
                     break;
                 case 1:
                     disp_range = 2;
                     ana.adOutDO(ana.hDevice2, 0x02);
                     break;
                 case 2:
                     disp_range = 1;
                     ana.adOutDO(ana.hDevice2, 0x01);
                     break;
             }
             * */
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = "aho.csv";

            sfd.InitialDirectory = @"C:\Users\elionix\Documents\sarada\DATA";

            sfd.Filter = "csvファイル|*.csv";

            sfd.FilterIndex = 2;

            sfd.Title = "保存先のファイルを選択してください";

            sfd.RestoreDirectory = true;

            sfd.OverwritePrompt = true;

            sfd.CheckPathExists = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {

                System.IO.Stream stream;

                stream = sfd.OpenFile();

                if (stream != null)
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(stream);

                    double ld = 0;
                    for (int i = 0; i < iSamplingNum * 2; i++)
                    {
                       // string ss = i.ToString("D") + "," + henni[i].ToString("F4") + "," + henni2[i].ToString("F4") + "\n";
                       // sw.Write(ss);
                    }
                    for (int i = 0; i < 1000; i++)
                    {
                        //      string ss = disp_data3[i].ToString("F4") + "," + Load_data3[i].ToString("F5") + "\n";
                        //       sw.Write(ss);
                    }
                    sw.Close();
                    stream.Close();
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ///   Axis ax2 = chart2.ChartAreas[0].AxisX;
            //  Axis ay2 = chart2.ChartAreas[0].AxisY;

            //chart2.ChartAreas[0].InnerPlotPosition.Auto = false;
            //chart2.ChartAreas[0].InnerPlotPosition.Width = 1;
            int start_num = 0;
            int end_num = 0;
            saisho_a = 0;
            saisho_b = 0;

            //    chart2.Series.Clear();

            //    ax2.Maximum = 2000;
            //   ax2.Interval = 200;
            //   ax2.Minimum = 0;

            //   ay2.Minimum = -50;
            //   ay2.Interval = 10;
            //   ay2.Maximum = 50;

            //  ax2.MajorGrid.LineColor = Color.LightGray;
            //  ay2.MajorGrid.LineColor = Color.LightGray;

            //  ax2.Title = "[変位[um]]";
            //  ay2.Title = "[残差[nm]]";

            //    chart2.Series.Add("リニアリティ");
            //    chart2.Series["リニアリティ"].ChartType = SeriesChartType.Line;

            start_num = 100;
            end_num = 1900;

            int k = 0;
            for (int i = 0; i < 2000; i++)
            {
                k_henni[k] = henni[i];
              //  k_henni2[k] = henni2[i];
                k++;
            }
            sai12(end_num - start_num, k_henni, k_henni2);
            //sai1(k_henni2, k_henni);
            //    textBox2.Text = saisho_a.ToString("0.00");
            //    textBox3.Text = saisho_b.ToString("0.00");

            double ld = 0;
            for (int i = start_num; i < end_num - 1; i++)
            {
                ld = k_henni[i];
                double kaiki = saisho_a * ld + saisho_b;
                double zansa = (k_henni2[i] - kaiki) * 1000;

                //       chart2.Series["リニアリティ"].Points.AddXY(i, zansa);
            }
            /*           chart2.Series.Add("除荷");
                       chart2.Series["除荷"].ChartType = SeriesChartType.Line;

                       k = 0;
                       for (int i = start_num; i < end_num; i++)
                       {
                           Load_data4[k] = Load_data3[i];
                           disp_data4[k] = disp_data3[i];
                           k++;
                       }
                       sai12(Load_data4, disp_data4);

                       ld = 0;
                       for (int i = 0; i < end_num - start_num - 1; i++)
                       {
                           ld = Load_data4[i];
                           double kaiki = saisho_a2 * ld + saisho_b2;
                           double zansa = (disp_data4[i] - kaiki) * 1000;
                           chart2.Series["除荷"].Points.AddXY(kaiki, zansa);
                       }

                       */
            //string s,s2;
            // s = saisho_a.ToString();
            // s2 = saisho_b.ToString();
            // MessageBox.Show(s +"+"+ s2);

        }
        private void threadSub4()
        {
     
            double ld;

            iSamplingNum = 40000;// int.Parse(strSamplingNum);


            fg2 = 0;

            ana.pAd = new ushort[1024];
            ana.pAd2 = new ushort[1024];

            ana.pAd = new ushort[1024];
            ana.pAd2 = new ushort[1024];

            xn = new double[1024];
            xn2 = new double[1024];

            ynr = new double[1024];
            ynr2 = new double[1024];

            ynw = new double[1024];
            ynw2 = new double[1024];

            xx = new double[iSamplingNum + 11];

            PH = new double[500];

            henni = new double[iSamplingNum + 11];
            henni2 = new double[2, iSamplingNum + 11];

            pvolt = new double[iSamplingNum + 11];

            k_henni = new double[iSamplingNum + 11];
            k_henni2 = new double[iSamplingNum + 11];

            button_laserRead.Enabled = false;
            button2.Enabled = false;


            Pen p1 = new Pen(Brushes.SkyBlue);
            p1.DashStyle = DashStyle.DashDot;


            try
            {
                        fg = 0;
                        gmin = 65535;
                
                        Success_Num = 0;

                        ana.adOutDO(ana.hDevice, 0x05);
                        draw_grid(g1, p1);
                        db1.Refresh();

                        for (int i = 0; i < 10; i++)
                        {
                            int ret = ana.Test_ad_sampling(ana.hDevice);

                            if (ret == 0)
                                Success_Num++;

                            toolStripStatusLabel1.Text = string.Format("データ通信中:{0}", i.ToString());
                            System.Threading.Thread.Sleep(50);
                        }
                        ana.adOutDO(ana.hDevice, 0x08);

                        if (Success_Num <= 1)
                        {
                            MessageBox.Show("コントローラーと通信できません。電源をONしてください。","DSC-01");
                            toolStripStatusLabel1.Text = string.Format("ステータス:" + " error");
                            button_laserRead.Enabled = true;
                            button2.Enabled = true;
                            thread = null;
                        }
                        else
                            toolStripStatusLabel1.Text = string.Format("ステータス:" + "通信中");

                ///////////////////////////////

                        if (Success_Num > 2)
                        {
                            ana.adOutDO(ana.hDevice, 0x05);


                            for (S_CT = 0; S_CT < 40000; S_CT++)
                            {

                                ana.ad_sampling(ana.hDevice);

                                draw_grid(g1, p1);



                                if (S_CT < 2)
                                {
                                    reset_raser();
                                    ld = 0;

                                }
                                else
                                {
                                    draw_fringe1(0, g1);
                                    ld = get_raser();
                                }

                                db1.Refresh();

                                laser_henni.Text = ld.ToString("0.00");

                                laser_henni.Refresh();


                                //System.Threading.Thread.Sleep(20);
                                henni[S_CT] = ld;


                            }
                            ana.adOutDO(ana.hDevice, 0x08);
                        }
                  
            }
            
            catch (System.Threading.ThreadAbortException)
            {
                //MessageBox.Show("校正スレッドが停止した", "メッセージ");
            }
        
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            if (thread == null)
            {
                thread = new System.Threading.Thread(new System.Threading.ThreadStart(threadSub4));

                thread.Start();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
            StreamWriter writer =
              new StreamWriter(@"C:\ENT_DSC01\LOG\laser-" + now.ToString("MMdd-HHmmss") + ".csv", true, sjisEnc);
            writer.WriteLine("テスト書き込みです。");
            double aho1 = 2.34;

            for (int i = 0; i < 10; i++)
            {
                double aho;
                aho = aho1 * i;
                writer.WriteLine(aho.ToString("F"));
            }
            writer.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                if (thread != null)
                {
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void menu_settei_Click(object sender, EventArgs e)
        {
            if (m_cal == 0)
            {
                MessageBox.Show("標準校正モードで測定します");
                m_cal = 1;
            }
            else
            {
                MessageBox.Show("通常校正モードで測定します");
                m_cal = 0;
            }
        }

        

       
    }
  
}
