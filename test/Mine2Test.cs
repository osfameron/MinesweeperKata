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

            Assert.AreEqual(W, Opposite(E));
        }

        [Test]
        public void CellTest()
        {
            Cell c1 = new Cell();
            Assert.AreEqual(0, c1.Neighbours.Count);

            Cell c2 = new Cell();

            c1.Connect(E, c2);
            Assert.AreEqual(1, c1.Neighbours.Count);
            Assert.AreEqual(1, c2.Neighbours.Count);

            Assert.AreEqual(c1.Neighbours[E], c2);
            Assert.AreEqual(c2.Neighbours[W], c1);

            Assert.Throws<ArgumentException>(
                () => {
                    Cell c3 = new Cell();
                    c1.Connect(E, c3);
                });

            Assert.Throws<ArgumentException>(
                () => {
                    Cell c3 = new Cell();
                    c3.Connect(W, c1);
                });

        }

    }
}
