using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.TmlHelpers;
using HamstarHelpers.UIHelpers.Elements.Dialogs;
using HamstarHelpers.WorldHelpers;
using ResetMode.NetProtocol;
using System;
using Terraria;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public void ValidatePlayer( ResetModeMod mymod, Player player ) {
			if( this.IsValidating ) { return; }
			this.IsValidating = true;

			var myworld = mymod.GetModWorld<ResetModeWorld>();
			var player_world = this.ActiveForWorldUid;
			bool is_prompting = false;

			if( player_world != WorldHelpers.GetUniqueId() ) {
				bool not_playing = string.IsNullOrEmpty( player_world );
				bool was_playing = myworld.Logic.IsWorldUidInSession( mymod, player_world );

				if( not_playing || was_playing ) {
					is_prompting = true;
					this.PromptToBeginPlaying( mymod, player );
				} else {
					this.Boot( player );
				}
			}

			if( !is_prompting ) {
				if( !mymod.Logic.IsPlayerSessionSynced( mymod, player ) ) {
					this.PromptToBeginPlaying( mymod, player );
				}
			}
		}


		////////////////

		public void PromptToBeginPlaying( ResetModeMod mymod, Player player ) {
			if( this.IsPrompting ) { return; }
			this.IsPrompting = true;

			if( Main.netMode == 2 ) {
				ServerPackets.SendPromptForReset( mymod, player.whoAmI );
				return;
			}

			string text = "Play reset mode? Your character will be reset (except Progress Points)." +
				"\nNote: Playing this character on another world will force it to reset here.";

			Action confirm_action = delegate () {
				PlayerHelpers.FullVanillaReset( player );
				this.BeginSession( mymod, player );
			};
			Action cancel_action = delegate () {
				this.Boot( player );
			};

			var prompt = new UIPromptDialog( new UIPromptTheme(), 600, 112, text, confirm_action, cancel_action );
			prompt.Open();
		}


		////////////////

		public void BeginSession( ResetModeMod mymod, Player player ) {
			this.ActiveForWorldUid = WorldHelpers.GetUniqueId();

			this.AddPlayerToSession( mymod, player );
		}

		public void Boot( Player player ) {
			TmlHelpers.ExitToMenu( true );
		}


		////////////////

		public void AddPlayerToSession( ResetModeMod mymod, Player player ) {
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			if( Main.netMode != 1 ) {
				this.FinishPlayerSessionJoin( mymod, player );
			} else {
				ClientPackets.RequestPlayerSessionJoinAcknowledge( mymod );
			}
		}


		public void FinishPlayerSessionJoin( ResetModeMod mymod, Player player ) {
			var myplayer = player.GetModPlayer<ResetModePlayer>();
			
			this.IsPrompting = false;
		}
	}
}
