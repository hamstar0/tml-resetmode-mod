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

			var myplayer = Main.LocalPlayer.GetModPlayer<ResetModePlayer>();
			myplayer.FinishModSettingsSync();
		}
	}
}
