using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.WorldHelpers;
using ResetMode.Data;
using System.Collections.Generic;
using Terraria;
using TimeLimit;


namespace ResetMode.Logic {
	class SessionLogic {
		public static string DataFileNameOnly { get { return "Session"; } }
		public static string RelativePath { get { return "Reset Mode Sessions"; } }


		////////////////

		public ResetModeSessionData Data { get; private set; }

		internal IDictionary<string, float> Rewards = new Dictionary<string, float>();


		////////////////

		internal SessionLogic() {
			this.Data = new ResetModeSessionData();
		}

		////////////////

		public void Load( ResetModeMod mymod ) {
			bool success;

			var data = DataFileHelpers.LoadJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, out success );
			if( success ) {
				this.Data = data;
			}
		}

		public void Save( ResetModeMod mymod ) {
			DataFileHelpers.SaveAsJson<ResetModeSessionData>( mymod, SessionLogic.DataFileNameOnly, this.Data );
		}


		////////////////
		
		public void EndCurrentSession( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			string world_id = WorldHelpers.GetUniqueId();   //Main.ActiveWorldFileData.UniqueId.ToString();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.EndCurrentSession " + world_id );
			}

			this.Data.ClearSessionData();

			this.Save( mymod );

			myworld.Logic.ResetForCurrentSession();

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void AddRewards( Player player, float rewards ) {
			if( Main.netMode != 2 ) { return; }

			bool success;
			string pid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) { return; }

			if( this.Rewards.ContainsKey( pid ) ) {
				this.Rewards[pid] += rewards;
			} else {
				this.Rewards[pid] = rewards;
			}
		}
	}
}
