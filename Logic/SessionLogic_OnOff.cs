using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using ResetMode.NetProtocols;
using System;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public bool StartSession() {
			var mymod = ResetModeMod.Instance;
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			//this.IsExiting = false;	// Careful!
			
			// Already running?
			if( this.Data.IsRunning ) {
				LogHelpers.Warn( "Session already running" );
				return false;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert();
			}
			
			this.Data.IsRunning = true;
			this.Save();

			if( Main.netMode == 2 ) {
				SessionProtocol.SyncToClients();
			}

			return true;
		}

		
		public bool EndSession() {
			var mymod = ResetModeMod.Instance;
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			// Already ended?
			if( !this.Data.IsRunning ) {
				LogHelpers.Alert( "Already stopped." );
				return false;
			}

			if( mymod.Config.DebugModeInfo ) {
				string worldId = WorldHelpers.GetUniqueId(true);
				LogHelpers.Alert( "Sets ALL session data to defaults, stops all TimeLimit \"reset\" commands (world id: " + worldId + ")" );
			}

			this.IsExiting = false;
			this.Data.ResetAll();
			this.Save();

			if( Main.netMode == 2 ) {
				SessionProtocol.SyncToClients();
			}

			this.ResetCurrentWorldForSession();

			return true;
		}
	}
}
