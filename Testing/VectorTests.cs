using SFML_GE.System;

namespace Testing
{
    [TestClass]
    public class VectorTests
    {
        [TestMethod]
        public void TestBasicMath()
        {
            Vector2 outputVec = new Vector2(100, 100) + new Vector2(100, 100);
            Assert.IsTrue(outputVec.Equals(new Vector2(200, 200)), outputVec.ToString());

            outputVec = new Vector2(50, -50) * new Vector2(-5, 5);
            Assert.IsTrue(outputVec.x == 50 * -5, outputVec.x.ToString());
            Assert.IsTrue(outputVec.y == -50 * 5, outputVec.y.ToString());
            Assert.IsTrue(outputVec.Equals(new Vector2(50 * -5, -50 * 5)), "Vector2.Equals() is incorrect!!");


            Vector2 lerped = Vector2.Lerp(new Vector2(0, 0), new Vector2(5, 5), 0.5f);
            Assert.IsTrue(lerped.Equals(new Vector2(2.5f, 2.5f)), "Lerp is incorrect!");
        }

    }
}
