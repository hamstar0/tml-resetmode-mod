using ResetMode.Data;
using System;
using Terraria;


namespace ResetMode {
	public static partial class ResetModeAPI {
		public static ResetModeConfigData GetModSettings() {
			return ResetModeMod.Instance.Config;
		}
		
		public static void SaveModSettingsChanges() {
			ResetModeMod.Instance.ConfigJson.SaveFile();
		}

		public static ResetModeSessionData GetSessionData() {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			return ResetModeMod.Instance.Session.Data;
		}

		public static void SaveSessionDataChanges() {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			var mymod = ResetModeMod.Instance;
			mymod.Session.Save();
		}

		////////////////

		public static bool StartSession() {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			var mymod = ResetModeMod.Instance;
			return mymod.Session.StartSession();
		}

		public static bool StopSession() {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			var mymod = ResetModeMod.Instance;
			mymod.Session.EndSession();

			return true;
		}
	}
}
