﻿using NUnit.Framework;
using SharpNL.Utility;

namespace SharpNL.Tests.Utility {
    [TestFixture]
    internal class SpanTest {

        [Test]
        public void testGetEnd() {
            Assert.AreEqual(6, new Span(5, 6).End);
        }

        [Test]
        public void testCompareToEquals() {
            var a = new Span(30, 1000);
            var b = new Span(30, 1000);

            Assert.AreEqual(true, a.CompareTo(b) == 0);
        }

        [Test]
        public void testCompareToEqualsDiffType1() {
            var a = new Span(30, 1000, "a");
            var b = new Span(30, 1000, "b");

            Assert.AreEqual(true, a.CompareTo(b) == -1);
        }

        [Test]
        public void testCompareToEqualsDiffType2() {
            var a = new Span(30, 1000, "b");
            var b = new Span(30, 1000, "a");

            Assert.AreEqual(true, a.CompareTo(b) == 1);
        }

        [Test]
        public void testCompareToEqualsNullType1() {
            var a = new Span(30, 1000);
            var b = new Span(30, 1000, "b");

            Assert.AreEqual(true, a.CompareTo(b) == 1);
        }

        [Test]
        public void testCompareToEqualsNullType2() {
            var a = new Span(30, 1000, "b");
            var b = new Span(30, 1000);

            Assert.AreEqual(true, a.CompareTo(b) == -1);
        }

        [Test]
        public void testCompareToEqualsSameType() {
            var a = new Span(30, 1000, "a");
            var b = new Span(30, 1000, "a");

            Assert.AreEqual(true, a.CompareTo(b) == 0);
        }

        [Test]
        public void testCompareToHigher() {
            var a = new Span(100, 200);
            var b = new Span(300, 400);

            Assert.AreEqual(true, a.CompareTo(b) < 0);
        }

        [Test]
        public void testCompareToLower() {
            var a = new Span(100, 1000);
            var b = new Span(10, 50);

            Assert.AreEqual(true, a.CompareTo(b) > 0);
        }

        [Test]
        public void testContains() {
            var a = new Span(500, 900);
            var b = new Span(520, 600);

            Assert.AreEqual(true, a.Contains(b));
        }

        [Test]
        public void testContainsInt() {
            var a = new Span(10, 300);

            /* NOTE: here the span does not contain the endpoint marked as the end
             * for the span.  This is because the end should be placed one past the
             * true end for the span.  The indexes used must observe the same
             * requirements for the contains function.  */

            Assert.False(a.Contains(9));
            Assert.True(a.Contains(10));
            Assert.True(a.Contains(200));
            Assert.True(a.Contains(299));
            Assert.False(a.Contains(300));
        }

        [Test]
        public void testContainsWithEqual() {
            var a = new Span(500, 900);

            Assert.AreEqual(true, a.Contains(a));
        }

        [Test]
        public void testContainsWithHigherIntersect() {
            var a = new Span(500, 900);
            var b = new Span(500, 1000);

            Assert.AreEqual(false, a.Contains(b));
        }

        [Test]
        public void testContainsWithLowerIntersect() {
            var a = new Span(500, 900);
            var b = new Span(450, 1000);

            Assert.AreEqual(false, a.Contains(b));
        }

        [Test]
        public void testCrosses() {
            var a = new Span(10, 50);
            var b = new Span(40, 100);

            Assert.True(a.Crosses(b));
            Assert.True(b.Crosses(a));

            var c = new Span(10, 20);
            var d = new Span(40, 50);

            Assert.False(c.Crosses(d));
            Assert.False(d.Crosses(c));

            Assert.False(b.Crosses(d));
        }

        [Test]
        public void testEquals() {
            var a1 = new Span(100, 1000, "test");
            var a2 = new Span(100, 1000, "test");

            Assert.True(a1.Equals(a2));

            // end is different
            var b1 = new Span(100, 100, "test");
            Assert.False(a1.Equals(b1));

            // type is different
            var c1 = new Span(100, 1000, "Test");
            Assert.False(a1.Equals(c1));

            var d1 = new Span(100, 1000);

            Assert.False(d1.Equals(a1));
            Assert.False(a1.Equals(d1));
        }

        [Test]
        public void testEqualsWithNull() {
            var a = new Span(0, 0);

            Assert.AreEqual(a.Equals(null), false);
        }

        [Test]
        public void testGetStart() {
            Assert.AreEqual(5, new Span(5, 6).Start);
        }

        [Test]
        public void testIntersects() {
            var a = new Span(10, 50);
            var b = new Span(40, 100);

            Assert.True(a.Intersects(b));
            Assert.True(b.Intersects(a));

            var c = new Span(10, 20);
            var d = new Span(40, 50);

            Assert.False(c.Intersects(d));
            Assert.False(d.Intersects(c));

            Assert.True(b.Intersects(d));
        }

        [Test]
        public void testLength() {
            Assert.AreEqual(11, new Span(10, 21).Length);
        }

        [Test]
        public void testStartsWith() {
            var a = new Span(10, 50);
            var b = new Span(10, 12);

            Assert.True(a.StartsWith(a));

            Assert.True(a.StartsWith(b));

            Assert.False(b.StartsWith(a));
        }

        [Test]
        public void testToString() {
            Assert.IsNotEmpty(new Span(50, 100).ToString());
        }

        [Test]
        public void testTrim() {
            const string value = "  12 34  ";
            var span1 = new Span(0, value.Length);
            Assert.AreEqual("12 34", span1.Trim(value).GetCoveredText(value));
        }

        [Test]
        public void testTrimWhitespaceSpan() {
            const string value = "              ";
            var span1 = new Span(0, value.Length);
            Assert.AreEqual("", span1.Trim(value).GetCoveredText(value));
        }

        [Test]
        public void testhHashCode() {
            Assert.AreEqual(new Span(10, 11), new Span(10, 11));
        }
    }
}