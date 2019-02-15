using HamstarHelpers.Components.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace ResetMode.Data {
	public class ResetModeConfigData : ConfigurationDataBase {
		public static string ConfigFileName => "Reset Mode Config.json";


		////////////////

		public string VersionSinceUpdate = "";

		public bool DebugModeInfo = false;
		public bool DebugModeRealTimeInfo = false;

		public int SecondsUntilResetInitially = 60 * 45;	// 45 minutes
		public int SecondsUntilResetSubsequently = 60 * 30;	// 30 minutes

		public bool AutoStartSession = false;

		public bool DeleteAllWorldsBetweenGames = false;
		public bool WrongWorldForcesHardReset = false;

		public bool ResetRewardsSpendings = true;
		public bool ResetRewardsKills = false;

		public KeyValuePair<string, string[]>[] OnWorldEngagedCalls = new KeyValuePair<string, string[]>[0];



		////////////////

		public void SetDefaults() { }

		////

		public bool CanUpdateVersion() {
			if( this.VersionSinceUpdate == "" ) { return true; }
			var versSince = new Version( this.VersionSinceUpdate );
			return versSince < ResetModeMod.Instance.Version;
		}

		public void UpdateToLatestVersion() {
			var mymod = ResetModeMod.Instance;
			var newConfig = new ResetModeConfigData();
			newConfig.SetDefaults();

			var versSince = this.VersionSinceUpdate != "" ?
				new Version( this.VersionSinceUpdate ) :
				new Version();

			if( this.VersionSinceUpdate == "" ) {
				this.SetDefaults();
			}

			this.VersionSinceUpdate = mymod.Version.ToString();
		}


		////////////////

		public override string ToString() {
			return JsonConvert.SerializeObject( this );
		}
	}
}
