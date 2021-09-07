using UnityEngine;

namespace FG {
	public class WeightedTrackPiece
	{
		private GameObject _trackPrefab;
		
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
