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
        }

    }
}
