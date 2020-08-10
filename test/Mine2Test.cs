using NUnit.Framework;
using Mine2;
using static Mine2.Rose;
using static Mine2.Rose.Direction;
using static Mine2.Extensions;

using System;
using System.Linq;

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

            Assert.AreEqual(NE, Mid(N, E));
            Assert.AreEqual(NE, Mid(E, N));
        }

        [Test]
        public void CellConnectTest()
        {
            var c1 = new Cell<int>(0);
            Assert.AreEqual(0, c1.Neighbours.Count);

            var c2 = new Cell<int>(0);

            c1.Connect(E, c2);
            Assert.AreEqual(1, c1.Neighbours.Count);
            Assert.AreEqual(1, c2.Neighbours.Count);

            Assert.AreEqual(c1.Neighbours[E], c2);
            Assert.AreEqual(c2.Neighbours[W], c1);

            Assert.Throws<ArgumentException>(
                () => {
                    var c3 = new Cell<int>(0);
                    c1.Connect(E, c3);
                });

            Assert.Throws<ArgumentException>(
                () => {
                    var c3 = new Cell<int>(0);
                    c3.Connect(W, c1);
                });
        }

        [Test]
        public void CellGrowTest()
        {
            var c1 = new Cell<int>(0);
            var c2 = c1.Grow(E);
            Assert.AreEqual(c1.Neighbours[E], c2);
            Assert.AreEqual(c2.Neighbours[W], c1);

            var c3 = c1.Grow(S);
            Assert.AreEqual(c1.Neighbours[S], c3);
            Assert.AreEqual(c3.Neighbours[N], c1);  
            Assert.AreEqual(c1.Neighbours[SE],  c2.Neighbours[S]); 
            Assert.AreEqual(c2.Neighbours[SW], c3);     
            Assert.AreEqual(c3.Neighbours[NE], c2);     
            Assert.AreEqual(c3.Neighbours[E], c2.Neighbours[S]);     

            Assert.Throws<ArgumentException>(() => {
                    var cx = new Cell<int>(0);
                    var cy = cx.Grow(NE);
            });
        }

        [Test]
        // 3x3 lattice test
        public void CellGrowLatticeTest()
        {
            var nw = new Cell<int>(0);
            var ne = nw.Grow(E).Grow(E);
            var se = ne.Grow(S).Grow(S);
            Assert.AreEqual(se, nw[SE][SE]);
            Assert.AreEqual(se, nw[S][S][E][E]);
            Assert.AreEqual(se, nw[E][E][S][S]);
            Assert.AreEqual(nw, ne[S][NW][W]);
            Assert.AreEqual(nw, se[W][W][N][N]);

            Assert.AreEqual(3, nw.Neighbours.Count);
            Assert.AreEqual(8, nw[E][S].Neighbours.Count);
            Assert.AreEqual(5, nw[E].Neighbours.Count);
            Assert.AreEqual(3, nw[E][E].Neighbours.Count);
            Assert.AreEqual(3, nw[S][S].Neighbours.Count);
            Assert.AreEqual(3, nw[E][E][S][S].Neighbours.Count);
        }

        [Test]
        public void LatticeTest()
        {
            var c = Cell<int>.Lattice(3,5, 0);
            Assert.AreEqual(5, c.Traverse(E).Count());
            Assert.AreEqual(3, c.Traverse(S).Count());
            Assert.AreEqual(new []{0,0,0}, c.Traverse(S).Select(v => v.Value).ToArray());

            Assert.AreEqual("00000\n00000\n00000", c.ToGridString());
        }

        [Test]
        public void MinesweeperTest()
        {
            var c = Cell<Piece>.Lattice(5,5, Piece.Empty);
            Assert.AreEqual(".....\n.....\n.....\n.....\n.....", c.ToGridString(PieceOut));
        }

    }
}
