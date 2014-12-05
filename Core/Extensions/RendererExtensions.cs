using UnityEngine;

namespace CraftingLegends.Core
{

	public static class RendererExtensions
	{
		/// <summary>
		/// adds a visibility (view frustum) check to a renderer component
		/// </summary>
		/// <param name="renderer"></param>
		/// <param name="camera"></param>
		/// <returns></returns>
		public static bool IsVisibleFrom(this Renderer renderer, Camera camera)
		{
			Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
			return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
		}
	}

}