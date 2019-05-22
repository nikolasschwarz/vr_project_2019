﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class PaintChalk
{

	public int resolution;

	public void AddPoint(Vector3 point)
	{
		pointList.Add(point);
		if (pointList.Count < 2)
			return;

		if (pointList.Count == 2)
		{
			AddLinearBezierCurve(pointList[0], pointList[1]);
			return;
		}
		if (pointList.Count == 3)
		{
			lineRenderer.positionCount = 0;
			AddQuadraticBezierCurve(pointList[0], pointList[1], getFirstSupportVec(pointList, 1));
			AddQuadraticBezierCurve(pointList[1], pointList[2], getSecondSupportVec(pointList, 1));
			return;
		}
		lineRenderer.positionCount -= resolution + 1;
		AddCubicBezierCurve(pointList[pointList.Count - 3], getSecondSupportVec(pointList, pointList.Count - 3), pointList[pointList.Count - 2], getFirstSupportVec(pointList, pointList.Count - 2));
		AddQuadraticBezierCurve(pointList[pointList.Count - 2], pointList[pointList.Count - 1], getSecondSupportVec(pointList, pointList.Count - 2));
	}

	Tuple<Vector3, Vector3> getSupportVecs(Vector3[] points, int index)
	{
		if (index <= 0 || index >= points.Length - 1)
		{
			throw new ArgumentException("illegal index");
		}

		Vector3 prevToPos = points[index] - points[index - 1];
		float prevToPosLength = prevToPos.magnitude;
		prevToPos.Normalize();
		Vector3 posToNext = points[index + 1] - points[index];
		float posToNextLength = posToNext.magnitude;
		posToNext.Normalize();
		Vector3 tangent = prevToPos + posToNext;
		tangent.Normalize();

		return new Tuple<Vector3, Vector3>(points[index] - tangent / 3 * prevToPosLength, points[index] + tangent / 3 * posToNextLength);
	}

	Vector3 getFirstSupportVec(List<Vector3> points, int index)
	{
		if (index <= 0 || index > points.Count - 1)
		{
			throw new ArgumentException("illegal index");
		}

		Vector3 prevToPos = points[index] - points[index - 1];
		float prevToPosLength = prevToPos.magnitude;
		Vector3 tangent = getTangent(points, index);

		return points[index] - tangent / 3 * prevToPosLength;
	}

	Vector3 getSecondSupportVec(List<Vector3> points, int index)
	{
		if (index < 0 || index >= points.Count - 1)
		{
			throw new ArgumentException("illegal index");
		}

		Vector3 posToNext = points[index + 1] - points[index];
		float posToNextLength = posToNext.magnitude;
		Vector3 tangent = getTangent(points, index);

		return points[index] + tangent / 3 * posToNextLength;
	}

	Vector3 getTangent(List<Vector3> points, int index)
	{
		if(index != 0)
		{
			if (index != points.Count - 1)
			{
				Vector3 prevToPos = points[index] - points[index - 1];
				prevToPos.Normalize();
				Vector3 posToNext = points[index + 1] - points[index];
				posToNext.Normalize();
				Vector3 tangent = prevToPos + posToNext;
				tangent.Normalize();
				return tangent;
			}
			else
			{
				Vector3 prevToPos = points[index] - points[index - 1];
				prevToPos.Normalize();
				return prevToPos;
			}
		}
		else
		{
			Vector3 posToNext = points[index + 1] - points[index];
			posToNext.Normalize();
			return posToNext;
		}
	}


	void AddLinearBezierCurve(Vector3 a, Vector3 b)
	{
		int oldPostionCount = lineRenderer.positionCount;
		lineRenderer.positionCount += 2;
		lineRenderer.SetPosition(oldPostionCount, a);
		lineRenderer.SetPosition(oldPostionCount + 1, b);
	}

	void AddQuadraticBezierCurve(Vector3 a, Vector3 b, Vector3 supp)
	{
		int oldPostionCount = lineRenderer.positionCount;
		lineRenderer.positionCount += resolution + 1;

		float inc = 1.0f / this.resolution;
		int i = oldPostionCount;
		for (float t = 0; Math.Round(t, 6) <= 1; t += inc)
		{
			lineRenderer.SetPosition(i++, GetQuadraticBezierPoint(a, b, supp, t));
		}
	}

	Vector3 GetQuadraticBezierPoint(Vector3 a, Vector3 b, Vector3 supp, float t)
	{
		return Mathf.Pow(1 - t, 2) * a + 2 * (1 - t) * t * supp + Mathf.Pow(t, 2) * b;
	}


	void AddCubicBezierCurve(Vector3 a, Vector3 aSupp, Vector3 b, Vector3 bSupp)
	{
		int oldPostionCount = lineRenderer.positionCount;
		lineRenderer.positionCount += resolution + 1;

		float inc = 1.0f / this.resolution;
		int i = oldPostionCount;
		for (float t = 0; Math.Round(t, 6) <=1; t += inc)
		{
			lineRenderer.SetPosition(i++, GetCubicBezierPoint(a, aSupp, b, bSupp, t));
		}
	}

	Vector3 GetCubicBezierPoint(Vector3 a, Vector3 aSupp, Vector3 b, Vector3 bSupp, float t)
	{
		return Mathf.Pow(1-t, 3) * a + 3 * Mathf.Pow(1-t,2) * t * aSupp + 3 * (1 - t) * Mathf.Pow(t, 2) * bSupp + Mathf.Pow(t, 3) * b;
	}


	private LineRenderer lineRenderer;

	private List<Vector3> pointList;

	public PaintChalk()
	{
		lineRenderer = new LineRenderer();
		//lineRenderer.transform.rotation.Set(90, 0, 0, 0);
		//lineRenderer.alignment = LineAlignment.TransformZ;
		pointList = new List<Vector3>();
	}

}
