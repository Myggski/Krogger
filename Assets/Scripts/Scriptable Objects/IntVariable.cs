namespace FG {
	using UnityEngine;

	[CreateAssetMenu(menuName = "Variables/IntVariable")]
	public class IntVariable : ScriptableObject
	{
		/// <summary>
		/// The float value that is being used
		/// </summary>
		public float Value { get; set; }
	}
}
