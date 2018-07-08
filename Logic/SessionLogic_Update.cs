using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal void UpdateSession() {
			var mymod = ResetModeMod.Instance;
			
			if( this.Data.IsRunning && !this.IsExiting ) {
				if( Main.netMode == 0 ) {
					this.UpdateSessionSingle( mymod );
				} else if( Main.netMode == 1 ) {
					this.UpdateSessionClient( mymod );
				} else {
					this.UpdateSessionServer( mymod );
				}
			}
		}


		////////////////

		internal void UpdateSessionSingle( ResetModeMod mymod ) {
			this.UpdateSessionHost( mymod );
		}

		internal void UpdateSessionClient( ResetModeMod mymod ) {
		}

		internal void UpdateSessionServer( ResetModeMod mymod ) {
			this.UpdateSessionHost( mymod );
		}

		////////////////
		
		private void UpdateSessionHost( ResetModeMod mymod ) {
			if( this.IsSessionNeedingWorld() ) {
				this.BeginResetTimer( mymod );
				this.AddWorldToSession( mymod );    // Changes world status
			} else if( this.IsSessionedWorldNotOurs() ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.SessionLogic.UpdateSessionHost - World has expired. World id: " + WorldHelpers.GetUniqueIdWithSeed() );
				}

				this.GoodExit( mymod );
			}
		}
	}
}
