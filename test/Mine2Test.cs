using NUnit.Framework;
using Mine2;
using static Mine2.Rose;
using static Mine2.Rose.Direction;

using System;

namespace Mine2Tests
{
    public class Tests
    {
        [Test]
        public void RoseTest()
        {
            Assert.AreEqual(S, Rotate(N, 4));
            Assert.AreEqual(E, Rotate(N, 2));
            Assert.AreEqual(NW, Rotate(N, -1));
            Assert.AreEqual(N, Rotate(NW, 1));
            Assert.AreEqual(N, Rotate(S, 4));
        }

    }
}
