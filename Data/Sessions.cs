using System.Collections.Generic;


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
	}
}
