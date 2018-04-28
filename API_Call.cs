using System;


namespace ResetMode {
	public static partial class ResetModeAPI {
		internal static object Call( string call_type, params object[] args ) {
			switch( call_type ) {
			case "GetModSettings":
				return ResetModeAPI.GetModSettings();
			case "SaveModSettingsChanges":
				ResetModeAPI.SaveModSettingsChanges();
				return null;
			case "GetSessionData":
				return ResetModeAPI.GetSessionData();
			case "SaveSessionDataChanges":
				ResetModeAPI.SaveSessionDataChanges();
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
	}
}
