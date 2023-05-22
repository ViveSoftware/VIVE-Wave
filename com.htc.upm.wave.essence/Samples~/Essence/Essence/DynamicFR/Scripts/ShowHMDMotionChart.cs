using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Wave.XR.Sample.Chart;

public class ShowHMDMotionChart : MonoBehaviour
{
	DynamicFR dynamicFR;
	public Text status;

	enum ChartName
	{
		AngularVelocity = 0,
		Velocity,
		FPS,
		Quality,
		FOV,
		ResolutionScale,
		ViewportScale,
		TotalCount,
	}

	Color[] chartColors = new Color[]
	{
		Color.green,
		Color.blue,
		Color.red,
		Color.magenta,
		Color.yellow,
		Color.white * 0.85f,
		Color.white,
	};

	List<LineRendererChart> graph = new List<LineRendererChart>();

	[SerializeField]
	private int dataCount = 100;

	void Start()
	{
		for (int i = 0; i < (int)ChartName.TotalCount; i++)
		{
			var chart = new LineRendererChart(((ChartName)i).ToString(), (uint)dataCount);
			chart.lineRenderer.transform.SetParent(transform, false);
			chart.lineRenderer.transform.localPosition = Vector3.zero;
			chart.lineRenderer.transform.localRotation = Quaternion.identity;
			chart.lineRenderer.transform.localScale = Vector3.one;
			chart.Offset = new Vector3(-0.5f, -0.5f, 0.1f * i);
			chart.SetColor(chartColors[i]);
			graph.Add(chart);
		}

		graph[(int)ChartName.AngularVelocity].MaxCap = 5;
		graph[(int)ChartName.AngularVelocity].MinCap = 0;
		graph[(int)ChartName.Velocity].MaxCap = 5;
		graph[(int)ChartName.Velocity].MinCap = 0;

		graph[(int)ChartName.FPS].DataPrecisionFormat = "0.0";
		graph[(int)ChartName.FOV].DataPrecisionFormat = "0";
		graph[(int)ChartName.Quality].DataPrecisionFormat = "0";
		graph[(int)ChartName.ResolutionScale].DataPrecisionFormat = "0.00";
		graph[(int)ChartName.ViewportScale].DataPrecisionFormat = "0.00";

		dynamicFR = GetComponent<DynamicFR>();
	}

	void Update()
	{
		var chartAngularVelocity = graph[(int)ChartName.AngularVelocity];
		var chartVelocity = graph[(int)ChartName.Velocity];
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			// Fake data for AngularVelocity and velocity
			for (int i = 0; i < 2; i++)
			{
				float s = Mathf.Sin(Time.fixedUnscaledTime / (i + 1));  // i + 1 avoid NaN
				Vector3 vel = new Vector3(Random.Range(-1.0f, 1.0f) * s, Random.Range(-1.0f, 1.0f) * s, Random.Range(-1.0f, 1.0f) * s);
				graph[i].AddValue(vel.magnitude);
			}
		}
		else
#endif
		{
			graph[(int)ChartName.AngularVelocity].AddValue(dynamicFR.AngularVelocity);
			graph[(int)ChartName.Velocity].AddValue(dynamicFR.Velocity);
		}

		graph[(int)ChartName.ResolutionScale].AddValue(dynamicFR.RS);
		graph[(int)ChartName.ViewportScale].AddValue(dynamicFR.VS);
		graph[(int)ChartName.Quality].AddValue((float)dynamicFR.Quality);
		graph[(int)ChartName.FOV].AddValue(dynamicFR.FOV);
		graph[(int)ChartName.FPS].AddValue(1.0f / Time.unscaledDeltaTime);

		// update status
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < (int)ChartName.TotalCount; i++)
		{
			if (i == (int)ChartName.FPS)
				sb.AppendFormat(graph[i].StatusColoredFormat, graph[i].data.Average).AppendLine();
			else
				sb.AppendFormat(graph[i].StatusColoredFormat, graph[i].data.currentValue).AppendLine();
		}
		status.text = sb.ToString();

		foreach (var chart in graph)
			chart.UpdateLineRenderer();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause == false)
		{
			foreach (var chart in graph)
				chart.data.Reset();
		}
	}

	private void OnDrawGizmos()
	{
		// The data will show in this area
		Gizmos.color = Color.gray;
		Gizmos.matrix = transform.localToWorldMatrix;
		float depth = 0.1f * (int)ChartName.TotalCount;
		Gizmos.DrawWireCube(new Vector3(0, 0, depth / 2), new Vector3(1, 1, depth));
	}
}
