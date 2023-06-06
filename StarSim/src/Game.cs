using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using GGL;

namespace StarSim
{
    public class Game
    {
        public int BaseFramerate = 50;
        private bool running = false;
        public SimulationTimer Timer;
        public SimulationData Data;
        public Camera Camera;
        public Simulation Sim;

        private bool changed = true;
        public bool ViewChanged
        {
            get => changed || Camera.Changed || Sim.NewFrameCalculatet;
            set {
                if (value) changed = true;
                else
                {
                    changed = Camera.Changed = Sim.NewFrameCalculatet = false;
                }
            }
        }

        public bool Running
        {
            get => Timer.Running;
            set
            {
                if (value == true) 
                    Start();
                else 
                    Stop();
            }
        }

        public Game()
        {
            Camera = new Camera();
            Data = new SimulationData();
            Sim = new Simulation(Camera, Data);

            Timer = new SimulationTimer(Sim);
        }

        public void Start()
        {
            Timer.Start();
        }
        public void Stop()
        {
            Timer.Stop();
        }

        public void Load(string path)
        {
            Timer.Lock();

            Data.Reset();

            using (var bs = new BinaryView(path))
            {
                bs.Decompress();

                bs.ReadByte();
                int starArrayLenght = bs.ReadInt32();
                Camera.PosX = bs.ReadSingle();
                Camera.PosY = bs.ReadSingle();
                Camera.Scale = bs.ReadSingle();
                Star[] stars = new Star[starArrayLenght];
                for (int i = 0; i < starArrayLenght; i++)
                {
                    stars[i] = new Star(bs.ReadInt32(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle(), bs.ReadSingle());
                    stars[i].Name = bs.ReadString();
                }
                int starIdx;
                if ((starIdx = bs.ReadInt32()) != -1) Data.SelectetStar = stars[starIdx];
                if ((starIdx = bs.ReadInt32()) != -1) Data.FocusStar = stars[starIdx];
                if ((starIdx = bs.ReadInt32()) != -1) Data.RefStar = stars[starIdx];
                //Data.Stars = stars;
            }

            Timer.Free();
        }

        public void Save(string path)
        {
            Timer.Lock();

            Data.CollapseStarArray();

            using (var bs = new BinaryView(path, false))
            {
                bs.WriteByte(0);

                bs.WriteInt32(Data.Count);
                bs.WriteDouble(Camera.PosX);
                bs.WriteDouble(Camera.PosY);
                bs.WriteDouble(Camera.Scale);
                for (int i = 0; i < Data.Count; i++)
                {
                    var star = Data[i];
                    bs.WriteInt32(star.Idx);
                    bs.WriteDouble(star.Mass);
                    bs.WriteDouble(star.PosX);
                    bs.WriteDouble(star.PosY);
                    bs.WriteDouble(star.SpeedX);
                    bs.WriteDouble(star.SpeedY);
                    bs.WriteString(star.Name);
                }
                bs.WriteInt32(Data.SelectetStar != null ? Data.SelectetStar.Idx : -1);
                bs.WriteInt32(Data.FocusStar != null ? Data.FocusStar.Idx : -1);
                bs.WriteInt32(Data.RefStar != null ? Data.RefStar.Idx : -1);

                bs.Compress();
            }

            Timer.Free();
        }
    }
}
