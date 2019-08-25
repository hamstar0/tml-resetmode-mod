using HamstarHelpers.Classes.Errors;
using HamstarHelpers.Classes.Protocols.Packet.Interfaces;
using HamstarHelpers.Helpers.Debug;
using HamstarHelpers.Helpers.Players;
using HamstarHelpers.Helpers.TModLoader;
using ResetMode.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Terraria;


namespace ResetMode.NetProtocols {
	class SessionProtocol : PacketProtocolSyncClient {
		public static void SyncToMe() {
			PacketProtocolSyncClient.SyncToMe<SessionProtocol>( -1 );
		}

		public static void SyncToClients() {
			for( int i = 0; i < Main.player.Length; i++ ) {
				Player plr = Main.player[i];
				if( plr == null || !plr.active ) { continue; }

				var protocol = new SessionProtocol( plr );
				protocol.SendToClient( i, -1 );
			}
		}

		public static void SyncToClient( int clientWho ) {
			PacketProtocolSyncClient.SyncFromServer<SessionProtocol>( clientWho, -1 );
		}



		////////////////

		public ResetModeSessionData NewData;



		////////////////

		private SessionProtocol() { }

		protected SessionProtocol( Player player ) {
			this.PrepareDataForPlayer( player );
		}

		////

		protected override void InitializeClientSendData() {
			throw new NotImplementedException();
		}

		protected override void InitializeServerRequestReplyDataOfClient( int toWho, int fromWho ) {
			Player plr = Main.player[toWho];

			if( plr == null /*|| !plr.active*/ ) {
				LogHelpers.Warn( "Player (" + toWho + ": " + ( plr == null ? "null" : plr.name ) + ") not available." );
				return;
			}

			string uid = PlayerIdentityHelpers.GetUniqueId( plr );
			if( uid == null ) {
				return;
			}

			this.PrepareDataForPlayer( plr );
		}


		////////////////

		//protected override bool ReceiveRequestWithServer( int fromWho ) {
		protected override void ReceiveOnServer( int fromWho ) {
			//return this.NewData == null;
		}

		////////////////

		protected override void ReceiveOnClient() {
			if( this.NewData == null ) {
				throw new ModHelpersException( "Invalid NewData." );
			}

			var mymod = ResetModeMod.Instance;

			mymod.Session.SetData( this.NewData );

			Player player = Main.LocalPlayer;
			var myplayer = (ResetModePlayer)TmlHelpers.SafelyGetModPlayer( player, ResetModeMod.Instance, "ResetModePlayer" );

			myplayer.FinishSessionSyncWithLocal();
		}


		////////////////

		private void PrepareDataForPlayer( Player player ) {
			var mymod = ResetModeMod.Instance;
			string uid = null;

			try {
				uid = PlayerIdentityHelpers.GetUniqueId( player );

				this.NewData = mymod.Session.Data.Clone();
				this.NewData.PlayersValidated = new HashSet<string>();
				this.NewData.PlayerPPSpendings = new ConcurrentDictionary<string, float>();
			} catch( Exception e ) {
				LogHelpers.Warn( "Error 1: " + e.ToString() );
			}

			try {
				if( mymod.Session.Data.PlayersValidated.Contains( uid ) ) {
					this.NewData.PlayersValidated.Add( uid );
				}
			} catch( Exception e ) {
				LogHelpers.Warn( "Error 2: " + e.ToString() );
			}

			try {
				if( mymod.Session.Data.PlayerPPSpendings.ContainsKey( uid ) ) {
					this.NewData.PlayerPPSpendings[ uid ] = mymod.Session.Data.PlayerPPSpendings[ uid ];
				}
			} catch( Exception e ) {
				LogHelpers.Warn( "Error 3a: " + e.ToString() );
			}
		}
	}
}
