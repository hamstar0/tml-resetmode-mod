using HamstarHelpers.Components.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace ResetMode.Data {
	public class ResetModeConfigData : ConfigurationDataBase {
		public static Version ConfigVersion => new Version( 1, 1, 2 );
		public static string ConfigFileName => "Reset Mode Config.json";


		////////////////

		public string VersionSinceUpdate = ResetModeConfigData.ConfigVersion.ToString();

		public bool DebugModeInfo = false;

		public int SecondsUntilResetInitially = 60 * 45;	// 45 minutes
		public int SecondsUntilResetSubsequently = 60 * 30;	// 30 minutes

		public bool AutoStartSession = false;

		public bool DeleteAllWorldsBetweenGames = false;
		public bool WrongWorldForcesHardReset = false;

		public bool ResetRewardsSpendings = true;
		public bool ResetRewardsKills = false;

		public KeyValuePair<string, string[]>[] OnWorldEngagedCalls = new KeyValuePair<string, string[]>[0];



		////////////////

		public bool UpdateToLatestVersion() {
			var newConfig = new ResetModeConfigData();

			var versSince = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();

			if( versSince >= ResetModeConfigData.ConfigVersion ) {
				return false;
			}

			this.VersionSinceUpdate = ResetModeConfigData.ConfigVersion.ToString();

			return true;
		}


		////////////////

		public override string ToString() {
			return JsonConvert.SerializeObject( this );
		}
	}
}
