using HamstarHelpers.DebugHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TimeLimit;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public void GoodExit( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - WorldLogic.GoodExit " + myworld.Data.IsExiting );
			}

			myworld.Data.IsExiting = true;

			if( Main.netMode == 0 ) {
				Main.NewText( "Time's up. Please switch to the next world to continue.", Color.Red );

				TimeLimitAPI.TimerStart( "exit", 5, false );
			} else if( Main.netMode == 2 ) {
				NetMessage.BroadcastChatMessage( NetworkText.FromLiteral( "Time's up. Please switch to the next new world to continue." ), Color.Red, -1 );

				TimeLimitAPI.TimerStart( "serverclose", 7, false );
			}
		}


		public void BadExit( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode - WorldLogic.BadExit " + myworld.Data.IsExiting );
			}

			if( myworld.Data.IsExiting ) { return; }
			myworld.Data.IsExiting = true;

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
