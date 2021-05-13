using System;
using System.Drawing;
using SD2D = System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Windows.Forms;

namespace StarSim
{
    public class Renderer
    {
        private Graphics g;
        private Control control;
        private StarSim simulation;
        private Camera camera;

        public bool ShowMarker = true;
        public bool ShowStarInfo = true;
        public bool ShowSimInfo = true;

        public bool HighRenderQuality = false;

        public Renderer(Camera cam, StarSim simulation)
        {
            this.camera = cam;
            this.simulation = simulation;
        }

        private void clampPoint(ref float posX, ref float posY, float refX, float refY)
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

            clampPoint(ref posX1, ref posY1, posX2, posY2);
            clampPoint(ref posX2, ref posY2, posX1, posY1);

            g.DrawLine(pen, posX1, posY1, posX2, posY2);
        }
        public void Render(object sender, Graphics graphics)
        {
            control = (Control)sender;
            var SWTotal = new Stopwatch();
            SWTotal.Start();
            g = graphics;

            if (HighRenderQuality)
            {
                g.InterpolationMode = SD2D.InterpolationMode.High;
                g.CompositingQuality = SD2D.CompositingQuality.HighQuality;
                g.PixelOffsetMode = SD2D.PixelOffsetMode.HighQuality;
                g.SmoothingMode = SD2D.SmoothingMode.HighQuality;
            }
            else
            {
                g.InterpolationMode = SD2D.InterpolationMode.NearestNeighbor;
                g.CompositingQuality = SD2D.CompositingQuality.HighSpeed;
                g.PixelOffsetMode = SD2D.PixelOffsetMode.HighSpeed;
                g.SmoothingMode = SD2D.SmoothingMode.HighSpeed;
            }

            var backPenColor = Color.FromArgb(40, 70, 60);

            var backPen = new Pen(backPenColor, 1);
            backPen.DashPattern = new float[] { 8, 4 };

            var backPen2 = new Pen(backPenColor, 1);
            backPen2.DashPattern = new float[] { 8, 4, 4, 8 };

            var guiLine = new Pen(Color.FromArgb(40, 60, 80), 1);

            Star[] starArray = simulation.Stars;

            if (ShowMarker)
            {
                float centerPosX, centerPosY;
                camera.WorldToScreenSpace(simulation.MassCenterX, simulation.MassCenterY, out centerPosX, out centerPosY);

                g.DrawEllipse(backPen, centerPosX - 20f, centerPosY - 20f, 40, 40);

                float refPosX = centerPosX, refPosY = centerPosY;
                if (simulation.RefStar != null)
                {
                    camera.WorldToScreenSpace(simulation.RefStar.PosX, simulation.RefStar.PosY, out refPosX, out refPosY);
                    g.DrawEllipse(backPen, refPosX - 10, refPosY - 10, 20, 20);
                    drawLine(backPen, centerPosX, centerPosY, refPosX, refPosY);
                }
                float focusPosX = centerPosX, focusPosY = centerPosY;
                if (simulation.FocusStar != null)
                {
                    camera.WorldToScreenSpace(simulation.FocusStar.PosX, simulation.FocusStar.PosY, out focusPosX, out focusPosY);
                }
                int dist1 = 10, dist2 = 40;
                drawLine(backPen, focusPosX + dist1, focusPosY + dist1, focusPosX + dist2, focusPosY + dist2);
                drawLine(backPen, focusPosX - dist1, focusPosY + dist1, focusPosX - dist2, focusPosY + dist2);
                drawLine(backPen, focusPosX + dist1, focusPosY - dist1, focusPosX + dist2, focusPosY - dist2);
                drawLine(backPen, focusPosX - dist1, focusPosY - dist1, focusPosX - dist2, focusPosY - dist2);

                if (simulation.SelectetStar != null)
                {
                    float curPosX, curPosY;
                    camera.WorldToScreenSpace(simulation.SelectetStar.PosX, simulation.SelectetStar.PosY, out curPosX, out curPosY);
                    drawLine(backPen2, curPosX, curPosY, refPosX, refPosY);
                }
            }

            var brush = new SolidBrush(Color.LightGray);

            for (int iS = 0; iS < simulation.Stars.Length; iS++)
            {
                var star = starArray[iS];
                if (!star.Enabled) continue;

                float r = star.Radius;

                camera.WorldToScreenSpace(star.PosX - r, star.PosY - r, out float posX, out float posY);
                r *= (float)camera.Scale;

                if (r < 0.05) r = 0.05f;
                
                if (star.Editor != null)
                {
                    float starPosX, starPosY, goalPosX, goalPosY;
                    camera.WorldToScreenSpace(star.PosX, star.PosY, out starPosX, out starPosY);
                    switch (star.Editor.SelectetIndex)
                    {
                        case 2: camera.WorldToScreenSpace(star.PosX + (star.SpeedX - simulation.SpeedCenterX) * 10, star.PosY + (star.SpeedY - simulation.SpeedCenterY) * 10, out goalPosX, out goalPosY); break;
                        case 3: camera.WorldToScreenSpace(star.PosX + (star.SpeedX - simulation.RefStar.SpeedX) * 10, star.PosY + (star.SpeedY - simulation.RefStar.SpeedY) * 10, out goalPosX, out goalPosY); break;
                        default: camera.WorldToScreenSpace(star.PosX + star.SpeedX * 10, star.PosY + star.SpeedY * 10, out goalPosX, out goalPosY); break;

                    }
                    drawLine(new Pen(Color.FromArgb(255, 50, 200, 100), 2), starPosX, starPosY, goalPosX, goalPosY);
                    var pen = new Pen(Color.FromArgb(50, 50, 200, 100), 4);
                    g.DrawEllipse(pen, posX, posY, r * 2, r * 2);
                }
                
                if (star == simulation.SelectetStar)
                {
                    if (star.Mass > 0) g.DrawEllipse(new Pen(Color.FromArgb(170, 255, 255), 1), posX, posY, r * 2, r * 2);
                    else g.DrawEllipse(new Pen(Color.FromArgb(255, 170, 255), 1), posX, posY, r * 2, r * 2);
                }
                else
                {
                    //if (star.Mass > 0) 
                    g.DrawEllipse(new Pen(Color.FromArgb(100, 200, 255), 1), posX, posY, r * 2, r * 2);
                    //else g.DrawEllipse(new Pen(Color.FromArgb(200, 100, 255), 1), posX, posY, r * 2, r * 2);
                }

                if ((simulation.SelectetStar == star || star.Marked) && ShowStarInfo || star.Editor != null)
                {
                    posX += r;
                    posY += r;
                    g.DrawLine(guiLine, new PointF(posX, posY), new PointF(posX + r * 1.5f, posY - r * 1.5f));
                    g.DrawLine(guiLine, new PointF(posX + r * 1.5f, posY - r * 1.5f), new PointF(posX + r * 1.5f + r * 2f + 4f, posY - r * 1.5f));
                    int txtPosX = (int)(posX + r * 1.5f + r * 2f + 9f);
                    int txtPosY = (int)(posY - r * 1.5f - 6f) - 15;
                    if (star.Name.Length > 0) g.DrawString("" + ((star.Name.Length > 0) ? star.Name : string.Format("{0:X}", star.Idx)), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    g.DrawString("" + Math.Round(star.Mass, 2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    if (star.Editor != null)
                    {
                        g.DrawString("" + Math.Round(Math.Abs(star.SpeedX) + Math.Abs(star.SpeedY), 2), new Font("Consolas", 9), brush, new PointF(txtPosX, txtPosY += 15));
                    }
                }

            }
            SWTotal.Stop();
            if (ShowSimInfo)
            {
                g.DrawString("Stars " + simulation.StarCount + " /" + simulation.Stars.Length, new Font("Consolas", 9), brush, new Point(10, 40));
                g.DrawString("Mass " + "-", new Font("Consolas", 9), brush, new Point(10, 50));
                g.DrawString("DrawTime " + SWTotal.ElapsedMilliseconds, new Font("Consolas", 9), brush, new Point(10, 60));
                g.DrawString("SimTime " + simulation.UsedTime, new Font("Consolas", 9), brush, new Point(10, 70));
                g.DrawString("SimSpeed x" + simulation.SimSpeed + (simulation.Running ? "" : " (paused)"), new Font("Consolas", 9), brush, new Point(10, 80));
                g.DrawString($"Pos {camera.PosX},{ camera.PosY}", new Font("Consolas", 9), brush, new Point(10, 90));
            }
        }

    }
}
