using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetConfirmProtocol : PacketProtocolRequestToServer {
		private PlayerResetConfirmProtocol() { }
		
		protected override void InitializeServerSendData( int toWho ) { }


		////////////////

		protected override bool ReceiveRequestWithServer( int fromWho ) {
			Player player = Main.player[ fromWho ];
			if( player == null ) {
				LogHelpers.Warn( "Invalid player (who:"+fromWho+")" );
				return true;
			}

			string uid = PlayerIdentityHelpers.GetProperUniqueId( player );
			if( uid == null ) {
				LogHelpers.Warn( "Missing player UID for player "+player.name+" ("+fromWho+")" );
				return true;
			}

			var mymod = (ResetModeMod)ResetModeMod.Instance;
			var myplayer = player.GetModPlayer<ResetModePlayer>();
			
			myplayer.Logic.BeginSessionForPlayer( player );
			myplayer.Logic.RefundRewardsSpendings( player );

			return false;
		}

		protected override void ReceiveReply() {
			var mymod = (ResetModeMod)ResetModeMod.Instance;
			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSessionForPlayer( player );
			myplayer.Logic.RefundRewardsSpendings( player );
		}
	}
}
