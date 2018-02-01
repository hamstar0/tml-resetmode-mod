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

			if( mymod.Logic.NetMode != 1 ) {
				mymod.SessionJson.LoadFile();
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
						mymod.Logic.ResumeSession( mymod );
						this.EngageWorldForCurrentSession( mymod );
					} else {
						//this.BadExit();
					}
				}
				break;

			case ResetModeStatus.Expired:
				if( mymod.Logic.IsSessionStarted( mymod ) ) {
					this.GoodExit( mymod );
				}
				break;
			}
		}


		////////////////

		public void GoodExit( ResetModeMod mymod ) {
			if( this.IsExiting ) { return; }
			this.IsExiting = true;

			if( mymod.Logic.NetMode == 0 ) {
				Main.NewText( "Time's up. Please switch to the next world to continue.", Color.Red );

				TimeLimitAPI.TimerStart( "exit", 5, false );
			} else if( mymod.Logic.NetMode == 2 ) {
				NetMessage.BroadcastChatMessage( NetworkText.FromLiteral( "Time's up. Please switch to the next new world to continue." ), Color.Red, -1 );

				TimeLimitAPI.TimerStart( "serverclose", 7, false );
			}
		}


		public void BadExit( ResetModeMod mymod ) {
			if( this.IsExiting ) { return; }
			this.IsExiting = true;

			if( mymod.Logic.NetMode == 0 ) {
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
