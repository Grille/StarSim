using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSim
{
    public class SimData
    {
        public Star SelectetStar, FocusStar, RefStar;
        public Star[] Stars { get; set; }
        public int StarCount;
        public double TotalMass;

        public double MassCenterX;
        public double MassCenterY;
        public double SpeedCenterX;
        public double SpeedCenterY;

        public SimData()
        {
            Reset();
        }

        public void Reset()
        {
            SelectetStar = null; FocusStar = null; RefStar = null;
            Stars = new Star[0];
            StarCount = 0;
            TotalMass = 0;

            MassCenterX = 0;
            MassCenterY = 0;
            SpeedCenterX = 0;
            SpeedCenterY = 0;
        }
        public void Init(int mode, int size, int stars, float minMass, float maxMass, float disSpeed)
        {
            float maxSpeed = disSpeed / 2f;
            float minSpeed = -maxSpeed;

            Stars = new Star[stars];
            var rnd = new Random();

            if (mode == 0)
            {
                for (int ii = 0; ii < Stars.Length; ii++)
                {
                    Stars[ii] = new Star(ii, (float)((maxMass - minMass) * rnd.NextDouble() + minMass)
                        , (float)(size * rnd.NextDouble() - size / 2)
                        , (float)(size * rnd.NextDouble() - size / 2)
                        , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                        , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                    );
                }
            }
            else
            {
                for (int ii = 0; ii < Stars.Length; ii++)
                {
                    Stars[ii] = new Star(ii, (float)((maxMass - minMass) * rnd.NextDouble() + minMass)
                        , (float)(0)
                        , (float)(size * rnd.NextDouble() - size / 2)
                        , (float)((maxSpeed - minSpeed) * rnd.NextDouble() + minSpeed)
                        , (float)(0)
                    );
                }
            }

        }
        public void AddStar(float mass, double posX, double posY, double speedX, double speedY)
        {
            resizeStarArray(StarCount + 1);
            Stars[StarCount] = new Star(StarCount, mass, posX, posY, speedX, speedY);
            StarCount++;
        }

        private void resizeStarArray(int lenght)
        {
            int iDst = 0;
            Star[] newStars = new Star[lenght];
            for (int iSrc = 0; iSrc < Stars.Length; iSrc++)
            {
                if (Stars[iSrc].Enabled)
                {
                    Stars[iSrc].Idx = iDst;
                    newStars[iDst] = Stars[iSrc];
                    iDst++;
                }
            }
            Stars = newStars;
        }
        public void CollapseStarArray() { resizeStarArray(StarCount); }

    }
}
