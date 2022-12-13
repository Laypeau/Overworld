using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleFaces : MonoBehaviour
{
	[SerializeField] private Mesh sourceMesh = default;
	[SerializeField] private ComputeShader computeShader = default;
	[SerializeField] private ComputeShader triToVert = default;
	[SerializeField] private Material material = default;
	[SerializeField] private float height = 1f;
	[SerializeField] private float animFrequency = 1f;

	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
	public struct SourceVertex
	{
		public Vector3 position;
		public Vector2 uv;
	}

	private bool initialised;
	private ComputeBuffer sourceVertBuffer;
	private ComputeBuffer sourceTriBuffer;
	private ComputeBuffer drawBuffer;
	private int kernelID;
	private int dispatchSize;
	private Bounds bounds;

	private ComputeBuffer argsBuffer;
	private int argsKernelID;

	private const int sourceVertStride = sizeof(float) * (3+2);
	private const int sourceTriStride = sizeof(int);
	private const int drawStride = sizeof(float) * (3 + ((3+2) * 3));
	private const int argsStride = sizeof(int) * 4;

	//https://answers.unity.com/questions/361275/cant-convert-bounds-from-world-coordinates-to-loca.html
	public Bounds TransformBounds(Bounds boundsOS)
	{
		Vector3 center = transform.TransformPoint(boundsOS.center);

		// transform the local extents' axes
		Vector3 extents = boundsOS.extents;
		Vector3 axisX = transform.TransformVector(extents.x, 0, 0);
		Vector3 axisY = transform.TransformVector(0, extents.y, 0);
		Vector3 axisZ = transform.TransformVector(0, 0, extents.z);

		// sum their absolute value to get the world extents
		extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
		extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
		extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

		return new Bounds { center = center, extents = extents };
	}

	void OnEnable()
	{
		if (initialised) OnDisable();
		initialised = true;

		Vector3[] positions = sourceMesh.vertices;
		Vector2[] uvs = sourceMesh.uv;
		int[] tris = sourceMesh.triangles;

		SourceVertex[] vertices = new SourceVertex[positions.Length];
		for (int i = 0; i < positions.Length; i++)
		{
			vertices[i] = new SourceVertex() { position = positions[i], uv = uvs[i] };
		}

		int numTriangles = tris.Length / 3;

		sourceVertBuffer = new ComputeBuffer(vertices.Length, sourceVertStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
		sourceVertBuffer.SetData(vertices);
		sourceTriBuffer = new ComputeBuffer(tris.Length, sourceTriStride, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
		sourceTriBuffer.SetData(tris);
		drawBuffer = new ComputeBuffer(numTriangles * 3, drawStride, ComputeBufferType.Append);
		drawBuffer.SetCounterValue(0);

		kernelID = computeShader.FindKernel("Main");
		computeShader.SetBuffer(kernelID, "verticesIn", sourceVertBuffer);
		computeShader.SetBuffer(kernelID, "trianglesIn", sourceTriBuffer);
		computeShader.SetBuffer(kernelID, "trianglesOut", drawBuffer);
		computeShader.SetInt("numSourceTriangles", numTriangles);

		argsBuffer = new ComputeBuffer(1, argsStride, ComputeBufferType.IndirectArguments);
		argsBuffer.SetData(new int[] {0, 1, 0, 0});

		argsKernelID = triToVert.FindKernel("CSMain");
		triToVert.SetBuffer(argsKernelID, "indirectArgsBuffer", argsBuffer);

		material.SetBuffer("drawTriangles", drawBuffer);

		computeShader.GetKernelThreadGroupSizes(kernelID, out uint x, out uint _, out uint _);
		dispatchSize = Mathf.CeilToInt((float)numTriangles / x);


		bounds = sourceMesh.bounds;
		bounds.Expand(height);
	}

	void OnDisable()
	{
		if (initialised)
		{
			sourceVertBuffer.Release();
			sourceTriBuffer.Release();
			drawBuffer.Release();
			argsBuffer.Release();
		}
		initialised = false;
	}

	void LateUpdate()
	{
		drawBuffer.SetCounterValue(0);

		computeShader.SetMatrix("localToWorld", transform.localToWorldMatrix);
		computeShader.SetFloat("height", Mathf.Sin(animFrequency * Time.time) * height);

		computeShader.Dispatch(kernelID, dispatchSize, 1, 1);

		ComputeBuffer.CopyCount(drawBuffer, argsBuffer, 0);

		triToVert.Dispatch(argsKernelID, 1, 1, 1);

		Graphics.DrawProceduralIndirect(material, TransformBounds(bounds), MeshTopology.Triangles, argsBuffer, 0, null, null, UnityEngine.Rendering.ShadowCastingMode.On, true, gameObject.layer);

	}
}