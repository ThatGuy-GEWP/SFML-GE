using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFML_GE.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    [TestClass]
    public class RandomTesting
    {
        // might not be the best way to test, but if rng fails for some reason 0 will most likely be over 30%
        [TestMethod]
        public void RandomGenDistributionTestInt() 
        {
            int[] numbers = new int[100];


            for (int i = 0; i < 500_000; i++)
            {
                int indx = RandomGen.Next(numbers.Length);
                numbers[indx] += 1;
            }

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine((i+1) + " : " + ((numbers[i] / 500_000f) * 100f) + "%");
                Assert.IsTrue(((numbers[i] / 100_000f) * 100f) < 30.0f, "number picked more then 30% of the time in a set of 1-100 out of 100k random samples");
            }
        }

        // Bodged way of testing floats
        [TestMethod]
        public void RandomGenDistributionTestFloat()
        {
            int[] numbers = new int[100];


            for (int i = 0; i < 500_000; i++)
            {
                int indx = (int)RandomGen.NextSingle((float)numbers.Length);
                numbers[indx] += 1;
            }

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine((i + 1) + " : " + ((numbers[i] / 500_000f) * 100f) + "%");
                Assert.IsTrue(((numbers[i] / 100_000f) * 100f) < 30.0f, "number picked more then 30% of the time in a set of 1-100 out of 100k random samples");
            }
        }

        [TestMethod]
        public void RandomGenRangeTest()
        {
            for(int i = 0; i < 500_000; i++)
            {
                int number = RandomGen.Next((int)0, (int)100);
                Assert.IsTrue(number < 100, "random int was out of range, greater then max");
                Assert.IsTrue(number >= 0, "random int was out of range, lower then max");
            }

            for (int i = 0; i < 500_000; i++)
            {
                float number = RandomGen.NextSingle(0.0f, 100.0f);
                Assert.IsTrue(number < 100, "random int was out of range, greater then max");
                Assert.IsTrue(number >= 0, "random int was out of range, lower then max");
            }
        }
    }
}
