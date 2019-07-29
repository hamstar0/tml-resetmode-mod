using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria.ModLoader.Config;


namespace ResetMode.Data {
	public class ResetModeConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ServerSide;


		////

		public bool DebugModeInfo = false;
		public bool DebugModeRealTimeInfo = false;

		[DefaultValue( 60 * 45 )]
		public int SecondsUntilResetInitially = 60 * 45;    // 45 minutes
		[DefaultValue( 60 * 30 )]
		public int SecondsUntilResetSubsequently = 60 * 30;	// 30 minutes

		public bool AutoStartSession = false;

		public bool DeleteAllWorldsBetweenGames = false;
		public bool WrongWorldForcesHardReset = false;

		[DefaultValue( true )]
		public bool ResetRewardsSpendings = true;
		public bool ResetRewardsKills = false;

		public KeyValuePair<string, string[]>[] OnWorldEngagedCalls = new KeyValuePair<string, string[]>[0];



		////////////////


		[OnDeserialized]
		internal void OnDeserializedMethod( StreamingContext context ) {
			if( this.OnWorldEngagedCalls != null ) {
				return;
			}
			this.OnWorldEngagedCalls = new KeyValuePair<string, string[]>[0];
		}


		////////////////

		public override string ToString() {
			return JsonConvert.SerializeObject( this );
		}
	}
}
