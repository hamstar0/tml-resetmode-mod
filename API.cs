﻿using ResetMode.Data;
using System;


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
			return ResetModeMod.Instance.Session;
		}

		public static void SaveSessionDataChanges() {
			ResetModeMod.Instance.SessionJson.SaveFile();
		}

		////////////////

		public static void StartSession() {
			var mymod = ResetModeMod.Instance;

			if( !mymod.Logic.IsSessionStarted( mymod ) ) {
				var myworld = mymod.GetModWorld<ResetModeWorld>();

				mymod.Logic.StartSession( mymod );
				myworld.Logic.EngageWorldForCurrentSession( mymod );
			}
		}

		public static void StopSession() {
			var mymod = ResetModeMod.Instance;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			mymod.Logic.StopSession( mymod );
			myworld.Logic.ClearSessionWorlds( mymod );
		}
	}
}
