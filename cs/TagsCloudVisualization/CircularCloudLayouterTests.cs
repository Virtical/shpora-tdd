using System;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace TagsCloudVisualization;

[TestFixture]
public class CircularCloudLayouterTests
{
    [Test]
    public void Constructor_SetCenterCorrectly_WhenInitialized()
    {
        var center = new Point(-5, 2);
        var cloudCenter = new CircularCloudLayouter(center).Center;

        cloudCenter.Should().Be(center);
    }

    [Test]
    public void CloudSizeIsZero_WhenInitialized()
    {
        var circularCloudLayouter = new CircularCloudLayouter(Point.Empty);

        var actualSize = circularCloudLayouter.Size();

        actualSize.Should().Be(Size.Empty);
    }

    [Test]
    public void CloudSizeEqualsFirstTagSize_WhenPuttingFirstTag()
    {
        var circularCloudLayouter = new CircularCloudLayouter(Point.Empty);
        var rectangleSize = new Size(10, 8);

        circularCloudLayouter.PutNextRectangle(rectangleSize);

        var actualSize = circularCloudLayouter.Size();
        var expectedSize = rectangleSize;

        actualSize.Should().Be(expectedSize);
    }

    [Test]
    public void CloudSizeIsCloseToCircleShape_WhenPuttingManyTags()
    {
        var circularCloudLayouter = new CircularCloudLayouter(Point.Empty);
        var rectangleSize = new Size(3, 2);

        for (var i = 0; i < 100; i++)
        {
            circularCloudLayouter.PutNextRectangle(rectangleSize);
        }

        var actualSize = circularCloudLayouter.Size();
        var aspectRatio = (double)actualSize.Width / actualSize.Height;

        aspectRatio.Should().BeInRange(0.5, 2.0);
    }

    [TestCase(0, TestName = "NoTags_WhenInitialized")]
    [TestCase(1, TestName = "SingleTag_WhenPuttingFirstTag")]
    [TestCase(10, TestName = "MultipleTags_WhenPuttingALotOfTags")]
    public void CloudContains(int rectangleCount)
    {
        var circularCloudLayouter = new CircularCloudLayouter(Point.Empty);
        var rectangleSize = new Size(5, 3);

        for (var i = 0; i < rectangleCount; i++)
        {
            circularCloudLayouter.PutNextRectangle(rectangleSize);
        }

        circularCloudLayouter.Tags.Count.Should().Be(rectangleCount);
    }

    [TestCase(0, 1, TestName = "WidthIsZero")]
    [TestCase(1, 0, TestName = "HeightIsZero")]
    [TestCase(-1, 1, TestName = "WidthIsNegative")]
    [TestCase(1, -1, TestName = "HeightIsNegative")]
    public void PutNextRectangle_ThrowException_When(int width, int height)
    {
        var circularCloudLayouter = new CircularCloudLayouter(Point.Empty);
        var incorrectRectangle = new Size(width, height);
        var putIncorrectRectangle = () => circularCloudLayouter.PutNextRectangle(incorrectRectangle);

        putIncorrectRectangle.Should().Throw<ArgumentException>();
    }

    [TestCase(8, 4, TestName = "LengthOfSidesRectangleIsEven")]
    [TestCase(9, 5, TestName = "LengthOfSidesRectangleIsOdd")]
    public void PutNextRectangle_PlacesFirstRectangleInCenter_When(int width, int height)
    {
        var center = new Point(3, -2);
        var circularCloudLayouter = new CircularCloudLayouter(center);
        var rectangleSize = new Size(width, height);
        var firstRectangle = circularCloudLayouter.PutNextRectangle(rectangleSize);

        var rectangleCenter = new Point(
            firstRectangle.Left + firstRectangle.Width / 2,
            firstRectangle.Top - firstRectangle.Height / 2);

        rectangleCenter.Should().Be(center);
    }

    [Test]
    public void PutNextRectangle_CloudTagsIsNotIntersect_WhenPuttingALotOfTags()
    {
        var circularCloudLayouter = new CircularCloudLayouter(Point.Empty);
        circularCloudLayouter.PutNextRectangle(new Size(10, 5));
        circularCloudLayouter.PutNextRectangle(new Size(9, 6));
        circularCloudLayouter.PutNextRectangle(new Size(8, 4));

        var tags = circularCloudLayouter.Tags;
        for (var i = 0; i < tags.Count; i++)
        {
            var currentRectangle = tags[i];
            tags
                .Where((_, j) => j != i)
                .All(otherRectangle => !currentRectangle.IntersectsWith(otherRectangle))
                .Should().BeTrue();
        }
    }
}