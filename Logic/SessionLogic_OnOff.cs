using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using ResetMode.NetProtocols;
using System;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public bool StartSession( ResetModeMod mymod ) {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			//this.IsExiting = false;	// Careful!
			
			// Already running?
			if( this.Data.IsRunning ) {
				LogHelpers.Log( "!ResetMode.SessionLogic.StartSession - Session already running" );
				return false;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.SessionLogic.StartSession" );
			}
			
			this.Data.IsRunning = true;
			this.Save( mymod );

			if( Main.netMode == 2 ) {
				SessionProtocol.SyncToClients();
			}

			return true;
		}

		
		public bool EndSession( ResetModeMod mymod ) {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			// Already ended?
			if( !this.Data.IsRunning ) {
				LogHelpers.Log( "ResetMode.SessionLogic.EndSession - Already stopped." );
				return false;
			}

			if( mymod.Config.DebugModeInfo ) {
				string world_id = WorldHelpers.GetUniqueIdWithSeed();
				LogHelpers.Log( "ResetMode.SessionLogic.EndSession" );
			}

			this.IsExiting = false;
			this.Data.ResetAll();
			this.Save( mymod );

			if( Main.netMode == 2 ) {
				SessionProtocol.SyncToClients();
			}

			this.ResetCurrentWorldForSession( mymod );

			return true;
		}
	}
}
