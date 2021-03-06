﻿using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Players;
using HamstarHelpers.Helpers.TModLoader;
using HamstarHelpers.Services.Messages.Inbox;
using Microsoft.Xna.Framework;
using ResetMode.NetProtocols;
using System;
using Terraria;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public static int GetHash( Player player ) {
			return PlayerIdentityHelpers.GetVanillaSnapshotHash( player, true, false );
		}



		////////////////

		internal bool IsPromptingForResetOnLocal = false;
		internal bool HasCheckedValidationOnHost = false;



		////////////////

		public void OnFinishLocalSync( Player player ) {
			var mymod = ResetModeMod.Instance;
			if( mymod.Session.IsSessionNeedingWorld() ) {
				this.Welcome( player );
			} else if( !mymod.Session.IsSessionedWorldNotOurs() ) {
				this.Instruct( player );
			}
		}


		////////////////
		
		public void PreUpdateUnsyncedLocal() { }

		public void PreUpdateSyncedSingle() {
			var mymod = ResetModeMod.Instance;
			this.CheckValidationOnHost( Main.LocalPlayer );
			this.UpdatePromptStasisState( Main.LocalPlayer );
		}

		public void PreUpdateSyncedCurrentClient() {
			var mymod = ResetModeMod.Instance;
			this.UpdatePromptStasisState( Main.LocalPlayer );
		}

		public void PreUpdateSyncedServerForPlayer( Player player ) {
			if( PlayerIdentityHelpers.GetUniqueId( player ) != null ) {
				this.CheckValidationOnHost( player );
			}
		}


		////////////////

		private void CheckValidationOnHost( Player player ) {
			if( Main.netMode == 1 ) {
				throw new Exception( "!ResetMode.PlayerLogic.CheckValidation - No clients." );
			}
			
			var mymod = ResetModeMod.Instance;
			if( !mymod.Session.Data.IsRunning ) { return; }
			if( this.HasCheckedValidationOnHost ) { return; }
			
			if( mymod.Session.IsSessionNeedingWorld() ) { return; }
			if( mymod.Session.IsPlaying( player ) ) { return; }

			if( mymod.Session.IsSessionedWorldNotOurs() ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Alert( "Playing sessioned world that isn't ours; ejecting player " + player.name + "..." );
				}

				if( Main.netMode == 0 ) {
					PlayerEjectProtocol.Eject( player );
				} else if( Main.netMode == 2 ) {
					PlayerEjectProtocol.QuickRequest( player.whoAmI );
				}
				return;
			}

			this.HasCheckedValidationOnHost = true;
			this.ValidatePlayerOnHost( player );
		}


		private void UpdatePromptStasisState( Player player ) {
			var mymod = ResetModeMod.Instance;
			if( !mymod.Session.Data.IsRunning ) { return; }

			if( this.IsPromptingForResetOnLocal ) {
				PlayerHelpers.LockdownPlayerPerTick( player );
			}
		}


		////////////////

		public void Welcome( Player player ) {
			var mymod = ResetModeMod.Instance;
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

		public void Boot( Player player, string reason ) {
			var mymod = ResetModeMod.Instance;
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Alert( "Player "+player.name+" (" + player.whoAmI+")" );
			}

			LogHelpers.Log( player.name + " was booted. Reason: " + reason );
			TmlHelpers.ExitToMenu( true );
		}
	}
}
