﻿using HamstarHelpers.Utilities.Config;
using System;


namespace ResetMode.Data {
	public class ResetModeConfigData : ConfigurationDataBase {
		public static Version ConfigVersion { get { return new Version(1, 0, 1); } }
		public static string ConfigFileName { get { return "Reset Mode Config.json"; } }


		////////////////

		public string VersionSinceUpdate = ResetModeConfigData.ConfigVersion.ToString();
		
		public bool DebugModeInfo = false;

		public int SecondsUntilResetInitially = 60 * 45;
		public int SecondsUntilResetSubsequently = 60 * 30;



		////////////////
		
		public static readonly int _1_0_0_SecondsUntilResetInitially = 60 * 60 * 45;
		public static readonly int _1_0_0_SecondsUntilResetSubsequently = 60 * 60 * 30;

		////////////////

		public bool UpdateToLatestVersion() {
			var new_config = new ResetModeConfigData();

			var vers_since = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();

			if( vers_since >= ResetModeConfigData.ConfigVersion ) {
				return false;
			}
			if( vers_since < new Version( 1, 0, 1 ) ) {
				if( this.SecondsUntilResetInitially == ResetModeConfigData._1_0_0_SecondsUntilResetInitially ) {
					this.SecondsUntilResetInitially = new_config.SecondsUntilResetInitially;
				}
				if( this.SecondsUntilResetSubsequently == ResetModeConfigData._1_0_0_SecondsUntilResetSubsequently ) {
					this.SecondsUntilResetSubsequently = new_config.SecondsUntilResetSubsequently;
				}
			}

			this.VersionSinceUpdate = ResetModeConfigData.ConfigVersion.ToString();

			return true;
		}
	}
}
