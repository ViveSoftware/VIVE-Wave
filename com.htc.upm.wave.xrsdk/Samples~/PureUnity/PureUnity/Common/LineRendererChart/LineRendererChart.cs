using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Wave.XR.Sample.Chart
{
	// Only for float.
	public class ChartData
	{
		private List<float> data = new List<float>();

		public int dataCount { get; private set; }

		// All the count in history.  You can reset it by Reset() or ResetAverage().
		public float OverTimeCount { get; private set; }

		// The running average in the data of capacity.  You can start to find a new average by ResetAverage().
		public float Average { get; private set; }

		// The average since start counting.  You can start to find a new average by ResetAverage().
		public float OverTimeAverage { get; private set; }

		// The Max value in the data of capacity.
		public float Max
		{
			get
			{
				float max = data[0];
				for (int i = 0; i < data.Count; i++)
					if (data[i] > max)
						max = data[i];

				return max;
			}
		}
		// The Min value in the data of capacity.
		public float Min
		{
			get
			{
				float min = data[0];
				for (int i = 0; i < data.Count; i++)
					if (data[i] < min)
						min = data[i];
				return min;
			}
		}

		// The Max value since start counting.  You can start to find a new Max by ResetMax().
		public float OverTimeMax { get; private set; }
		// The Min value since start counting0  You can start to find a new Min by ResetMax().
		public float OverTimeMin { get; private set; }
		// The Max value since start counting.  You can start to find a new Max by ResetMax().

		public float currentValue { get; private set; }

		public int updateCount = 0;

		private ChartData() { }
		public ChartData(uint count)
		{
			ResetDataCount(count);
			Reset();
		}

		public void ResetDataCount(uint count)
		{
			// I think this is big enough
			if (count > int.MaxValue / 2)
				count = int.MaxValue / 2;
			dataCount = (int)count;
		}

		public void Reset()
		{
			OverTimeMax = float.NaN;
			OverTimeMin = float.NaN;
			Average = float.NaN;
			OverTimeAverage = float.NaN;
			currentValue = float.NaN;
			OverTimeCount = 0;
			updateCount = 0;
			data.Clear();
		}

		public void ResetMax()
		{
			OverTimeMax = float.NaN;
			updateCount = 0;
		}

		public void ResetMin()
		{
			OverTimeMin = float.NaN;
			updateCount = 0;
		}

		public void ResetAverage()
		{
			Average = float.NaN;
			OverTimeAverage = float.NaN;
			OverTimeCount = 0;
			updateCount = 0;
			data.Clear();
		}

		void UpdateMaxAndMin(float value)
		{
			if (float.IsNaN(OverTimeMax) || value > OverTimeMax)
				OverTimeMax = value;
			if (float.IsNaN(OverTimeMin) || value < OverTimeMin)
				OverTimeMin = value;

			//if (!float.IsNaN(MaxCap) && OverTimeMax > MaxCap)
			//	OverTimeMax = MaxCap;
			//if (!float.IsNaN(MinCap) && OverTimeMin < MinCap)
			//	OverTimeMin = MinCap;

			updateCount++;
		}

		void UpdateAverage(float value, float dropped)
		{
			int c = data.Count;
			// Following code equals to "Average = (Average * size - dropped + value) / size"
			if (float.IsNaN(Average))
				Average = value;
			else if (float.IsNaN(dropped))
				Average = Average + (value - Average) / c;  // no drop. Count increase.
			else
				Average = Average + (value - dropped) / c;

			// Note: OverTimeCount can be increased by 1 only here.
			OverTimeCount++;
			// Following code equals to "newAvg = (oldAvg * (newSize - 1) + newValue) / newSize"
			OverTimeAverage = OverTimeAverage + (value - OverTimeAverage) / OverTimeCount;
		}

		float UpdateData(float value)
		{
			data.Add(value);

			float drop = float.NaN;
			if (data.Count > dataCount)
			{
				drop = data[0];
				data.RemoveAt(0);
			}
			return drop;
		}

		public void Add(float value)
		{
			currentValue = value;

			UpdateMaxAndMin(value);
			var dropped = UpdateData(value);
			UpdateAverage(value, dropped);
		}

		public int Count
		{
			get
			{
				return data.Count;
			}
		}

		// If you need resample the output count of your data, use this function.
		public List<float> GetSampledData(int count)
		{
			throw new NotImplementedException();
		}

		public List<float> GetData()
		{
			return data;
		}
	}

	public class LineRendererChart
	{
		string name;
		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
				StatusColoredFormat = GenerateStatusColoredFormat();
			}
		}

		private int dataCount;
		public ChartData chartData;
		public LineRenderer lineRenderer;
		GameObject obj = null;  // developer can access this obj by lineRenderer.transform.gameobject

		// How to present
		public enum Mode
		{
			// Data will be normalized by OverTimeMax value.
			LimitToOverTimeMax,
			// Data will be limited by MaxCap and MinCap value.  If not set the MaxCap and MinCap, mode will fallback to LimitToOverTimeMax.
			LimitToCap,
			// Data will be normalized by max value.
			LimitToMax,
		}

		// The hard limit value higher bound.  If set, when value, Max and OverTimeMax are larger than MaxCap, they are limited to MaxCap.
		public float MaxCap { get; set; }  // TODO Need validate.  Check if MaxCap > MinCap.
		// The hard limit value lower bound.  If set, when value, Min and OverTimeMin are smaller than MinCap, they are limited to MinCap.
		public float MinCap { get; set; }  // TODO Need validate.  Check if MaxCap > MinCap.


		// Origin point (0, 0)'s space offset in object's local space.  Default is (-0.5f, 0, 0).
		public Vector3 Offset { get; set; }

		// For example, "0.000" for "0.123", or "0" for "1234".
		string dataPrecisionFormat;
		public string DataPrecisionFormat
		{
			get
			{
				return dataPrecisionFormat;
			}
			set
			{
				dataPrecisionFormat = value;
				StatusColoredFormat = GenerateStatusColoredFormat();
			}
		}
		public string StatusColoredFormat { get; private set; }

		public LineRendererChart(string name, uint count)
		{
			ResetDataCount(count);
			obj = new GameObject(name);
			var renderer = obj.AddComponent<LineRenderer>();
			renderer.material = new Material(Shader.Find("Sprites/Default"));
			Init(name, new ChartData((uint)dataCount), renderer);
		}

		public LineRendererChart(string name, ChartData data, LineRenderer renderer, uint count)
		{
			ResetDataCount(count);
			Init(name, data, renderer);
		}

		public LineRendererChart(string name, LineRenderer renderer, uint count)
		{
			ResetDataCount(count);
			Init(name, new ChartData((uint)dataCount), renderer);
		}

		void Init(string name, ChartData data, LineRenderer renderer)
		{
			this.name = name;
			chartData = data;
			lineRenderer = renderer;
			dataPrecisionFormat = "0.000";

			lineRenderer.loop = false;
			lineRenderer.useWorldSpace = false;
			lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			lineRenderer.receiveShadows = false;
			lineRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
			lineRenderer.alignment = LineAlignment.TransformZ;
			lineRenderer.startWidth = 0.01f;
			lineRenderer.endWidth = 0.01f;
			lineRenderer.startColor = Color.white;
			lineRenderer.endColor = Color.white;

			Offset = new Vector3(-0.5f, 0, 0);

			StatusColoredFormat = GenerateStatusColoredFormat();

			MaxCap = float.NaN;
			MinCap = float.NaN;
		}

		void ResetDataCount(uint count)
		{
			// I think this is big enough
			if (count > int.MaxValue / 2)
				count = int.MaxValue / 2;
			dataCount = (int)count;
		}

		public ChartData data
		{
			get {
				return chartData;
			}
		}

		public LineRenderer renderer
		{
			get
			{
				return lineRenderer;
			}
		}

		public void SetColor(Color color)
		{
			lineRenderer.startColor = color;
			lineRenderer.endColor = color;
			StatusColoredFormat = GenerateStatusColoredFormat();
		}

		public string GenerateStatusColoredFormat()
		{
			return "<color=#" + ColorUtility.ToHtmlStringRGB(lineRenderer.endColor) + ">" + Name + "</color> = {0:" + dataPrecisionFormat + "}";
		}

		public void AddValue(float value)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
				chartData.Add(0);
			else
				chartData.Add(value);
		}

		public float NaNToZero(float value)
		{
			if (float.IsNaN(value) || float.IsInfinity(value))
				return 0;
			return value;
		}

		public void UpdateLineRenderer(Mode mode = Mode.LimitToCap)
		{
			if (mode == Mode.LimitToCap)
			{
				if (float.IsNaN(MaxCap) || float.IsNaN(MinCap))
					mode = Mode.LimitToOverTimeMax;
			}

			Vector3[] positions = new Vector3[dataCount * 3];

			// Get data
			float[] chart = chartData.GetData().ToArray();

			// Process data
			float max = 1;
			bool needClamp = false;
			float minCap = MinCap, maxCap = MaxCap;
			switch (mode)
			{
				case Mode.LimitToMax:
					// This mode cost more CPU time
					max = chartData.Max;
					break;
				case Mode.LimitToOverTimeMax:
					max = chartData.OverTimeMax;
					break;
				case Mode.LimitToCap:
					max = 1;
					needClamp = true;
					break;
			}
			int N = chart.Length;
			for (int i = 0; i < N; i++)
			{
				float value;
				if (max != 1)
					value = chart[i] / max;
				else
					value = chart[i];

				if (needClamp)
					value = (Mathf.Clamp(value, minCap, maxCap) - minCap) / (maxCap - minCap);
				chart[i] = value;
			}

			float step = 1.0f / dataCount;

			// When data is not full.  Skip some point.
			int skipIndex = 0;
			if (chartData.Count < dataCount)
				skipIndex = dataCount - chartData.Count;
			for (int i = 0; i < skipIndex; i++)
			{
				int j = i * 3;
				Vector3 point = new Vector3(step * i, 0, 0) + Offset;
				positions[j] = point;
				positions[j + 1] = point;
				positions[j + 2] = point;
			}

			Vector3 lastPoint = Vector3.zero;
			bool needInitial = true;
			for (int i = skipIndex; i < dataCount; i++)
			{
				int j = i * 3;

				// Current point
				Vector3 point = new Vector3(step * i, NaNToZero(chart[i - skipIndex]), 0) + Offset;

				// Last point
				if (needInitial)
				{
					needInitial = false;
					lastPoint = point;
				}

				// Next point
				Vector3 nextPoint = Vector3.zero;
				int nextPointIndex = i + 1;
				if (nextPointIndex == dataCount)
					nextPointIndex = i;
				nextPoint = new Vector3(step * nextPointIndex, NaNToZero(chart[nextPointIndex - skipIndex]), 0) + Offset;

				// Calculate Vector
				Vector3 vectorLast = (point - lastPoint) * 0.001f;
				Vector3 vectorNext = (nextPoint - point) * 0.001f;

				/**
				 * 1 & 3 are extra point.  2 is the value point.
				 * 
				 *       *                 *
				 *      * *               /
				 *     /   \     *-*     *
				 *    *     *   *   **-** 
				 *  **       **-*
				 *  123 123 123 23 123 123 1
				 *              1
				**/

				// Add extra point to keep line smooth
				positions[j] = point - vectorLast;
				positions[j + 1] = point;
				positions[j + 2] = point + vectorNext;

				lastPoint = point;
			}

			lineRenderer.positionCount = positions.Length;
			lineRenderer.SetPositions(positions);
		}
	}
}
