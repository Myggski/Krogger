using UnityEngine;

namespace FG {
	[System.Serializable]
	public struct WeightedTrackPiece
	{
		[SerializeField]
		private GameObject _trackPrefab;
	
		[SerializeField]
		private int _weight;

		public GameObject TrackPrefab
		{
			get => _trackPrefab;
			set => _trackPrefab = value;
		}

		public int Weight
		{
			get => _weight;
			set => _weight = value;
		}

		public WeightedTrackPiece(GameObject trackPrefab, int weight)
		{
			this._trackPrefab = trackPrefab;
			this._weight = weight;
		}
	}
}
