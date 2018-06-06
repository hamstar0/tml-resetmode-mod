using HamstarHelpers.Utilities.Network;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class SessionProtocol : PacketProtocol {
		public ResetModeSessionData Data;

		////////////////

		public override void SetServerDefaults() {
			this.Data = ResetModeMod.Instance.Session.Data;
		}

		protected override void ReceiveWithClient() {
			ResetModeMod.Instance.Session.SetData( this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishSessionSync();
		}
	}
}
