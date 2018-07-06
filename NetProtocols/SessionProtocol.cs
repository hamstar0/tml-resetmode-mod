﻿using HamstarHelpers.Components.Network;
using ResetMode.Data;
using Terraria;


namespace ResetMode.NetProtocols {
	class SessionProtocol : PacketProtocol {
		public ResetModeSessionData Data;
		public int Status;


		////////////////

		public override void SetServerDefaults() {
			var mymod = ResetModeMod.Instance;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			this.Data = mymod.Session.Data;
			this.Status = (int)myworld.Data.WorldStatus;
		}

		////////////////

		protected override void ReceiveWithClient() {
			var mymod = ResetModeMod.Instance;
			var myworld = mymod.GetModWorld<ResetModeWorld>();

			mymod.Session.SetData( mymod, this.Data, (ResetModeWorldStatus)this.Status );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishSessionSync();
		}
	}
}
