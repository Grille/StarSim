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
        Timer TimerLogik;
        public SimData Data;
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
            get => running;
            set
            {
                if (value == true) Start();
                else Stop();
            }
        }

        public Game()
        {
            Camera = new Camera();
            Data = new SimData();
            Sim = new Simulation(Camera, Data);

            TimerLogik = new Timer();
            TimerLogik.Interval = 1000 / BaseFramerate;
            TimerLogik.Elapsed += new ElapsedEventHandler(tick);
        }

        public void Start()
        {
            running = true;
            TimerLogik.Start();
        }
        public void Stop()
        {
            TimerLogik.Stop();
            running = false;
        }

        private void tick(object sender, ElapsedEventArgs args)
        {
            Sim.Run();
        }

        public void Load(string path)
        {
            Sim.WaitForIdle();
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
                Data.Stars = stars;
            }
        }

        public void Save(string path)
        {
            Sim.WaitForIdle();

            Data.CollapseStarArray();

            using (var bs = new BinaryView(path, false))
            {
                bs.WriteByte(0);

                bs.WriteInt32(Data.Stars.Length);
                bs.WriteDouble(Camera.PosX);
                bs.WriteDouble(Camera.PosY);
                bs.WriteDouble(Camera.Scale);
                Star[] stars = Data.Stars;
                for (int i = 0; i < Data.Stars.Length; i++)
                {
                    bs.WriteInt32(stars[i].Idx);
                    bs.WriteDouble(stars[i].Mass);
                    bs.WriteDouble(stars[i].PosX);
                    bs.WriteDouble(stars[i].PosY);
                    bs.WriteDouble(stars[i].SpeedX);
                    bs.WriteDouble(stars[i].SpeedY);
                    bs.WriteString(stars[i].Name);
                }
                bs.WriteInt32(Data.SelectetStar != null ? Data.SelectetStar.Idx : -1);
                bs.WriteInt32(Data.FocusStar != null ? Data.FocusStar.Idx : -1);
                bs.WriteInt32(Data.RefStar != null ? Data.RefStar.Idx : -1);

                bs.Compress();
            }
        }
    }
}
