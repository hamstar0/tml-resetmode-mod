using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using ResetMode.NetProtocols;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModePlayer : ModPlayer {
		private void OnSingleConnect() {
			this.FinishModSettingsSync();
			this.FinishSessionSync();
		}

		private void OnCurrentClientConnect() {
			PacketProtocol.QuickRequestToServer<ModSettingsProtocol>();
			PacketProtocol.QuickRequestToServer<SessionProtocol>();
		}

		private void OnServerConnect( Player player ) {
			this.HasModSettings = true;
			this.HasSessionData = true;
		}


		////////////////

		public void FinishModSettingsSync() {
			this.HasModSettings = true;

			this.PostAnySync();
		}
		
		public void FinishSessionSync() {
			this.HasSessionData = true;

			this.PostAnySync();
		}

		////////////////
		
		public void PostAnySync() {
			if( this.IsSynced() && !this.HasEnteredWorld ) {
				this.HasEnteredWorld = true;
				
				this.Logic.OnEnterWorld( (ResetModeMod)this.mod, this.player );
			}
		}

		////////////////

		public bool IsSynced() {
			return this.HasModSettings && this.HasSessionData;
		}
	}
}
