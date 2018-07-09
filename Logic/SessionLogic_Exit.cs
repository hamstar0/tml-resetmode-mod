using HamstarHelpers.DebugHelpers;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using TimeLimit;


namespace ResetMode.Logic {
	partial class SessionLogic {
		public void GoodExit( ResetModeMod mymod ) {
			if( mymod.Session.IsExiting ) { return; }
			mymod.Session.IsExiting = true;

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.Logic.SessionLogic.GoodExit " + mymod.Session.IsExiting );
			}

			string msg = "This world is now expired. Please switch to the next world to continue.";

			if( Main.netMode == 0 ) {
				Main.NewText( msg, Color.Red );

				TimeLimitAPI.TimerStart( "exit", 5, false );
			} else if( Main.netMode == 2 ) {
				NetMessage.BroadcastChatMessage( NetworkText.FromLiteral( msg ), Color.Red, -1 );

				TimeLimitAPI.TimerStart( "serverclose", 7, false );
			}
		}


		public void BadExit( ResetModeMod mymod ) {
			if( mymod.Session.IsExiting ) { return; }
			mymod.Session.IsExiting = true;

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.Logic.SessionLogic.BadExit" );
			}

			string msg = "World not valid for reset mode. Exiting...";

			if( Main.netMode == 0 ) {
				//TmlHelpers.ExitToMenu( false );
				TimeLimitAPI.TimerStart( "exit", 4, false );
				Main.NewText( msg, Color.Red );
			} else {
				//TmlHelpers.ExitToDesktop( false );
				TimeLimitAPI.TimerStart( "serverclose", 4, false );
				NetMessage.BroadcastChatMessage( NetworkText.FromLiteral(msg), Color.Red );
			}
		}
	}
}
