﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using TagsCloudVisualization;

public class LayoutVisualizer
{
    public static void Main()
    {
        var rectangleCounts = new List<int>{ 25, 50, 100 };

        var outputDirectory = "../../Layouts";
        Directory.CreateDirectory(outputDirectory);

        for (int i = 0; i < rectangleCounts.Count; i++)
        {
            var count = rectangleCounts[i];

            var circularCloudLayouter = new CircularCloudLayouter(new Point(350, 350));

            var layout = GenerateLayout(circularCloudLayouter, count);

            var imagePath = Path.Combine(outputDirectory, $"layout_{i + 1}.png");
            SaveLayoutImage(layout, imagePath, 700, 700);
        }

        var readmePath = Path.Combine(outputDirectory, "README.md");
        CreateReadmeFile(readmePath, rectangleCounts.Count);
    }

    private static CircularCloudLayouter GenerateLayout(CircularCloudLayouter layouter, int count)
    {
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            int height = random.Next(20, 40);
            int width = (int)(height * (2 + random.NextDouble()));
            var rectangleSize = new Size(width, height);

            layouter.PutNextRectangle(rectangleSize);
        }

        return layouter;
    }

    private static void SaveLayoutImage(CircularCloudLayouter layout, string filePath, int width, int height)
    {
        using var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.White);
        var pen = new Pen(Color.Blue, 2);

        foreach (var rectangle in layout.Tags)
        {
            graphics.DrawRectangle(pen, rectangle);
        }

        bitmap.Save(filePath);
    }

    private static void CreateReadmeFile(string directory, int layoutCount)
    {
        using var writer = new StreamWriter(directory);

        writer.WriteLine("# Layout Visualization");
        writer.WriteLine();

        for (int i = 1; i <= layoutCount; i++)
        {
            writer.WriteLine($"## Layout {i}");
            writer.WriteLine($"![Layout {i}](layout_{i}.png)");
            writer.WriteLine();
        }
    }
}
