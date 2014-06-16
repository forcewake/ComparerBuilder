namespace ComparerBuilder.Core.Tests.Comparers
{
    using ComparerBuilder.Core.Comparers;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class ConstComparerTests
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void ConstComparerShouldReturnInitValue(int init)
        {
            var constComparer = new ConstComparer<int>(init);
            constComparer.Compare(int.MinValue, int.MaxValue).Should().Be(init);
        }

        [Test]
        public void DefaultConstComparerForIntShouldReturnZero()
        {
            ConstComparer<int>.Default.Compare(int.MinValue, int.MaxValue).Should().Be(0);
        }

        [Test]
        public void DefaultConstComparerForStringShouldReturnZero()
        {
            ConstComparer<string>.Default.Compare(string.Empty, "").Should().Be(0);
        }
    }
}
