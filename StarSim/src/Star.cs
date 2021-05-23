using System;

namespace StarSim
{

    public class Star
    {
        public double Density;
        public int Idx;
        public string Name;
        public bool Enabled;
        public bool Marked;
        public bool Tracked;
        public EditStarDialog Editor;
        public Star Reference;
        public Star ColisionsRef;
        public double Mass;
        public double AbsMass;

        public float Radius;

        public float[] PosTracking;

        public double PosX, PosY;
        public double PullX, PullY;
        public double SpeedX, SpeedY;

        public Star(int id, double mass, double posX, double posY, double speedX, double speedY)
        {
            Idx = id;
            Name = "";
            Enabled = true;
            Density = 100;
            UpdateMass(mass);
            PosX = posX;PosY = posY;
            SpeedX = speedX; SpeedY = speedY;
            ColisionsRef = null;
            Reference = null;
        }
        public void UpdateMass(double mass)
        {
            Mass = mass;
            AbsMass = Math.Abs(mass);
            Radius = (float)(Math.Sqrt((AbsMass / Density)) / Math.PI)*10;
        }
    }
}
