using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.TmlHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal void Update() {
			var mymod = ResetModeMod.Instance;

			if( mymod.Config.DebugModeRealTimeInfo ) {
				string worldId = WorldHelpers.GetUniqueId(true);
				var myplayer = (ResetModePlayer)TmlHelpers.SafelyGetModPlayer( Main.LocalPlayer, mymod, "ResetModePlayer" );

				DebugHelpers.Print( "ResetModeSession",
					"Is running? "+ this.Data.IsRunning
					+ ", Exiting? "+this.IsExiting
					+ ", Needs world? " + this.IsSessionNeedingWorld()
					+ ", World id: " + worldId
					+ ", Been played? " + this.HasWorldEverBeenPlayed( worldId ), 20 );
				DebugHelpers.Print( "ResetModePlayer",
					"IsPromptingForResetOnLocal? " + myplayer.Logic.IsPromptingForResetOnLocal
					+ ", IsSynced? " + myplayer.IsSynced
					+ ", HasModSettings? "+myplayer.HasModSettings
					+ ", HasSessionData? "+myplayer.HasSessionData, 20 );
			}

			if( this.Data.IsRunning && !this.IsExiting ) {
				if( Main.netMode == 0 ) {
					this.UpdateSingle();
				} else if( Main.netMode == 1 ) {
					this.UpdateClient();
				} else {
					this.UpdateServer();
				}
			}
		}


		////////////////

		internal void UpdateSingle() {
			this.UpdateGame();
		}

		internal void UpdateClient() {
		}

		internal void UpdateServer() {
			this.UpdateGame();
		}

		////////////////
		
		private void UpdateGame() {
			var mymod = ResetModeMod.Instance;
			string worldId = WorldHelpers.GetUniqueId(true);

			if( this.IsSessionNeedingWorld() ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Alert( "Session needs a world (current world id: " + worldId + ")" );
				}
				
				if( this.HasWorldEverBeenPlayed( worldId ) ) {
					//if( Main.netMode != 2 ) {   // Servers should just indefinitely boot people until closed; stopgap measure
					this.GoodExit();
				} else {
					this.BeginResetTimer();
					this.AddWorldToSession();    // Changes world status
				}
			} else if( this.IsSessionedWorldNotOurs() ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Alert( "World has expired (current world id: " + worldId + "). Session data: "+this.Data.ToString() );
				}

				if( mymod.Config.WrongWorldForcesHardReset ) {
					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Alert( "WrongWorldForcesHardReset == true. Sets AwaitingNextWorld=true, CurrentSessionedWorldId=\"\"" );
					}
					this.Data.AwaitingNextWorld = true;
					this.Data.CurrentSessionedWorldId = "";
					this.Save();
				}

				if( this.HasWorldEverBeenPlayed( worldId ) ) {
					this.GoodExit();
				} else {
					this.BadExit();
				}
			}
		}
	}
}
