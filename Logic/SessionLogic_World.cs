using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
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
			if( this.IsSessionNeedingWorld() ) {
				this.AddWorldToSession( mymod );    // Changes world status
			} else if( this.IsSessionedWorldNotOurs() ) {
				if( !this.IsExiting ) {
					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Log( "ResetMode.SessionLogic.UpdateSessionWorld - World has expired. World id: " + WorldHelpers.GetUniqueIdWithSeed() );
					}

					this.GoodExit( mymod );
				}
			}
		}


		////////////////

		public bool IsSessionNeedingWorld() {
			return string.IsNullOrEmpty( this.SessionData.CurrentSessionedWorldId );
		}

		public bool IsSessionedWorldNotOurs() {
			return this.SessionData.CurrentSessionedWorldId != WorldHelpers.GetUniqueIdWithSeed();
		}

		public bool HasWorldEverBeenPlayed( string world_id ) {
			return !this.SessionData.AllPlayedWorlds.Contains( world_id );
		}


		////////////////

		public void AddWorldToSession( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueIdWithSeed();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.SessionLogic.AddWorldToSession - World ID: " + world_id );
			}
			
			if( !this.SessionData.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}
			
			this.SessionData.AllPlayedWorlds.Add( world_id );
			this.SessionData.CurrentSessionedWorldId = world_id;
			this.SessionData.AwaitingNextWorld = false;
			this.Save( mymod );

			this.RunModCalls( mymod );
		}


		public void ExpireCurrentWorldInSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.SessionLogic.ExpireWorldInSession" );
			}

			this.SessionData.AwaitingNextWorld = true;
			this.SessionData.CurrentSessionedWorldId = "";
			this.SessionData.PlayersValidated.Clear();
			this.Save( mymod );

			this.GoodExit( mymod );
		}


		public void ResetCurrentWorldForSession( ResetModeMod mymod ) {
			this.SessionData.PlayersValidated.Clear();
			this.SessionData.CurrentSessionedWorldId = "";

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
				this.SessionData.CurrentSessionedWorldId = "";
				this.SessionData.AwaitingNextWorld = true;

				this.Save( mymod );
			} catch( Exception e ) {
				LogHelpers.Log( e.ToString() );
			}
		}
	}
}
