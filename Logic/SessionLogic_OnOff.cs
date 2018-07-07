using HamstarHelpers.Components.Network;
using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
using ResetMode.NetProtocols;
using System;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public bool StartSession( ResetModeMod mymod ) {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			// Already running?
			if( this.SessionData.IsRunning ) {
				return false;
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - SessionLogic.StartSession" );
			}
			
			this.SessionData.IsRunning = true;
			this.Save( mymod );

			if( Main.netMode == 2 ) {
				PacketProtocol.QuickSendToClient<SessionProtocol>( -1, -1 );
			}

			//PlayerLogic.ValidateAll( mymod );

			return true;
		}

		
		public bool EndSession( ResetModeMod mymod ) {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			// Already ended?
			if( !this.SessionData.IsRunning ) {
				return false;
			}

			if( mymod.Config.DebugModeInfo ) {
				string world_id = WorldHelpers.GetUniqueIdWithSeed();
				LogHelpers.Log( "ResetMode - SessionLogic.EndSession" );
			}

			this.SessionData.ResetAll();
			this.Save( mymod );

			if( Main.netMode == 2 ) {
				PacketProtocol.QuickSendToClient<SessionProtocol>( -1, -1 );
			}

			this.ResetCurrentWorldForSession( mymod );

			return true;
		}
	}
}
