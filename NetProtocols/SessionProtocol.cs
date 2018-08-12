using HamstarHelpers.Components.Network;
using HamstarHelpers.Components.Network.Data;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using ResetMode.Data;
using System.Collections.Generic;
using Terraria;


namespace ResetMode.NetProtocols {
	class SessionProtocol : PacketProtocol {
		public static void SyncToClients() {
			for( int i=0; i<Main.player.Length; i++ ) {
				Player plr = Main.player[i];
				if( plr == null || !plr.active ) { continue; }

				var protocol = new SessionProtocol( plr );
				protocol.SendToClient( i, -1 );
			}
		}


		////////////////

		public ResetModeSessionData Data;


		////////////////

		private SessionProtocol( PacketProtocolDataConstructorLock ctor_lock ) { }

		private SessionProtocol( Player player ) {
			this.PrepareDataForPlayer( player );
		}

		protected override void SetServerDefaults() { }


		////////////////

		private void PrepareDataForPlayer( Player player ) {
			var mymod = ResetModeMod.Instance;

			bool success;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out success );
			if( !success ) {
				LogHelpers.Log( "!ResetMode.SessionProtocol.PrepareDataForPlayer - Could not get player id. (" + player.name+" ("+player.whoAmI+") active? "+player.active+")" );
				return;
			}

			this.Data = mymod.Session.Data.Clone();
			this.Data.PlayersValidated = new HashSet<string>();
			this.Data.PlayerPPSpendings = new Dictionary<string, float>();

			if( mymod.Session.Data.PlayersValidated.Contains( uid ) ) {
				this.Data.PlayersValidated.Add( uid );
			}

			if( mymod.Session.Data.PlayerPPSpendings.ContainsKey( uid ) ) {
				this.Data.PlayerPPSpendings[ uid ] = this.Data.PlayerPPSpendings[ uid ];
			}
		}


		////////////////

		protected override bool ReceiveRequestWithServer( int from_who ) {
			if( Main.player[from_who] == null || !Main.player[from_who].active ) {
				LogHelpers.Log( "!ResetMode.SessionProtocol.ReceiveRequestWithServer - Player not available." );
				return true;
			}

			this.PrepareDataForPlayer( Main.player[ from_who ] );

			return false;
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
