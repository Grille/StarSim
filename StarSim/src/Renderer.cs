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
        private Game game;
        private Control control;
        private SimulationData data;
        private Camera camera;
        private PerformanceWatch stats;

        public bool ShowMarker = true;
        public bool ShowStarInfo = true;
        public bool ShowSimInfo = false;

        public bool HighRenderQuality = false;

        public Renderer(Game game)
        {
            this.game = game;
            camera = game.Camera;
            data = game.Data;
            stats = new PerformanceWatch();
        }

        private void clampPointToScreen(ref float posX, ref float posY, float refX, float refY)
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

            clampPointToScreen(ref posX1, ref posY1, posX2, posY2);
            clampPointToScreen(ref posX2, ref posY2, posX1, posY1);

            g.DrawLine(pen, posX1, posY1, posX2, posY2);
        }
        public void Render(object sender, Graphics graphics)
        {
            control = (Control)sender;
            stats.Begin();
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

            if (ShowMarker)
            {
                float centerPosX, centerPosY;
                camera.WorldToScreenSpace(data.MassCenterX, data.MassCenterY, out centerPosX, out centerPosY);

                g.DrawEllipse(backPen, centerPosX - 20f, centerPosY - 20f, 40, 40);
                
                float refPosX = centerPosX, refPosY = centerPosY;
                if (data.RefStar != null)
                {
                    camera.WorldToScreenSpace(data.RefStar.PosX, data.RefStar.PosY, out refPosX, out refPosY);
                    g.DrawEllipse(backPen, refPosX - 10, refPosY - 10, 20, 20);
                    drawLine(backPen, centerPosX, centerPosY, refPosX, refPosY);
                }
                float focusPosX = centerPosX, focusPosY = centerPosY;
                if (data.FocusStar != null)
                {
                    camera.WorldToScreenSpace(data.FocusStar.PosX, data.FocusStar.PosY, out focusPosX, out focusPosY);
                }
                int dist1 = 10, dist2 = 40;
                drawLine(backPen, focusPosX + dist1, focusPosY + dist1, focusPosX + dist2, focusPosY + dist2);
                drawLine(backPen, focusPosX - dist1, focusPosY + dist1, focusPosX - dist2, focusPosY + dist2);
                drawLine(backPen, focusPosX + dist1, focusPosY - dist1, focusPosX + dist2, focusPosY - dist2);
                drawLine(backPen, focusPosX - dist1, focusPosY - dist1, focusPosX - dist2, focusPosY - dist2);

                if (data.SelectetStar != null)
                {
                    float curPosX, curPosY;
                    camera.WorldToScreenSpace(data.SelectetStar.PosX, data.SelectetStar.PosY, out curPosX, out curPosY);
                    drawLine(backPen2, curPosX, curPosY, refPosX, refPosY);
                }
                
            }

            var brush = new SolidBrush(Color.LightGray);

            for (int iS = 0; iS < data.Count; iS++)
            {
                var star = data[iS];
                if (!star.Enabled) continue;

                float r = star.Radius;

                camera.WorldToScreenSpace(star.PosX - r, star.PosY - r, out float posX, out float posY);
                r *= (float)camera.Scale;

                if (r < 0.05) r = 0.05f;
                
                if (star.Editor != null)
                {
                    float starPosX, starPosY, goalPosX, goalPosY, hX,hY;
                    camera.WorldToScreenSpace(star.PosX, star.PosY, out starPosX, out starPosY);
                    camera.WorldToScreenSpace(star.PosX + star.PullX * 10000, star.PosY + star.PullY * 10000, out hX, out hY);
                    switch (star.Editor.SelectetIndex)
                    {
                        case 2: camera.WorldToScreenSpace(star.PosX + (star.SpeedX - data.SpeedCenterX) * 10, star.PosY + (star.SpeedY - data.SpeedCenterY) * 10, out goalPosX, out goalPosY); break;
                        case 3: camera.WorldToScreenSpace(star.PosX + (star.SpeedX - data.RefStar.SpeedX) * 10, star.PosY + (star.SpeedY - data.RefStar.SpeedY) * 10, out goalPosX, out goalPosY); break;
                        default: camera.WorldToScreenSpace(star.PosX + star.SpeedX * 10, star.PosY + star.SpeedY * 10, out goalPosX, out goalPosY); break;

                    }
                    drawLine(new Pen(Color.FromArgb(255, 50, 200, 100), 2), starPosX, starPosY, goalPosX, goalPosY);

                    drawLine(new Pen(Color.FromArgb(255, 245, 200, 100), 2), starPosX, starPosY, hX, hY);

                    var pen = new Pen(Color.FromArgb(50, 50, 200, 100), 4);
                    g.DrawEllipse(pen, posX, posY, r * 2, r * 2);
                }
                
                if (star == data.SelectetStar)
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

                if ((data.SelectetStar == star || star.Marked) && ShowStarInfo || star.Editor != null)
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
            stats.EndAndLog();
            string info = "";
            info += $"Stars {data.EnabledCount} / {data.Count}\n";
            info += $"Comparisons {data.TotalComparisons}\n";
            info += $"Mass {data.TotalMass}\n";
            info += $"SimSpeed x{game.Sim.SimSpeed * game.Sim.SimMultiplier} {(game.Running ? "" : " (paused)")}\n\n";
            if (ShowSimInfo)
            {
                info += $"-Drawing:\n  delta: {stats.AverageTime}ms\n  fps: {stats.FPS}\n\n";
                var simstats = game.Sim.Stats;
                info += $"-Simulation:\n  delta: {simstats.AverageTime}ms\n  fps: {simstats.FPS} / {game.BaseFramerate * game.Sim.SimSpeed}\n\n";
                info += $"-SimTasks[{game.Sim.TaskCount}]\n";
                for (int i = 0; i< game.Sim.TaskCount; i++)
                {
                    var taskstats = game.Sim.TaskStats[i];
                    int percent = (int)(taskstats.AverageTime / simstats.AverageTime*100);
                    info += $"  t{i,2}: delta: {(int)taskstats.AverageTime,4}ms fps: {percent,3}%\n";
                }
                info += "\n";
                info += $"Pos {camera.PosX},{ camera.PosY}\n";
            }
            g.DrawString(info, new Font("Consolas", 9), brush, new Point(10, 40));
        }

    }
}
