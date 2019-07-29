using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.World;
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
			return this.Data.CurrentSessionedWorldId != WorldHelpers.GetUniqueIdForCurrentWorld(true);
		}

		public bool HasWorldEverBeenPlayed( string worldId ) {
			return this.Data.AllPlayedWorlds.Contains( worldId );
		}


		////////////////

		public void BeginResetTimer() {
			var mymod = ResetModeMod.Instance;
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert();
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
			string worldId = WorldHelpers.GetUniqueIdForCurrentWorld(true);

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "Sets AllPlayedWorlds.Add(<world id>), CurrentSessionedWorldId=<world id>, AwaitingNextWorld=false (worldId: "+worldId+")" );
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
				LogHelpers.Alert( "Sets AwaitingNextWorld=true, CurrentSessionedWorldId=\"\", PlayersValidated.Clear()" );
			}
			
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
				this.GoodExit();
			}
		}


		public void ResetCurrentWorldForSession() {
			this.Data.PlayersValidated.Clear();
			this.Data.CurrentSessionedWorldId = "";

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void ClearAllWorlds() {
			if( ResetModeMod.Instance.Config.DebugModeInfo ) {
				LogHelpers.Alert( "Deletes all world files, Sets PlayersValidated.Clear(), CurrentSessionedWorldId=\"\", AwaitingNextWorld=true" );
			}

			try {
				Main.LoadWorlds();

				while( Main.WorldList.Count > 0 ) {
					WorldFileData worldData = Main.WorldList[0];
					WorldFileHelpers.EraseWorld( worldData, false );
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
