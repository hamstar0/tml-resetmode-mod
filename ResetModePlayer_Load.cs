using HamstarHelpers.Components.Network;
using HamstarHelpers.DebugHelpers;
using ResetMode.NetProtocols;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModePlayer : ModPlayer {
		private void OnSingleConnect() {
			this.FinishModSettingsSync();
			this.FinishSessionSync();
		}

		private void OnClientConnect() {
			PacketProtocol.QuickRequestToServer<ModSettingsProtocol>();
			PacketProtocol.QuickRequestToServer<SessionProtocol>();
		}

		private void OnServerConnect() {
			this.HasModSettings = true;
			this.HasSessionData = true;
		}


		////////////////

		public void FinishModSettingsSync() {
			this.HasModSettings = true;

			this.UpdateSync();
		}
		
		public void FinishSessionSync() {
			this.HasSessionData = true;

			this.UpdateSync();
		}

		////////////////

		public bool IsSynced() {
			return this.HasModSettings && this.HasSessionData;
		}

		public void UpdateSync() {
			if( this.IsSynced() && !this.HasEnteredWorld ) {
				this.HasEnteredWorld = true;
				
				this.Logic.OnEnterWorld( (ResetModeMod)this.mod, this.player );
			}
		}
	}
}
