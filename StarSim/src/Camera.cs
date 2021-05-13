using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StarSim
{
    public class Camera
    {
        public double PosX = 0;
        public double PosY = 0;
        public double Scale = 1;

        private int width = 0;
        private int height = 0;

        private Point lastLocation = new Point(0,0);

        public void SetScreenSize(int width,int height)
        {
            this.width = width;
            this.height = height;
        }

        public void ScreenToWorldSpace(double x, double y, out double outX, out double outY)
        {
            outX = (x - width / 2) / Scale + PosX;
            outY = (y - height / 2) / Scale + PosY;
        }

        public void WorldToScreenSpace(double x, double y, out float outX, out float outY)
        {
            outX = (float)((x - PosX) * Scale + width / 2);
            outY = (float)((y - PosY) * Scale + height / 2);
        }

        public void MouseMove(MouseEventArgs e, bool move)
        {
            if (move)
            {
                PosX += (lastLocation.X - e.Location.X) / Scale;
                PosY += (lastLocation.Y - e.Location.Y) / Scale;
            }
            lastLocation = e.Location;
        }

        public void MouseWheel(MouseEventArgs e, double scrollFactor)
        {
            ScreenToWorldSpace(e.Location.X, e.Location.Y,out double oldPosX, out double oldPosY);

            if (e.Delta > 0)
                Scale *= scrollFactor;
            else
                Scale /= scrollFactor;

            ScreenToWorldSpace(e.Location.X, e.Location.Y, out double newPosX, out double newPosY);
            PosX += oldPosX - newPosX;
            PosY += oldPosY - newPosY;
        }
    }
}
