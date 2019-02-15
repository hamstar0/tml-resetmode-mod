using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.UI.Elements.Dialogs;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.Services.Promises;
using ResetMode.NetProtocols;
using System;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode.Logic {
	partial class PlayerLogic {
		public void ValidatePlayer( Player player ) {
			var mymod = ResetModeMod.Instance;
			if( Main.netMode == 1 ) { throw new Exception( "Clients cannot call this." ); }

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.PlayerLogic.ValidatePlayer - Validating "+player.name+"..." );
			}
			
			if( Main.netMode == 2 ) {
				PacketProtocolRequestToClient.QuickRequest<PlayerResetProtocol>( player.whoAmI, -1, -1 );
			} else if( Main.netMode == 0 ) {
				this.PromptReset( player );
			}
		}


		////////////////

		public void PromptReset( Player player ) {
			if( Main.netMode == 2 ) { throw new Exception( "Server cannot call this." ); }

			int playerWho = player.whoAmI;
			string text = "Play reset mode? Your character will be reset (except Progress Points)." +
				"\nNote: Playing this character on another world will force it to reset here.";

			Action confirmAction = delegate () {
				Player replayer = Main.player[ playerWho ];

				PlayerHelpers.FullVanillaReset( replayer );
				PlayerModHelpers.ModdedExtensionsReset( replayer );

				if( Main.netMode == 0 ) {
					this.BeginSessionForPlayer( replayer );
					this.RefundRewardsSpendings( replayer );
				} else if( Main.netMode == 1 ) {
					PacketProtocol.QuickRequestToServer<PlayerResetConfirmProtocol>( -1 );
				}

				this.IsPromptingForReset = false;
			};
			Action cancelAction = delegate () {
				this.Boot( player, "choose not to play" );
			};

			////

			this.IsPromptingForReset = true;

			var prompt = new UIPromptDialog( new UIPromptTheme(), 600, 112, text, confirmAction, cancelAction );
			prompt.Open();
		}


		////////////////

		public void BeginSessionForPlayer( Player player ) {
			var mymod = ResetModeMod.Instance;
			mymod.Session.AddPlayer( player );

			if( Main.netMode == 2 ) {
				int who = player.whoAmI;

				Promises.AddWorldInPlayOncePromise( () => {
					PacketProtocol.QuickSendToClient<SessionProtocol>( who, -1 );
				} );
			}
		}

		////////////////

		public void RefundRewardsSpendings( Player player ) {
			var mymod = ResetModeMod.Instance;
			Mod rewardsMod = ModLoader.GetMod( "Rewards" );
			if( rewardsMod == null ) {
				if( mymod.Config.DebugModeInfo ) {
					LogHelpers.Alert( "No Rewards mod." );
				}
				return;
			}
			
			var uid = PlayerIdentityHelpers.GetProperUniqueId( player );

			if( mymod.Config.ResetRewardsSpendings ) {
				if( mymod.Session.Data.PlayerPPSpendings.ContainsKey( uid ) ) {
					float pp_spent = mymod.Session.Data.PlayerPPSpendings[uid];

					rewardsMod.Call( "AddPoints", player, pp_spent );

					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Log( "ResetMode.PlayerLogic.RefundRewardsSpendings - '" + player.name + "' PP spendings of " + pp_spent + " returned" );
					}
				} else {
					if( mymod.Config.DebugModeInfo ) {
						LogHelpers.Log( "!ResetMode.PlayerLogic.RefundRewardsSpendings - '" + player.name + "' PP could not be set" );
					}
				}
			}

			mymod.Session.Data.PlayerPPSpendings[uid] = 0;
			if( Main.netMode != 1 ) {
				mymod.Session.Save();
			}

			if( mymod.Config.ResetRewardsKills ) {
				rewardsMod.Call( "ResetKills", player );
			}
		}
	}
}
