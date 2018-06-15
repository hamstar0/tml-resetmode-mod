using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Utilities.Network;
using ResetMode.Data;
using ResetMode.NetProtocols;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModePlayer : ModPlayer {
		private void OnSingleConnect() {
			this.LoadGame();
			this.FinishModSettingsSync();
			this.FinishSessionSync();
		}

		private void OnClientConnect() {
			PacketProtocol.QuickRequestToServer<ModSettingsProtocol>();
			PacketProtocol.QuickRequestToServer<SessionProtocol>();
		}

		private void OnServerConnect() {
			this.HasModSettings = true;
			this.LoadGame();
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


		////////////////

		private void LoadGame() {
			var mymod = (ResetModeMod)this.mod;
			var myworld = mymod.GetModWorld<ResetModeWorld>();
			
			if( mymod.Session.Data.IsRunning ) {
				if( myworld.Data.WorldStatus != ResetModeStatus.Normal ) {
					this.Logic.ValidatePlayer( mymod, this.player );
				}
			}
		}
	}
}
