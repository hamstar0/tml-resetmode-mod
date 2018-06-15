using ResetMode.Data;
using ResetMode.Logic;
using Terraria;
using Terraria.ModLoader;


namespace ResetMode {
	partial class ResetModePlayer : ModPlayer {
		public PlayerLogic Logic;
		
		public bool HasModSettings { get; private set; }
		public bool HasSessionData { get; private set; }
		private bool HasEnteredWorld = false;



		////////////////

		public override bool CloneNewInstances { get { return false; } }

		public override void Initialize() {
			this.Logic = new PlayerLogic();
			this.HasModSettings = false;
			this.HasSessionData = false;
			this.HasEnteredWorld = false;
		}

		public override void clientClone( ModPlayer client_clone ) {
			var clone = (ResetModePlayer)client_clone;
			clone.HasModSettings = this.HasModSettings;
			clone.HasSessionData = this.HasSessionData;
			clone.HasEnteredWorld = this.HasEnteredWorld;
		}


		////////////////

		public override void SyncPlayer( int to_who, int from_who, bool new_player ) {
			var mymod = (ResetModeMod)this.mod;

			if( Main.netMode == 2 ) {
				if( to_who == -1 && from_who == this.player.whoAmI ) {
					this.OnServerConnect();
				}
			}
		}
		
		public override void OnEnterWorld( Player player ) {
			if( player.whoAmI != this.player.whoAmI ) { return; }

			var mymod = (ResetModeMod)this.mod;

			if( Main.netMode == 0 ) {
				if( !mymod.ConfigJson.LoadFile() ) {
					mymod.ConfigJson.SaveFile();
					ErrorLogger.Log( "Reset Mode config " + ResetModeConfigData.ConfigVersion.ToString() + " created (ModPlayer.OnEnterWorld())." );
				}
			}

			if( Main.netMode == 0 ) {
				this.OnSingleConnect();
			}
			if( Main.netMode == 1 ) {
				this.OnClientConnect();
			}
		}


		////////////////

		public override void PreUpdate() {
			var mymod = (ResetModeMod)this.mod;
			bool is_me = this.player.whoAmI == Main.myPlayer;

			if( is_me && this.IsSynced() ) {
				this.Logic.Update( mymod, this.player );
			}
		}
	}
}
