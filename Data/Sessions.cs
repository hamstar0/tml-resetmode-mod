using HamstarHelpers.Utilities.Config;
using System.Collections.Generic;
using System.IO;


namespace ResetMode.Data {
	public class ResetModeSessionData {
		public static string DataFileName { get { return "Session.json"; } }
		public static string RelativePath { get { return "Reset Mode Sessions"; } }


		////////////////

		public IDictionary<string, int> PlayerHashes = new Dictionary<string, int>();
		public IDictionary<string, int> PlayerPP = new Dictionary<string, int>();

		public ISet<string> AllPlayedWorlds = new HashSet<string>();
		public bool AwaitingNextWorld = false;
		public bool IsRunning = false;



		////////////////

		public void ClearSession() {
			this.PlayerHashes = new Dictionary<string, int>();
			this.PlayerPP = new Dictionary<string, int>();
			this.AllPlayedWorlds = new HashSet<string>();
			this.AwaitingNextWorld = false;
		}

		public void EndSession() {
			this.ClearSession();
			this.IsRunning = false;
		}


		////////////////

		public void NetSend( BinaryWriter writer ) {
			string played_worlds = JsonConfig<ISet<string>>.Serialize( this.AllPlayedWorlds );

			writer.Write( (bool)this.IsRunning );
			writer.Write( (string)played_worlds );
		}

		public void NetReceive( BinaryReader reader ) {
			bool is_running = reader.ReadBoolean();
			var played_worlds = JsonConfig<ISet<string>>.Deserialize( reader.ReadString() );

			this.IsRunning = is_running;
			this.AllPlayedWorlds = played_worlds;
		}
	}
}
