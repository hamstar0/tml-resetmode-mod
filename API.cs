using HamstarHelpers.MiscHelpers;
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

			return ResetModeMod.Instance.Session;
		}

		public static void SaveSessionDataChanges() {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			var mymod = ResetModeMod.Instance;

			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, ResetModeSessionData.DataFileNameOnly, mymod.Session );
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
