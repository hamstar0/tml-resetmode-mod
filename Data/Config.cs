using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader.Config;


namespace ResetMode.Data {
	public class ResetModeConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////

		[DefaultValue( false )]
		public bool DebugModeInfo = false;
		[DefaultValue( false )]
		public bool DebugModeRealTimeInfo = false;

		[Range( 10, 60 * 60 * 24 * 7 * 52 )]
		[DefaultValue( 60 * 45 )]
		public int SecondsUntilResetInitially = 60 * 45;    // 45 minutes
		[Range( 10, 60 * 60 * 24 * 7 * 52 )]
		[DefaultValue( 60 * 30 )]
		public int SecondsUntilResetSubsequently = 60 * 30; // 30 minutes

		[DefaultValue( false )]
		public bool AutoStartSession = false;

		[DefaultValue( false )]
		public bool DeleteAllWorldsBetweenGames = false;
		[DefaultValue( false )]
		public bool WrongWorldForcesHardReset = false;

		[DefaultValue( true )]
		public bool ResetRewardsSpendings = true;
		[DefaultValue( false )]
		public bool ResetRewardsKills = false;

		public List<KeyValuePair<string, string[]>> OnWorldEngagedCalls = new List<KeyValuePair<string, string[]>>();



		////////////////

		public override ModConfig Clone() {
			var clone = (ResetModeConfig)base.Clone();

			clone.OnWorldEngagedCalls = new List<KeyValuePair<string, string[]>>( this.OnWorldEngagedCalls );

			return clone;
		}

		////////////////

		public override string ToString() {
			return JsonConvert.SerializeObject( this );
		}
	}
}
