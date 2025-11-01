using NUnit.Framework;
using UtilsCommon.Mathematics;

namespace EditorTests
{
    public class EasingTest
    {
        [TestCase(0, 0)]
        [TestCase(0.5f, 0.5f)]
        [TestCase(1, 1)]
        public void Linear_WhenGivenX_ReturnsEasedValue(float input, float output)
        {
            // Arrange
            // Is in TestCase

            // Act
            var result = Easing.Linear(input);

            // Assert
            Assert.AreEqual(result, output);
        }

        [TestCase(0, 0)]
        [TestCase(0.5f, 0.133974612f)]
        [TestCase(1, 1)]
        public void InCirc_WhenGivenX_ReturnsEasedValue(float input, float output)
        {
            // Arrange
            // Is in TestCase

            // Act
            var result = Easing.InCirc(input);

            // Assert
            Assert.AreEqual(result, output);
        }
    }
}