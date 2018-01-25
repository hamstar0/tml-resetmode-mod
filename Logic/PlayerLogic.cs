using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.UIHelpers.Elements.Dialogs;
using HamstarHelpers.WorldHelpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader.IO;


namespace ResetMode.Logic {
	class PlayerLogic {
		public string ActiveForWorldUid { get; private set; }
		private bool IsPrompting = false;


		////////////////

		public PlayerLogic() {
			this.ActiveForWorldUid = "";
		}

		public void Load( Player player, TagCompound tags ) {
			if( tags.ContainsKey( "active_world_uid" ) ) {
				this.ActiveForWorldUid = tags.GetString( "active_world_uid" );
			}

			if( tags.ContainsKey( "me_hash" ) ) {
				int hash = tags.GetInt( "me_hash" );

				if( hash != PlayerHelpers.GetVanillaSnapshotHash( player, true, false ) ) {
					this.ActiveForWorldUid = "";
				}
			}
		}

		public TagCompound Save( Player player ) {
			return new TagCompound {
				{ "active_world_uid", this.ActiveForWorldUid },
				{ "me_hash", PlayerHelpers.GetVanillaSnapshotHash(player, true, false) }
			};
		}


		////////////////

		public void Welcome( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			switch( myworld.Logic.WorldStatus ) {
			case ResetModeStatus.Normal:
				if( Main.netMode == 0 ) {
					Main.NewText( "Type /resetmodebegin to start Reset Mode. Type /help for a list of other available commands.", Color.LightGray );
				} else {
					Main.NewText( "Type resetmodebegin in the server console to start Reset Mode.", Color.LightGray );
				}
				break;
			case ResetModeStatus.Active:
				Main.NewText( "Welcome to Reset Mode. This world will reset into a new one after the timer expires. Only your Progress Points remain between worlds.", Color.LightYellow );
				break;
			}
		}


		////////////////

		public void Update( ResetModeMod mymod, Player player ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			
			switch( myworld.Logic.WorldStatus ) {
			case ResetModeStatus.Active:
				if( !this.IsPlaying( mymod ) && !this.IsPrompting ) {
					this.PromptToBeginPlaying( player );
				}
				break;
			case ResetModeStatus.Expired:
				TmlHelpers.ExitToMenu( false );
				break;
			}

			if( this.IsPrompting ) {
				player.noItems = true;
				player.noBuilding = true;
				player.stoned = true;
				player.immune = true;
			}
		}


		////////////////

		public void PromptToBeginPlaying( Player player ) {
			this.IsPrompting = true;

			string text = "Play reset mode? Your character will be reset (except Progress Points)."+
				"\nNote: Playing this character on another world will force it to reset here.";

			Action confirm_action = delegate () {
				this.IsPrompting = false;
				this.BeginResetMode( player );
			};
			Action cancel_action = delegate () {
				this.IsPrompting = false;
				TmlHelpers.ExitToMenu( false );
			};

			var prompt = new UIPromptDialog( new UIPromptTheme(), 600, 112, text, confirm_action, cancel_action );
			prompt.Open();
		}


		////////////////

		public bool IsPlaying( ResetModeMod mymod ) {
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			return !string.IsNullOrEmpty(this.ActiveForWorldUid) && myworld.Logic.WorldStatus != ResetModeStatus.Normal;
		}


		public void BeginResetMode( Player player ) {
			this.ActiveForWorldUid = WorldHelpers.GetUniqueId();

			PlayerHelpers.FullVanillaReset( player );
		}
	}
}
