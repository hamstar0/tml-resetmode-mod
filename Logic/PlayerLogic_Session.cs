using HamstarHelpers.DebugHelpers;
using HamstarHelpers.PlayerHelpers;
using HamstarHelpers.UIHelpers.Elements.Dialogs;
using HamstarHelpers.WorldHelpers;
using ResetMode.NetProtocol;
using System;
using Terraria;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public void ValidatePlayer( ResetModeMod mymod, Player player ) {
			this.IsValidating = true;

			var myworld = mymod.GetModWorld<ResetModeWorld>();

			if( this.ActiveForWorldUid != WorldHelpers.GetUniqueId() ) {
				bool not_playing = string.IsNullOrEmpty( this.ActiveForWorldUid );
				bool was_playing = myworld.Logic.IsWorldUidInSession( mymod, this.ActiveForWorldUid );

				if( not_playing || was_playing ) {
					if( !this.IsPrompting ) {
LogHelpers.Log( "ValidatePlayer not_playing: "+ not_playing+ ", was_playing: "+ was_playing+", wuid: "+ this.ActiveForWorldUid );
						this.PromptToBeginPlaying( mymod, player );
					}
				} else {
					this.Boot( mymod, player, "already playing another session" );
				}
			}

			if( mymod.Logic.NetMode != 1 ) {
				bool has_pp_changed;
				bool is_synced = mymod.Logic.IsPlayerSessionSynced( mymod, player, out has_pp_changed );

				// PP changes warrant a boot?
				if( has_pp_changed ) {
					this.Boot( mymod, player, "already using Rewards elsewhere" );
					return;
				}

				if( !is_synced ) {
					if( !this.IsPrompting ) {
LogHelpers.Log( "ValidatePlayer not synced" );
						this.PromptToBeginPlaying( mymod, player );
					}
				}
			}
		}


		////////////////

		public void PromptToBeginPlaying( ResetModeMod mymod, Player player ) {
			this.IsPrompting = true;

			if( mymod.Logic.NetMode == 2 ) {
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
				this.Boot( mymod, player, "choose not to play" );
			};

			var prompt = new UIPromptDialog( new UIPromptTheme(), 600, 112, text, confirm_action, cancel_action );
			prompt.Open();
		}


		////////////////

		public void BeginSession( ResetModeMod mymod, Player player ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.BeginSession player: " + player.whoAmI );
			}

			this.ActiveForWorldUid = WorldHelpers.GetUniqueId();

			if( mymod.Logic.NetMode == 1 ) {
				this.SyncClient( mymod, player );
			}

			this.AddPlayerToSession( mymod, player );
		}


		////////////////

		public void AddPlayerToSession( ResetModeMod mymod, Player player ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.AddPlayerToSession player: " + player.whoAmI );
			}

			var myplayer = player.GetModPlayer<ResetModePlayer>();

			if( mymod.Logic.NetMode != 1 ) {
				this.FinishPlayerSessionJoin( mymod, player );
			} else {
				ClientPackets.RequestPlayerSessionJoinAcknowledge( mymod );
			}
		}


		public void FinishPlayerSessionJoin( ResetModeMod mymod, Player player ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "PlayerLogic.FinishPlayerSessionJoin player: " + player.whoAmI );
			}

			var myplayer = player.GetModPlayer<ResetModePlayer>();
			
			this.IsPrompting = false;
		}
	}
}
