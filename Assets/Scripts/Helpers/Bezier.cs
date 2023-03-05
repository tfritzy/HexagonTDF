using System;
using UnityEngine;

public static class Bezier
{
    public static Vector3[] InterpolateCurve(Vector3 start, Vector3 end, Vector3 control, int resolution)
    {
        // Calculate the total distance between start and end points
        float distance = Vector3.Distance(start, end);

        // Calculate the step size based on the resolution and distance
        float stepSize = distance / resolution;

        // Create an array to hold the points on the curve
        Vector3[] points = new Vector3[resolution + 1];

        // Calculate the first point on the curve
        points[0] = start;

        // Calculate the remaining points on the curve
        for (int i = 1; i <= resolution; i++)
        {
            // Calculate the percentage of the distance covered so far
            float percent = i / (float)resolution;

            // Calculate the position of the current point using a parameterized control point
            Vector3 position = Mathf.Pow(1 - percent, 2) * start + 2 * (1 - percent) * percent * control + Mathf.Pow(percent, 2) * end;

            // Add the current point to the array
            points[i] = position;
        }

        return points;
    }
}