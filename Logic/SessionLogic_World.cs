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
			return mymod.Session.SessionData.AllPlayedWorlds.Contains( world_uid );
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
			switch( this.WorldData.WorldStatus ) {
			case ResetModeWorldStatus.Normal:
				this.AddWorldToSession( mymod );	// Changes world status
				break;

			case ResetModeWorldStatus.Active:
				break;

			case ResetModeWorldStatus.Expired:
				if( !this.WorldData.IsExiting ) {
					this.GoodExit( mymod );
				}
				break;
			}
		}


		////////////////

		public void AddWorldToSession( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueIdWithSeed();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.AddWorldToSession " + world_id );
			}
			
			if( !this.SessionData.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}

			this.SessionData.AllPlayedWorlds.Add( world_id );
			this.SessionData.CurrentWorld = world_id;
			this.SessionData.AwaitingNextWorld = false;
			this.Save( mymod );

			this.RunModCalls( mymod );
		}


		public void ExpireCurrentWorldInSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.ExpireWorldInSession" );
			}

			this.SessionData.AwaitingNextWorld = true;
			this.SessionData.CurrentWorld = "";
			this.SessionData.PlayersValidated.Clear();
			this.Save( mymod );

			this.GoodExit( mymod );
		}


		public void ResetCurrentWorldForSession( ResetModeMod mymod ) {
			this.SessionData.PlayersValidated.Clear();
			this.SessionData.CurrentWorld = "";

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

				this.SessionData.PlayersValidated.Clear();
				this.SessionData.CurrentWorld = "";
				this.SessionData.AwaitingNextWorld = true;

				this.Save( mymod );
			} catch( Exception e ) {
				LogHelpers.Log( e.ToString() );
			}
		}
	}
}
