using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Constants
{
	public class Player
	{
		public static readonly Vector2 RaycastDistance = new Vector2(1.5f, 0.5f);
		public const float DiagonalRaycastDistance = 1.5811388300841896659994467722164f;
		public const float DropDuration = 0.5f;

		public enum Inputs { A, D, None };
		public enum Directions { Up, Down, Left, Right, None };
	}
}