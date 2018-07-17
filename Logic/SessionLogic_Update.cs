using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal void Update() {
			var mymod = ResetModeMod.Instance;
			
			if( this.Data.IsRunning && !this.IsExiting ) {
				if( Main.netMode == 0 ) {
					this.UpdateSingle( mymod );
				} else if( Main.netMode == 1 ) {
					this.UpdateClient( mymod );
				} else {
					this.UpdateServer( mymod );
				}
			}
		}


		////////////////

		internal void UpdateSingle( ResetModeMod mymod ) {
			this.UpdateHost( mymod );
		}

		internal void UpdateClient( ResetModeMod mymod ) {
		}

		internal void UpdateServer( ResetModeMod mymod ) {
			this.UpdateHost( mymod );
		}

		////////////////
		
		private void UpdateHost( ResetModeMod mymod ) {
			if( this.IsSessionNeedingWorld() ) {
				string world_id = WorldHelpers.GetUniqueIdWithSeed();

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.UpdateHost - Session needs a world (current world id: " + world_id + ")" );
				}
				
				if( this.HasWorldEverBeenPlayed( world_id ) ) {
					if( Main.netMode != 2 ) {   // Servers should just indefinitely boot people until closed; stopgap measure
						this.GoodExit( mymod );
					}
				} else {
					this.BeginResetTimer( mymod );
					this.AddWorldToSession( mymod );    // Changes world status
				}
			} else if( this.IsSessionedWorldNotOurs() ) {
				string world_id = WorldHelpers.GetUniqueIdWithSeed();

				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.Logic.SessionLogic.UpdateSessionHost - World has expired (current world id: " + world_id + ")" );
				}

				if( this.HasWorldEverBeenPlayed( world_id ) ) {
					this.GoodExit( mymod );
				} else {
					this.BadExit( mymod );
				}
			}
		}
	}
}
