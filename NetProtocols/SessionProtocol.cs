using HamstarHelpers.Components.Errors;
using HamstarHelpers.Components.Network;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using ResetMode.Data;
using System;
using System.Collections.Concurrent;
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

		public ResetModeSessionData NewData;



		////////////////

		private SessionProtocol() { }

		protected SessionProtocol( Player player ) {
			this.PrepareDataForPlayer( player );
		}

		////

		protected override void SetServerDefaults( int to_who ) {
			Player plr = Main.player[ to_who ];

			if( plr == null || !plr.active ) {
				LogHelpers.Warn( "Player (" + to_who + ": " + ( plr == null ? "null" : plr.name ) + ") not available." );
				return;
			}

			this.PrepareDataForPlayer( plr );
		}


		////////////////
		
		protected override void ReceiveWithClient() {
			if( this.NewData == null ) {
				throw new HamstarException( "Invalid NewData." );
			}

			var mymod = ResetModeMod.Instance;

			mymod.Session.SetData( this.NewData );

			Player player = Main.LocalPlayer;
			var myplayer = player.GetModPlayer<ResetModePlayer>();

			myplayer.FinishSessionSync();
		}


		////////////////

		private void PrepareDataForPlayer( Player player ) {
			var mymod = ResetModeMod.Instance;

			try {
				string uid = PlayerIdentityHelpers.GetProperUniqueId( player );

				this.NewData = mymod.Session.Data.Clone();
				this.NewData.PlayersValidated = new HashSet<string>();
				this.NewData.PlayerPPSpendings = new ConcurrentDictionary<string, float>();

				if( mymod.Session.Data.PlayersValidated.Contains( uid ) ) {
					this.NewData.PlayersValidated.Add( uid );
				}

				if( mymod.Session.Data.PlayerPPSpendings.ContainsKey( uid ) ) {
					this.NewData.PlayerPPSpendings[uid] = mymod.Session.Data.PlayerPPSpendings[uid];
				}
			} catch( Exception e ) {
				LogHelpers.Log( "!ResetMode.SessionProtocol.PrepareDataForPlayer - Error: " + e.ToString() );
			}
		}
	}
}
