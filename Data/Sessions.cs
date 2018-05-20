using System.Collections.Generic;


namespace ResetMode.Data {
	public class ResetModeSessionData {
		//public IDictionary<string, int> PlayerHashes = new Dictionary<string, int>();
		public IDictionary<string, float> PlayerPP = new Dictionary<string, float>();

		public ISet<string> AllPlayedWorlds = new HashSet<string>();
		public bool AwaitingNextWorld = false;
		public bool IsRunning = false;



		////////////////
		
		internal void AddActiveWorld( string world_id ) {
			this.AllPlayedWorlds.Add( world_id );
			this.AwaitingNextWorld = false;
			this.IsRunning = true;
		}

		internal void ClearWorldHistory() {
			this.AllPlayedWorlds.Clear();
		}

		internal void ClearSessionData() {
			this.ClearWorldHistory();
			this.PlayerPP.Clear();
			this.AwaitingNextWorld = false;
			this.IsRunning = false;
		}
	}
}
