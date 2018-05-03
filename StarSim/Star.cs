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

    public struct Star
    {
        public int ID;
        public string Name;
        public bool Kill;
        public bool Enabled;
        public int Reference;
        public float Mass;
        public float AbsMass;
        public float NewMass;
        public float SizeR;
        public float[] Pos;
        public float[] NewPos;
        public float[] Speed;
        public float[] NewSpeed;

        //public int Richtung; //ghkads

        public void Init(int id,float mass, float posX, float posY, float speedX, float speedY)
        {
            ID = id;
            Name = "";
            Enabled = true;
            UpdateMass(mass);
            Pos = new float[2] { posX, posY };
            Speed = new float[2] { speedX, speedY };
            NewMass = -1;
            Reference = -1;
        }
        public void UpdateMass()
        {
            if (NewMass == -1) return;
            UpdateMass(NewMass);
            NewMass = -1;
        }
        public void UpdateMass(float mass)
        {
            Mass = mass;
            AbsMass = Math.Abs(mass);
            SizeR = (float)(Math.Sqrt(Math.Abs(mass)) / Math.PI) * 10;
        }
    }
}