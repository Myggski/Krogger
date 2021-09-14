namespace FG {
	using UnityEngine;

	[CreateAssetMenu(menuName = "Variables/IntVariable")]
	public class IntVariable : ScriptableObject
	{
		/// <summary>
		/// The int value that is being used
		/// </summary>
		public int Value { get; set; }
	}
}
