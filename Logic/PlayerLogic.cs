using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.Utilities.Messages;
using Microsoft.Xna.Framework;
using ResetMode.Data;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public static int GetHash( Player player ) {
			return PlayerIdentityHelpers.GetVanillaSnapshotHash( player, true, false );
		}



		////////////////

		private bool IsPromptingForReset = false;
		private bool HasCheckedValidation = false;



		////////////////

		public void OnEnterWorld( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			switch( myworld.Data.WorldStatus ) {
			case ResetModeStatus.Normal:
				this.Welcome( mymod, player );
				break;

			case ResetModeStatus.Active:
				this.Instruct( player );
				break;
			}
		}


		////////////////

		public void PreUpdateSingle( ResetModeMod mymod, Player player ) {
			this.CheckValidation( mymod, player );
			this.UpdatePrompt( player );
		}

		public void PreUpdateClient( ResetModeMod mymod, Player player ) {
			this.UpdatePrompt( player );
		}

		public void PreUpdateServer( ResetModeMod mymod, Player player ) {
			this.CheckValidation( mymod, player );
		}

		////////////////

		private void UpdatePrompt( Player player ) {
			if( this.IsPromptingForReset ) {
				player.noItems = true;
				player.noBuilding = true;
				player.stoned = true;
				player.immune = true;
				player.immuneTime = 2;
			}
		}

		private void CheckValidation( ResetModeMod mymod, Player player ) {
			if( this.HasCheckedValidation ) { return; }

			if( mymod.Session.Data.IsRunning ) {
				var myworld = mymod.GetModWorld<ResetModeWorld>();
				
				if( myworld.Data.WorldStatus == ResetModeStatus.Active ) {
					if( !myworld.Data.IsPlaying( mymod, player ) ) {
						this.HasCheckedValidation = true;
						this.ValidatePlayer( mymod, player );
					}
				}
			}
		}


		////////////////

		public void Welcome( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 0 ) {
				InboxMessages.SetMessage( "reset_mode_welcome", "Type /resetmodestart to start Reset Mode. Type /help for a list of other available commands.", true );
			} else {
				InboxMessages.SetMessage( "reset_mode_welcome", "Type resetmodestart in the server console to start Reset Mode.", true );
			}
		}

		public void Unwelcome( Player player ) {
			Main.NewText( "Warning: " + player.name + " is already playing reset mode elsewhere. You risk losing your progress.", Color.Red );
		}

		public void Instruct( Player player ) {
			Main.NewText( "Welcome to Reset Mode. After the timer, you and this world reset. Only your Progress Points (PP) are kept for the next.", Color.Cyan );
		}


		////////////////

		public void Boot( ResetModeMod mymod, Player player, string reason ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.Boot player: " + player.whoAmI );
			}

			ErrorLogger.Log( player.name + " was booted because " + reason );
			TmlHelpers.ExitToMenu( true );
		}
	}
}
