using HamstarHelpers.Utilities.Network;
using ResetMode.Data;
using Terraria;


namespace ResetMode {
	class ResetModeModSettingsProtocol : PacketProtocol {
		public ResetModeConfigData Data;

		////////////////

		public override void SetDefaults() {
			this.Data = ResetModeMod.Instance.Config;
		}

		public override void ReceiveOnClient() {
			ResetModeMod.Instance.ConfigJson.SetData( this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishModSettingsSync();
		}
	}



	class ResetModePlayerResetProtocol : PacketProtocol {
		public override void ReceiveRequestOnClient() {
			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.PromptReset( ResetModeMod.Instance, Main.LocalPlayer );
		}
	}


	class ResetModePlayerResetConfirmProtocol : PacketProtocol {
		public override void ReceiveRequestOnServer( int from_who ) {
			Player player = Main.player[ from_who ];
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.Logic.BeginSession( ResetModeMod.Instance, player );
		}
	}
}
