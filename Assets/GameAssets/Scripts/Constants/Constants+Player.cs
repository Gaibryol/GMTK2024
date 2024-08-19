using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constants
{
	public class Player
	{
		public static readonly Vector2 RaycastDistance = new Vector2(0.515f, 0.515f);
		public const float DiagonalRaycastDistance = 0.72831998462f;
		public const float BoneRaycastDistance = 1.5f;
		public const float BoneDownRaycastDistance = 1f;
		public const float CirclecastRadius = 0.25f;

		public enum Inputs { A, D, None };

		public enum Orientations { Flat, WallOnRight, WallOnLeft };
	}
}