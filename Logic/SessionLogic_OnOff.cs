using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.MiscHelpers;
using HamstarHelpers.WorldHelpers;
using System;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public bool StartSession( ResetModeMod mymod ) {
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.Start" );
			}

			// Already running?
			if( this.Data.IsRunning ) {
				return false;
			}
			
			this.Data.IsRunning = true;

			return true;
		}
		
		public void EndSession( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Config.DebugModeInfo ) {
				string world_id = WorldHelpers.GetUniqueIdWithSeed();
				LogHelpers.Log( "WorldLogic.End " + world_id );
			}

			this.Data.ResetAll();

			this.Save( mymod );

			this.ResetWorldForSession( mymod );
		}
	}
}
