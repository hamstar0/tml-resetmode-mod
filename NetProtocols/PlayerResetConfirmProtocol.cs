using HamstarHelpers.Components.Network;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetConfirmProtocol : PacketProtocol { //PacketProtocolRequestToEither {
		private PlayerResetConfirmProtocol() { }

		protected override void SetClientDefaults() { }
		protected override void SetServerDefaults( int toWho ) { }


		////////////////

		protected override bool ReceiveRequestWithServer( int fromWho ) {
			var mymod = (ResetModeMod)ResetModeMod.Instance;
			Player player = Main.player[ fromWho ];
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSessionForPlayer( player );
			myplayer.Logic.RefundRewardsSpendings( player );

			PacketProtocol.QuickRequestToClient<PlayerResetConfirmProtocol>( fromWho, -1, 0 );

			return true;
		}

		protected override bool ReceiveRequestWithClient() {
			var mymod = (ResetModeMod)ResetModeMod.Instance;
			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSessionForPlayer( player );
			myplayer.Logic.RefundRewardsSpendings( player );

			return true;
		}
	}
}
