using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader.IO;
using TimeLimit;


namespace ResetMode.Logic {
	public enum ResetModeStatus {
		Normal,
		Active,
		Expired
	}



	partial class WorldLogic {
		internal ResetModeStatus WorldStatus = ResetModeStatus.Normal;

		public bool IsExiting { get; private set; }


		////////////////

		public WorldLogic() {
			this.IsExiting = false;
		}

		////////////////

		internal void Load( ResetModeMod mymod, TagCompound tags ) {
			if( tags.ContainsKey("status") ) {
				this.WorldStatus = (ResetModeStatus)tags.GetInt( "status" );
			}
		}

		internal TagCompound Save() {
			return new TagCompound { { "status", (int)this.WorldStatus } };
		}



		////////////////

		public void Update( ResetModeMod mymod ) {
			switch( this.WorldStatus ) {
			case ResetModeStatus.Normal:
				if( mymod.Logic.IsSessionStarted( mymod ) ) {
					if( mymod.Session.AwaitingNextWorld ) {
						this.EngageWorldForCurrentSession( mymod );
					} else {
						this.BadExit();
					}
				}
				break;

			case ResetModeStatus.Expired:
				if( mymod.Logic.IsSessionStarted( mymod ) ) {
					this.GoodExit();
				}
				break;
			}
		}


		////////////////

		public void GoodExit() {
			if( this.IsExiting ) { return; }
			this.IsExiting = true;

			if( Main.netMode == 0 ) {
				Main.NewText( "Time's up. Please switch to the next world to continue.", Color.Red );
			} else if( Main.netMode == 2 ) {
				NetMessage.BroadcastChatMessage( NetworkText.FromLiteral( "Time's up. Please switch to the next new world to continue." ), Color.Red, -1 );

				TimeLimitAPI.TimerStart( "serverclose", 8, false );
			}

			TimeLimitAPI.TimerStart( "exit", 6, false );
		}


		public void BadExit() {
			if( this.IsExiting ) { return; }
			this.IsExiting = true;

			if( Main.netMode == 0 ) {
				//TmlHelpers.ExitToMenu( false );
				TimeLimitAPI.TimerStart( "exit", 4, false );
				Main.NewText( "World not valid for reset mode. Exiting...", Color.Red );
			} else {
				//TmlHelpers.ExitToDesktop( false );
				TimeLimitAPI.TimerStart( "serverclose", 4, false );
				NetMessage.BroadcastChatMessage( NetworkText.FromLiteral( "World not valid for reset mode. Exiting..." ), Color.Red );
			}
		}
	}
}
