﻿using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
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
			return !this.Data.AllPlayedWorlds.Contains( world_id );
		}


		////////////////

		public void BeginResetTimer( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.SessionLogic.BeginResetTimer" );
			}
			
			if( TimeLimitAPI.GetTimersOf( "reset" ).Count > 0 ) {
				LogHelpers.Log( "ResetMode.SessionLogic.BeginResetTimer - Existing reset timers halted." );
				Main.NewText( "Warning: Existing reset timers removed." );
			}
			TimeLimitAPI.TimerStop( "reset" );	// Stop regardless? API failure perhaps?

			if( !this.Data.AwaitingNextWorld ) {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetInitially, false );
			} else {
				TimeLimitAPI.TimerStart( "reset", mymod.Config.SecondsUntilResetSubsequently, false );
			}
		}
			
		public void AddWorldToSession( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueIdWithSeed();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.SessionLogic.AddWorldToSession - World ID: " + world_id );
			}

			this.Data.AllPlayedWorlds.Add( world_id );
			this.Data.CurrentSessionedWorldId = world_id;
			this.Data.AwaitingNextWorld = false;
			this.Save( mymod );

			this.RunModCalls( mymod );
		}


		public void ExpireCurrentWorldInSession( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.SessionLogic.ExpireWorldInSession" );
			}

			this.Data.AwaitingNextWorld = true;
			this.Data.CurrentSessionedWorldId = "";
			this.Data.PlayersValidated.Clear();
			this.Save( mymod );

			this.GoodExit( mymod );
		}


		public void ResetCurrentWorldForSession( ResetModeMod mymod ) {
			this.Data.PlayersValidated.Clear();
			this.Data.CurrentSessionedWorldId = "";

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

				this.Data.PlayersValidated.Clear();
				this.Data.CurrentSessionedWorldId = "";
				this.Data.AwaitingNextWorld = true;

				this.Save( mymod );
			} catch( Exception e ) {
				LogHelpers.Log( e.ToString() );
			}
		}
	}
}
