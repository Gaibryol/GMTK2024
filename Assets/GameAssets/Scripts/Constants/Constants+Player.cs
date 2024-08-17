using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constants
{
	public class Player
	{
		public static readonly Vector2 RaycastDistance = new Vector2(0.525f, 0.525f);
		public const float DiagonalRaycastDistance = 0.74246212024f;
		public const float DropDuration = 0.5f;

		public enum Inputs { A, D, None };
		public enum Directions { Up, Down, Left, Right, None };
	}
}