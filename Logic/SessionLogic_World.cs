using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using ResetMode.NetProtocols;
using System;
using Terraria;
using Terraria.IO;
using TimeLimit;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public bool IsSessionNeedingWorld() {
			return string.IsNullOrEmpty( this.Data.CurrentSessionedWorldId );
		}

		public bool IsSessionedWorldNotOurs() {
			return this.Data.CurrentSessionedWorldId != WorldHelpers.GetUniqueIdWithSeed();
		}

		public bool HasWorldEverBeenPlayed( string world_id ) {
			return this.Data.AllPlayedWorlds.Contains( world_id );
		}


		////////////////

		public void BeginResetTimer() {
			var mymod = ResetModeMod.Instance;
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "" );
			}
			
			if( TimeLimitAPI.GetTimersOf( "reset" ).Count > 0 ) {
				LogHelpers.Alert( "Existing reset timers halted." );
				Main.NewText( "Warning: Existing reset timers removed." );
			}
			TimeLimitAPI.TimerStop( "reset" );	// Stop regardless? API failure perhaps?

			if( !this.Data.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}
		}
			
		public void AddWorldToSession() {
			var mymod = ResetModeMod.Instance;
			string worldId = WorldHelpers.GetUniqueIdWithSeed();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "World ID: " + worldId );
			}

			this.Data.AllPlayedWorlds.Add( worldId );
			this.Data.CurrentSessionedWorldId = worldId;
			this.Data.AwaitingNextWorld = false;
			if( Main.netMode != 1 ) {
				this.Save();
			}

			this.RunModCalls();
		}


		public void ExpireCurrentWorldInSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "" );
			}

try {
			this.Data.AwaitingNextWorld = true;
			this.Data.CurrentSessionedWorldId = "";
			this.Data.PlayersValidated.Clear();
			if( Main.netMode != 1 ) {
				this.Save();
			}

			if( Main.netMode == 2 ) {
				SessionProtocol.SyncToClients();
			}

			if( Main.netMode != 1 ) {
LogHelpers.Log("???");
				this.GoodExit();
			}
} catch( Exception e ) { LogHelpers.Log("?!? "+e.ToString()); }
		}


		public void ResetCurrentWorldForSession() {
			this.Data.PlayersValidated.Clear();
			this.Data.CurrentSessionedWorldId = "";

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void ClearAllWorlds() {
			try {
				Main.LoadWorlds();

				while( Main.WorldList.Count > 0 ) {
					WorldFileData world_data = Main.WorldList[0];
					WorldFileHelpers.EraseWorld( world_data, false );
				}

				this.Data.PlayersValidated.Clear();
				this.Data.CurrentSessionedWorldId = "";
				this.Data.AwaitingNextWorld = true;

				if( Main.netMode != 1 ) {
					this.Save();
				}
			} catch( Exception e ) {
				LogHelpers.Warn( e.ToString() );
			}
		}
	}
}
