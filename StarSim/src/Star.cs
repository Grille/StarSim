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
using GGL.IO;


namespace StarSim
{

    public class Star
    {
        public int Idx;
        public string Name;
        public bool Enabled;
        public bool Marked;
        public bool Tracked;
        public EditStarDialog Editor;
        public Star Reference;
        public Star ColisionsRef;
        public float Mass;
        public float AbsMass;

        public float SizeR;

        public float[] PosTracking;

        public double PosX,PosY;
        public double NewPosX,NewPosY;
        public double SpeedX,SpeedY;
        public double NewSpeedX,NewSpeedY;

        //public int Richtung; //ghkads

        public Star(int id,float mass, double posX, double posY, double speedX, double speedY)
        {
            Idx = id;
            Name = "";
            Enabled = true;
            UpdateMass(mass);
            PosX = posX;PosY = posY;
            SpeedX = speedX; SpeedY = speedY;
            ColisionsRef = null;
            Reference = null;
        }
        public void UpdateMass(float mass)
        {
            Mass = mass;
            AbsMass = Math.Abs(mass);
            SizeR = (float)(Math.Sqrt(Math.Abs(mass)) / Math.PI) * 10;
        }
    }
}