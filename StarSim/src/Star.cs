using System;

namespace StarSim
{

    public class Star
    {
        public float Density;
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

        public float Radius;

        public float[] PosTracking;

        public double PosX, PosY;
        public double NewPosX, NewPosY;
        public double SpeedX, SpeedY;
        public double NewSpeedX, NewSpeedY;

        public Star(int id,float mass, double posX, double posY, double speedX, double speedY)
        {
            Idx = id;
            Name = "";
            Enabled = true;
            Density = 1;
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
            Radius = (float)(Math.Sqrt((AbsMass / Density)) / Math.PI)*10;
        }
    }
}
