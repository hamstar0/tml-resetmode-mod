using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.UIHelpers.Elements.Dialogs;
using HamstarHelpers.Utilities.Errors;
using HamstarHelpers.Utilities.Network;
using System;
using Terraria;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public static void ValidateAll( ResetModeMod mymod ) {
			for( int i=0; i<Main.player.Length; i++ ) {
				Player player = Main.player[i];
				if( player == null || !player.active ) { continue; }

				var myplayer = player.GetModPlayer<ResetModePlayer>();
				myplayer.Logic.ValidatePlayer( mymod, player );
			}
		}



		////////////////

		public void ValidatePlayer( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 1 ) { throw new Exception("Clients cannot call this."); }

			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );
			
			if( !has_uid ) { throw new HamstarException( "ValidatePlayer - Player has no uid." ); }

			PacketProtocol.QuickSendRequest<ResetModePlayerResetProtocol>( player.whoAmI, -1 );
		}


		////////////////

		public void PromptReset( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 2 ) { throw new Exception( "Server cannot call this." ); }
			
			this.IsPromptingForReset = true;
			
			string text = "Play reset mode? Your character will be reset (except Progress Points)." +
				"\nNote: Playing this character on another world will force it to reset here.";

			Action confirm_action = delegate () {
				PlayerHelpers.FullVanillaReset( player );
				this.BeginSession( mymod, player );
			};
			Action cancel_action = delegate () {
				this.Boot( mymod, player, "choose not to play" );
			};

			var prompt = new UIPromptDialog( new UIPromptTheme(), 600, 112, text, confirm_action, cancel_action );
			prompt.Open();
		}


		////////////////

		public void BeginSession( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 1 ) {
				PacketProtocol.QuickSendRequest<ResetModePlayerResetConfirmProtocol>( -1, -1 );
			} else {
				var myworld = mymod.GetModWorld<ResetModeWorld>();
				myworld.Logic.AddPlayer( mymod, player );
			}
		}
	}
}
