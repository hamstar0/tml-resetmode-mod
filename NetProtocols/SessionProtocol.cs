using HamstarHelpers.Components.Network;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class SessionProtocol : PacketProtocol {
		public ResetModeSessionData Data;


		////////////////

		public override void SetServerDefaults() {
			var mymod = ResetModeMod.Instance;

			this.Data = mymod.Session.SessionData;
		}

		////////////////

		protected override void ReceiveWithClient() {
			var mymod = ResetModeMod.Instance;

			mymod.Session.SetData( mymod, this.Data );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishSessionSync();
		}
	}
}
