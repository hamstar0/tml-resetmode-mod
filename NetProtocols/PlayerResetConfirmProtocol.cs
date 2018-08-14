﻿using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class PlayerResetConfirmProtocol : PacketProtocol {
		private PlayerResetConfirmProtocol( PacketProtocolDataConstructorLock ctor_lock ) { }

		protected override void SetClientDefaults() { }
		protected override void SetServerDefaults() { }


		////////////////

		protected override bool ReceiveRequestWithServer( int from_who ) {
			var mymod = (ResetModeMod)ResetModeMod.Instance;
			Player player = Main.player[ from_who ];
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSessionForPlayer( mymod, player );
			myplayer.Logic.RefundRewardsSpendings( mymod, player );

			PacketProtocol.QuickRequestToClient<PlayerResetConfirmProtocol>( from_who, -1 );

			return true;
		}

		protected override bool ReceiveRequestWithClient() {
			var mymod = (ResetModeMod)ResetModeMod.Instance;
			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSessionForPlayer( mymod, player );
			myplayer.Logic.RefundRewardsSpendings( mymod, player );

			return true;
		}
	}
}
