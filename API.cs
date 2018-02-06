using ResetMode.Data;
using System;
using Terraria;


namespace ResetMode {
	public static class ResetModeAPI {
		internal static object Call( string call_type, params object[] args ) {
			switch( call_type ) {
			case "GetModSettings":
				return ResetModeAPI.GetModSettings();
			case "SaveModSettingsChanges":
				ResetModeAPI.SaveModSettingsChanges();
				return null;
			case "StartSession":
				ResetModeAPI.StartSession();
				return null;
			case "StopSession":
				ResetModeAPI.StopSession();
				return null;
			}
			
			throw new Exception("No such api call "+call_type);
		}



		////////////////

		public static ResetModeConfigData GetModSettings() {
			return ResetModeMod.Instance.Config;
		}
		
		public static void SaveModSettingsChanges() {
			ResetModeMod.Instance.ConfigJson.SaveFile();
		}

		public static ResetModeSessionData GetSessionData() {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			return ResetModeMod.Instance.Session;
		}

		public static void SaveSessionDataChanges() {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			ResetModeMod.Instance.SessionJson.SaveFile();
		}

		////////////////

		public static bool StartSession() {
			if( Main.netMode == 1 ) { throw new Exception("Clients cannot call this."); }

			var mymod = ResetModeMod.Instance;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Session.IsRunning ) {
				return false;
			}

			myworld.Logic.EngageWorldForCurrentSession( mymod );

			return true;
		}

		public static bool StopSession() {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			var mymod = ResetModeMod.Instance;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( !mymod.Session.IsRunning ) {
				return false;
			}
			
			myworld.Logic.EndCurrentSession( mymod );

			return true;
		}
	}
}
