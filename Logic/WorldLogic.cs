using HamstarHelpers.DebugHelpers;
using HamstarHelpers.WorldHelpers;
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
			if( tags.ContainsKey( "status" ) ) {
				this.WorldStatus = (ResetModeStatus)tags.GetInt( "status" );
			}

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.Load uid: " + WorldHelpers.GetUniqueId() + ", this.WorldStatus: " + this.WorldStatus );
			}
		}

		internal TagCompound Save( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.Save uid: " + WorldHelpers.GetUniqueId() + ", this.WorldStatus: " + this.WorldStatus );
			}

			return new TagCompound { { "status", (int)this.WorldStatus } };
		}


		public void OnWorldStart( ResetModeMod mymod ) {
			if( this.WorldStatus == ResetModeStatus.Normal ) {
				if( mymod.Session.AwaitingNextWorld ) {
					this.EngageWorldForCurrentSession( mymod );
				}
			}
		}



		////////////////

		public void Update( ResetModeMod mymod ) {
			switch( this.WorldStatus ) {
			case ResetModeStatus.Normal:
				break;

			case ResetModeStatus.Expired:
				if( !this.IsExiting ) {
					this.GoodExit( mymod );
				}
				break;
			}
		}


		////////////////

		public void GoodExit( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.GoodExit " + this.IsExiting );
			}

			this.IsExiting = true;

			if( Main.netMode == 0 ) {
				Main.NewText( "Time's up. Please switch to the next world to continue.", Color.Red );

				TimeLimitAPI.TimerStart( "exit", 5, false );
			} else if( Main.netMode == 2 ) {
				NetMessage.BroadcastChatMessage( NetworkText.FromLiteral( "Time's up. Please switch to the next new world to continue." ), Color.Red, -1 );

				TimeLimitAPI.TimerStart( "serverclose", 7, false );
			}
		}


		public void BadExit( ResetModeMod mymod ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "WorldLogic.BadExit " + this.IsExiting );
			}

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
