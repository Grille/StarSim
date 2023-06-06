using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarSim
{
    public class SimulationData : IReadOnlyList<Star>
    {
        public record struct ComparisonPair(Star Star1, Star Star2)
        {
            public bool Enabled => Star1.Enabled && Star2.Enabled;
        }

        public Star SelectetStar, FocusStar, RefStar;
        private Star[] Stars { get; set; }

        public int EnabledCount;

        public double TotalMass;

        public double MassCenterX;
        public double MassCenterY;
        public double SpeedCenterX;
        public double SpeedCenterY;

        public long TotalComparisons => (long)Stars.Length * ((long)Stars.Length - 1) / 2;

        public int Count => ((IReadOnlyCollection<Star>)Stars).Count;

        public Star this[int index] => ((IReadOnlyList<Star>)Stars)[index];

        public SimulationData()
        {
            Reset();
        }

        public void Reset()
        {
            SelectetStar = null; FocusStar = null; RefStar = null;
            Stars = new Star[0];
            EnabledCount = 0;
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
            resizeStarArray(EnabledCount + 1);
            Stars[EnabledCount] = new Star(EnabledCount, mass, posX, posY, speedX, speedY);
            EnabledCount++;
        }

        public ComparisonPair GetComparisonPair(int index)
        {
            long comparisonIndex = index;
            long numObjects = Stars.Length;
            long objectIndex1 = numObjects - 2 - (int)Math.Floor(Math.Sqrt(-8 * comparisonIndex + 4 * numObjects * (numObjects - 1) - 7) / 2.0 - 0.5);
            long objectIndex2 = comparisonIndex + objectIndex1 + 1 - numObjects * (numObjects - 1) / 2 + (numObjects - objectIndex1) * ((numObjects - objectIndex1) - 1) / 2;

            return new ComparisonPair(Stars[objectIndex1], Stars[objectIndex2]);
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
        public void CollapseStarArray() { resizeStarArray(EnabledCount); }

        public IEnumerator<Star> GetEnumerator()
        {
            return ((IEnumerable<Star>)Stars).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Stars.GetEnumerator();
        }
    }
}
