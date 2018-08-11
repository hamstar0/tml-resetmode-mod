using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal void Update() {
			var mymod = ResetModeMod.Instance;

			if( mymod.Config.DebugModeInfo ) {
				string world_id = WorldHelpers.GetUniqueIdWithSeed();

				DebugHelpers.Print( "ResetModeSessionUpdate",
					"Is running? "+ this.Data.IsRunning
					+ ", Exiting? "+this.IsExiting
					+ ", Needs world? " + this.IsSessionNeedingWorld()
					+ ", World id: " + world_id
					+ ", Been played? " + this.HasWorldEverBeenPlayed( world_id ), 20 );
			}

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
			this.UpdateGame( mymod );
		}

		internal void UpdateClient( ResetModeMod mymod ) {
		}

		internal void UpdateServer( ResetModeMod mymod ) {
			this.UpdateGame( mymod );
		}

		////////////////
		
		private void UpdateGame( ResetModeMod mymod ) {
			string world_id = WorldHelpers.GetUniqueIdWithSeed();

			if( this.IsSessionNeedingWorld() ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.SessionLogic.UpdateGame - Session needs a world (current world id: " + world_id + ")" );
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
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.SessionLogic.UpdateGame - World has expired (current world id: " + world_id + ")" );
				}

				if( mymod.Config.WrongWorldForcesHardReset ) {
					this.Data.AwaitingNextWorld = true;
					this.Data.CurrentSessionedWorldId = "";
					this.Save( mymod );
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
