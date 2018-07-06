using HamstarHelpers.DebugHelpers;
using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.Services.Messages;
using HamstarHelpers.Services.Timers;
using HamstarHelpers.TmlHelpers;
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
			case ResetModeWorldStatus.Normal:
				this.Welcome( mymod, player );
				break;

			case ResetModeWorldStatus.Active:
				this.Instruct( player );
				break;
			}
		}


		////////////////

		public void PreUpdateUnsyncedLocal( ResetModeMod mymod, Player player ) {
			this.CheckFailsafeTimerUnsynced( mymod, player );
		}

		public void PreUpdateSyncedSingle( ResetModeMod mymod ) {
			this.CheckValidation( mymod, Main.LocalPlayer );
			this.UpdatePromptStasis( mymod, Main.LocalPlayer );

			this.CheckFailsafeTimerSynced( mymod );
		}

		public void PreUpdateSyncedClient( ResetModeMod mymod, Player player ) {
			this.UpdatePromptStasis( mymod, player );

			this.CheckFailsafeTimerSynced( mymod );
		}

		public void PreUpdateSyncedServer( ResetModeMod mymod, Player player ) {
			if( LoadHelpers.IsWorldSafelyBeingPlayed() ) {
				this.CheckValidation( mymod, player );
			}
		}

		////////////////

		private void UpdatePromptStasis( ResetModeMod mymod, Player player ) {
			if( !mymod.Session.Data.IsRunning ) { return; }

			if( this.IsPromptingForReset ) {
				player.noItems = true;
				player.noBuilding = true;
				player.stoned = true;
				player.immune = true;
				player.immuneTime = 2;
			}
		}

		private void CheckValidation( ResetModeMod mymod, Player player ) {
			if( !mymod.Session.Data.IsRunning ) { return; }
			if( this.HasCheckedValidation ) { return; }

			var myworld = mymod.GetModWorld<ResetModeWorld>();
			if( myworld.Data.WorldStatus != ResetModeWorldStatus.Active ) { return; }

			if( myworld.Data.IsPlaying( mymod, player ) ) { return; }

			this.HasCheckedValidation = true;
			this.ValidatePlayer( mymod, player );
		}

		////////////////
		
		private void CheckFailsafeTimerUnsynced( ResetModeMod mymod, Player player ) {
			if( !mymod.Session.Data.IsRunning ) { return; }

			if( Timers.GetTimerTickDuration( "ResetMode:ValidationTimeoutFailsafe" ) == 0 ) {
				Timers.SetTimer( "ResetMode:ValidationTimeoutFailsafe", 10 * 60, () => {
					this.Boot( mymod, player, "Could not be validated." );
					return false;
				} );
			}
		}

		private void CheckFailsafeTimerSynced( ResetModeMod mymod ) {
			if( !mymod.Session.Data.IsRunning ) { return; }

			if( Timers.GetTimerTickDuration( "ResetMode:ValidationTimeoutFailsafe" ) > 0 ) {
				Timers.UnsetTimer( "ResetMode:ValidationTimeoutFailsafe" );
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

			ErrorLogger.Log( player.name + " was booted. Reason: " + reason );
			TmlHelpers.ExitToMenu( true );
		}
	}
}
