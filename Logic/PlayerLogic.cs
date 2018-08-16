using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.Helpers.TmlHelpers;
using HamstarHelpers.Helpers.WorldHelpers;
using HamstarHelpers.Services.Messages;
using Microsoft.Xna.Framework;
using ResetMode.NetProtocols;
using System;
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
			if( mymod.Session.IsSessionNeedingWorld() ) {
				this.Welcome( mymod, player );
			} else if( !mymod.Session.IsSessionedWorldNotOurs() ) {
				this.Instruct( player );
			}
		}


		////////////////
		
		public void PreUpdateUnsyncedLocal( ResetModeMod mymod ) {
		}

		public void PreUpdateSyncedSingle( ResetModeMod mymod ) {
			this.CheckValidation( mymod, Main.LocalPlayer );
			this.UpdatePromptStasis( mymod, Main.LocalPlayer );
		}

		public void PreUpdateSyncedCurrentClient( ResetModeMod mymod ) {
			this.UpdatePromptStasis( mymod, Main.LocalPlayer );
		}

		public void PreUpdateSyncedServerForClient( ResetModeMod mymod, Player player ) {
			if( LoadHelpers.IsWorldSafelyBeingPlayed() ) {
				this.CheckValidation( mymod, player );
			}
		}


		////////////////

		private void CheckValidation( ResetModeMod mymod, Player player ) {
			if( Main.netMode == 1 ) {
				throw new Exception( "!ResetMode.PlayerLogic.CheckValidation - No clients." );
			}
			
			if( !mymod.Session.Data.IsRunning ) { return; }
			if( this.HasCheckedValidation ) { return; }
			
			if( mymod.Session.IsSessionNeedingWorld() ) { return; }
			if( mymod.Session.IsPlaying( mymod, player ) ) { return; }

			if( mymod.Session.IsSessionedWorldNotOurs() ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Log( "ResetMode.PlayerLogic.CheckValidation - Playing sessioned world that isn't ours; ejecting player " + player.name + "..." );
				}

				if( Main.netMode == 0 ) {
					PlayerEjectProtocol.Eject( player );
				} else if( Main.netMode == 2 ) {
					PacketProtocol.QuickRequestToClient<PlayerEjectProtocol>( player.whoAmI, -1 );
				}
				return;
			}

			this.HasCheckedValidation = true;
			this.ValidatePlayer( mymod, player );
		}


		private void UpdatePromptStasis( ResetModeMod mymod, Player player ) {
			if( !mymod.Session.Data.IsRunning ) { return; }

			if( this.IsPromptingForReset ) {
				PlayerHelpers.LockdownPlayerPerTick( player );
			}
		}


		////////////////

		public void Welcome( ResetModeMod mymod, Player player ) {
			if( !mymod.Config.AutoStartSession ) {
				if( Main.netMode == 0 ) {
					InboxMessages.SetMessage( "reset_mode_welcome", "Type /rm-start to start Reset Mode. Type /help for a list of other available commands.", true );
				} else {
					InboxMessages.SetMessage( "reset_mode_welcome", "Type rm-start in the server console to start Reset Mode.", true );
				}
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
				LogHelpers.Log( "ResetMode.PlayerLogic.Boot - Player "+player.name+" (" + player.whoAmI+")" );
			}

			ErrorLogger.Log( player.name + " was booted. Reason: " + reason );
			TmlHelpers.ExitToMenu( true );
		}
	}
}
