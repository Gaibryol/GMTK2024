using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constants
{
	public class Player
	{
		public const float RaycastDistance = 0.525f;
		public const float DiagonalRaycastDistance = 0.74246212024f;

		public enum Inputs { A, D, None };
		public enum Directions { Up, Down, Left, Right, None };
	}
}