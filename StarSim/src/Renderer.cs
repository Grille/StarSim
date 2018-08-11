using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WF = System.Windows.Forms;
using GGL.IO;
using System.Globalization;
using GGL.Graphic;

namespace StarSim
{
    public class Renderer
    {
        private Graphics g;
        private Control control;
        private StarSim simulation;

        public double CamPosX, CamPosY;
        public double scaling = 1;

        public int ChildNumber = 0;

        public bool showMarker = true;
        public bool showStarInfo = true;
        public bool showSimInfo = true;

        public bool HighRenderQuality = false;

        public Renderer(StarSim simulation)
        {
            this.simulation = simulation;
        }

        private void transformPoint(double x, double y, out float outX, out float outY)
        {
            outX = ((float)x + (float)CamPosX) * (float)scaling + control.Width / 2f;
            outY = ((float)y + (float)CamPosY) * (float)scaling + control.Height / 2f;
        }
        private void fixPoint(ref float posX, ref float posY, float refX, float refY)
        {

            float mX = (refX - posX) / (refY - posY);
            float nullPosX = posX - mX * posY;

            float mY = (refY - posY) / (refX - posX);
            float nullPosY = posY - mY * posX;

            if (posX < 0)
            {
                posY = nullPosY;
                posX = 0;
            }
            if (posY < 0)
            {
                posX = nullPosX;
                posY = 0;
            }
            if (posX > control.Width)
            {
                posY = mY * control.Width + nullPosY;
                posX = control.Width;
            }
            if (posY > control.Height)
            {
                posX = mX * control.Height + nullPosX;
                posY = control.Height;
            }
        }
        private void drawLine(Pen pen, float posX1, float posY1, float posX2, float posY2)
        {

            if (posX1 < 0 && posX2 < 0) return;
            if (posY1 < 0 && posY2 < 0) return;
            if (posX1 > control.Width && posX2 > control.Width) return;
            if (posY1 > control.Height && posY2 > control.Height) return;

            fixPoint(ref posX1, ref posY1, posX2, posY2);
            fixPoint(ref posX2, ref posY2, posX1, posY1);

            /*
            g.DrawEllipse(new Pen(Color.Red, 2), posX1-10, posY1-10, 20, 20);
            g.DrawEllipse(new Pen(Color.Red, 2), posX2 - 10, posY2 - 10, 20, 20);

            /*
            g.DrawString("X: " + posX1, new Font("Consolas", 9), new SolidBrush(Color.Red), new PointF(posX1, posY1));
            */

            g.DrawLine(pen, posX1, posY1, posX2, posY2);
        }
        public void Render(object sender, Graphics graphics)
        {
            try {
                control = (Control)sender;
                Stopwatch SWTotal = new Stopwatch();
                SWTotal.Start();
                g = graphics;

                if (HighRenderQuality)
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                }
                else
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                }

                //g.TranslateTransform(Width / 2, Height / 2);
                //g.ScaleTransform(scaling, scaling);
                Color backPenColor = Color.FromArgb(40, 70, 60);

                Pen backPen = new Pen(backPenColor, 1);
                backPen.DashPattern = new float[] { 8, 4 };

                Pen backPen2 = new Pen(backPenColor, 1);
                backPen2.DashPattern = new float[] { 8, 4, 4, 8 };

                Pen guiLine = new Pen(Color.FromArgb(40, 60, 80), 1);

                /*
                g.DrawLine(backPen, 0, Height / 2, Width, Height / 2);
                g.DrawLine(backPen, Width/2, 0 , Width / 2, Height);
                */

                Star[] starArray = simulation.Stars;
                //Star curStar = simulation.SelectetStar, simulation.RefStar = simulation.RefStar, simulation.FocusStar = simulation.FocusStar;
                if (showMarker)
                {
                    float centerPosX, centerPosY;
                    transformPoint((float)simulation.MassCenterX, (float)simulation.MassCenterY, out centerPosX, out centerPosY);

                    g.DrawEllipse(backPen, centerPosX - 20f, centerPosY - 20f, 40, 40);

                    /*
                    g.DrawLine(backPen, 0, Height / 2, Width, Height / 2);
                    g.DrawLine(backPen, Width/2, 0 , Width / 2, Height);
                    */
                    float refPosX = centerPosX, refPosY = centerPosY;
                    if (simulation.RefStar != null)
                    {
                        transformPoint((float)simulation.RefStar.PosX, (float)simulation.RefStar.PosY, out refPosX, out refPosY);
                        g.DrawEllipse(backPen, refPosX - 10, refPosY - 10, 20, 20);
                        drawLine(backPen, centerPosX, centerPosY, refPosX, refPosY);
                    }
                    float focusPosX = centerPosX, focusPosY = centerPosY;
                    if (simulation.FocusStar != null)
                    {
                        transformPoint((float)simulation.FocusStar.PosX, (float)simulation.FocusStar.PosY, out focusPosX, out focusPosY);
                    }
                    int dist1 = 10, dist2 = 40;
                    drawLine(backPen, focusPosX + dist1, focusPosY + dist1, focusPosX + dist2, focusPosY + dist2);
                    drawLine(backPen, focusPosX - dist1, focusPosY + dist1, focusPosX - dist2, focusPosY + dist2);
                    drawLine(backPen, focusPosX + dist1, focusPosY - dist1, focusPosX + dist2, focusPosY - dist2);
                    drawLine(backPen, focusPosX - dist1, focusPosY - dist1, focusPosX - dist2, focusPosY - dist2);

                    if (simulation.SelectetStar != null)
                    {
                        float curPosX, curPosY;
                        transformPoint((float)simulation.SelectetStar.PosX, (float)simulation.SelectetStar.PosY, out curPosX, out curPosY);
                        drawLine(backPen2, curPosX, curPosY, refPosX, refPosY);
                    }
                }


                //try
                //{
                SolidBrush brush = new SolidBrush(Color.LightGray);

                for (int iS = 0; iS < simulation.Stars.Length; iS++)
                {
                    Star star = starArray[iS];
                    if (!star.Enabled) continue;

                    float r = star.Radius;

                    byte RColor = (byte)star.Mass;
                    float posX = (float)(star.PosX - r + CamPosX);
                    float posY = (float)(star.PosY - r + CamPosY);

                    posX *= (float)scaling; posY *= (float)scaling; r *= (float)scaling;
                    posX += control.Width / 2f; posY += control.Height / 2f;

                    if (r < 0.05) r = 0.05f;
                    if (star.Editor != null)
                    {
                        float starPosX, starPosY, goalPosX, goalPosY;
                        transformPoint(star.PosX, star.PosY, out starPosX, out starPosY);
                        switch (star.Editor.SelectetIndex)
                        {
                            case 2: transformPoint(star.PosX + (star.SpeedX - simulation.SpeedCenterX) * 10, star.PosY + (star.SpeedY - simulation.SpeedCenterY) * 10, out goalPosX, out goalPosY); break;
                            case 3: transformPoint(star.PosX + (star.SpeedX - simulation.RefStar.SpeedX) * 10, star.PosY + (star.SpeedY - simulation.RefStar.SpeedY) * 10, out goalPosX, out goalPosY); break;
                            default: transformPoint(star.PosX + star.SpeedX * 10, star.PosY + star.SpeedY * 10, out goalPosX, out goalPosY); break;

                        }
                        drawLine(new Pen(Color.FromArgb(255, 50, 200, 100), 2), starPosX, starPosY, goalPosX, goalPosY);
                        Pen peeeen = new Pen(Color.FromArgb(50, 50, 200, 100), 4);
                        g.DrawEllipse(peeeen, posX, posY, r * 2, r * 2);
                    }
                    if (star == simulation.SelectetStar)
                    {
                        if (star.Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(170, 255, 255), 1), posX, posY, r * 2, r * 2);
                        else g.DrawEllipse(new Pen(Color.FromArgb(255, 170, 255), 1), posX, posY, r * 2, r * 2);
                    }
                    else
                    {
                        if (star.Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(100, 200, 255), 1), posX, posY, r * 2, r * 2);
                        else g.DrawEllipse(new Pen(Color.FromArgb(200, 100, 255), 1), posX, posY, r * 2, r * 2);
                    }

                    if ((simulation.SelectetStar == star || star.Marked) && showStarInfo || star.Editor != null)
                    {
                        posX += r;
                        posY += r;
                        g.DrawLine(guiLine, new PointF(posX, posY), new PointF(posX + r * 1.5f, posY - r * 1.5f));
                        g.DrawLine(guiLine, new PointF(posX + r * 1.5f, posY - r * 1.5f), new PointF(posX + r * 1.5f + r * 2f + 4f, posY - r * 1.5f));
                        int txtPosX = (int)(posX + r * 1.5f + r * 2f + 9f);
                        int txtPosY = (int)(posY - r * 1.5f - 6f) - 15;
                        if (star.Name.Length > 0) g.DrawString("" + ((star.Name.Length > 0) ? star.Name : String.Format("{0:X}", star.Idx)), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                        g.DrawString("" + Math.Round(star.Mass, 2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                        if (star.Editor != null)
                        {
                            g.DrawString("" + Math.Round(Math.Abs(star.SpeedX) + Math.Abs(star.SpeedY), 2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                        }
                        txtPosY += 5;

                        /*
                        g.DrawString("Relative data to " +((simulation.RefStar != -1) ? ((starArray[simulation.RefStar].Name.Length > 0) ? starArray[simulation.RefStar].Name : String.Format("{0:X}", starArray[simulation.RefStar].ID)) : "Nothing"), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                        g.DrawString("Dist: " + Math.Abs((int)(massCenterX - starArray[iS].PosX)) + " posY: " +Math.Abs((int)(massCenterY - starArray[iS].PosY)), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                        g.DrawString("Speed: " + starArray[iS].SpeedX + " speedY: " + Math.Round(starArray[iS].SpeedY, 2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                        //g.DrawString("Speed: " + (Math.Abs(starArray[ii].SpeedX) + Math.Abs(starArray[ii].SpeedY)), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                   */
                    }

                }
                SWTotal.Stop();
                if (showSimInfo)
                {
                    g.DrawString("Stars " + simulation.StarCount + " /" + simulation.Stars.Length, new Font("Consolas", 9), brush, new Point(10, 40));
                    g.DrawString("Mass " + "-", new Font("Consolas", 9), brush, new Point(10, 50));
                    g.DrawString("DrawTime " + SWTotal.ElapsedMilliseconds, new Font("Consolas", 9), brush, new Point(10, 60));
                    g.DrawString("SimTime " + simulation.UsedTime, new Font("Consolas", 9), brush, new Point(10, 70));
                    g.DrawString("SimSpeed x" + simulation.SimSpeed + (Program.Simulation.Running ? "" : " (paused)"), new Font("Consolas", 9), brush, new Point(10, 80));
                    /*
                    g.DrawString("L_Time " + usedTime + "ms", new Font("Consolas", 9), brush, new Point(10, 50));
                    g.DrawString("R_Time " + SWTotal.ElapsedMilliseconds + "ms", new Font("Consolas", 9), brush, new Point(10, 60));
                    */
                }
            }
            catch (Exception) { }
        }

    }
}
