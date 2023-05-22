using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Thanks for the ideas and informations from 
 *	 https://answers.unity.com/questions/514293/changing-a-gameobjects-primitive-mesh.html
**/

namespace Wave.XR.Sample
{
	public class PrimitiveMeshHelper
	{
		private static Mesh primitiveMeshQuad = null;
		private static Mesh primitiveMeshCube = null;
		private static Mesh primitiveMeshSphere = null;
		private static Mesh primitiveMeshCapsule = null;
		private static Mesh primitiveMeshCylinder = null;
		private static Mesh primitiveMeshPlane = null;

		public static Mesh GetCachedBuiltinMesh(PrimitiveType type)
		{
			Mesh mesh = null;
			switch (type)
			{
				case PrimitiveType.Sphere:
					if (primitiveMeshSphere == null)
						primitiveMeshSphere = Resources.GetBuiltinResource<Mesh>("New-Sphere.fbx");
					mesh = primitiveMeshSphere;
					break;

				case PrimitiveType.Quad:
					if (primitiveMeshQuad == null)
						primitiveMeshQuad = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
					mesh = primitiveMeshQuad;
					break;

				case PrimitiveType.Cube:
					if (primitiveMeshCube == null)
						primitiveMeshCube = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
					mesh = primitiveMeshCube;
					break;

				case PrimitiveType.Cylinder:
					if (primitiveMeshCylinder == null)
						primitiveMeshCylinder = Resources.GetBuiltinResource<Mesh>("New-Cylinder.fbx");
					mesh = primitiveMeshCylinder;
					break;

				case PrimitiveType.Capsule:
					if (primitiveMeshCapsule == null)
						primitiveMeshCapsule = Resources.GetBuiltinResource<Mesh>("New-Capsule.fbx");
					mesh = primitiveMeshCapsule;
					break;

				case PrimitiveType.Plane:
					if (primitiveMeshPlane == null)
						primitiveMeshPlane = Resources.GetBuiltinResource<Mesh>("New-Plane.fbx");
					mesh = primitiveMeshPlane;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			return mesh;
		}
	}
}
