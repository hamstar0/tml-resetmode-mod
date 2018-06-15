using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
using ResetMode.Data;
using System;
using Terraria;
using Terraria.IO;
using TimeLimit;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public static bool IsWorldUidInSession( ResetModeMod mymod, string world_uid ) {
			return mymod.Session.Data.AllPlayedWorlds.Contains( world_uid );
		}



		////////////////

		internal void UpdateSessionWorldSingle( ResetModeMod mymod ) {
			this.UpdateSessionWorld( mymod );
		}

		internal void UpdateSessionWorldClient( ResetModeMod mymod ) {
		}

		internal void UpdateSessionWorldServer( ResetModeMod mymod ) {
			this.UpdateSessionWorld( mymod );
		}


		private void UpdateSessionWorld( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			switch( myworld.Data.WorldStatus ) {
			case ResetModeStatus.Normal:
				this.AddWorldToSession( mymod );
				break;

			case ResetModeStatus.Active:
				break;

			case ResetModeStatus.Expired:
				if( !myworld.Data.IsExiting ) {
					this.GoodExit( mymod );
				}
				break;
			}
		}


		////////////////

		public void AddWorldToSession( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueIdWithSeed();
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.EngageForCurrentSession " + world_id );
			}

			if( !this.Data.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}

			this.Data.AllPlayedWorlds.Add( world_id );
			this.Data.AwaitingNextWorld = false;
			this.Save( mymod );

			myworld.Data.WorldStatus = ResetModeStatus.Active;

			this.RunModCalls( mymod );
		}


		public void ExpireWorldInSession( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.ExpireForCurrentSession" );
			}

			this.Data.AwaitingNextWorld = true;
			this.Save( mymod );

			myworld.Data.WorldStatus = ResetModeStatus.Expired;

			this.GoodExit( mymod );
		}


		public void ResetWorldForSession( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			myworld.Data.WorldPlayers.Clear();
			myworld.Data.WorldStatus = ResetModeStatus.Normal;

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void ClearAllWorlds() {
			var mymod = ResetModeMod.Instance;

			try {
				Main.LoadWorlds();

				while( Main.WorldList.Count > 0 ) {
					WorldFileData world_data = Main.WorldList[0];
					WorldFileHelpers.EraseWorld( world_data, false );
				}

				this.Data.AwaitingNextWorld = true;
				this.Save( mymod );
			} catch( Exception e ) {
				LogHelpers.Log( e.ToString() );
			}
		}
	}
}
