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
			}
			
			throw new Exception("No such api call "+call_type);
		}



		////////////////

		public static ResetModeConfigData GetModSettings() {
			return ResetModeMod.Instance.Config;
		}
		
		public static void SaveModSettingsChanges() {
			ResetModeMod.Instance.JsonConfig.SaveFile();
		}
	}
}
