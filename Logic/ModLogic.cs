using HamstarHelpers.Helpers.PlayerHelpers;
using System.IO;
using Terraria;
using TimeLimit;


namespace ResetMode.Logic {
	class ModLogic {
		public bool IsSessionStarted( ResetModeMod mymod ) {
			return mymod.Session.IsRunning;
		}


		////////////////

		public void StartSession( ResetModeMod mymod ) {
			int time = mymod.Session.AwaitingNextWorld ? mymod.Config.SecondsUntilResetSubsequently : mymod.Config.SecondsUntilResetInitially;

			mymod.Session.IsRunning = true;
			mymod.SessionJson.SaveFile();

			TimeLimitAPI.TimerStart( "reset", time, false );
		}


		public void StopSession( ResetModeMod mymod ) {
			if( !this.IsSessionStarted( mymod ) ) { return; }

			mymod.Session.ClearSession();
			mymod.SessionJson.SaveFile();

			TimeLimitAPI.TimerStop( "reset" );
		}


		////////////////

		public void NetSendPlayerData( Player player, BinaryWriter writer ) {
			//TODO
		}

		public void NetReceivePlayerData( Player player, BinaryReader reader ) {
			//TODO
		}


		////////////////

		private void GetPlayerInfo( Player player, out string uid, out int hash ) {
			bool _;
			uid = PlayerIdentityHelpers.GetUniqueId( player, out _ );
			hash = PlayerIdentityHelpers.GetVanillaSnapshotHash( player, true, false );
		}


		public void SavePlayerSnapshot( ResetModeMod mymod, Player player ) {
			if( Main.netMode != 1 ) {
				string uid;
				int hash;
				this.GetPlayerInfo( player, out uid, out hash );

				mymod.Session.PlayerHashes[uid] = hash;
				mymod.SessionJson.SaveFile();
			}
		}


		public bool IsPlayerSessionSynced( ResetModeMod mymod, Player player ) {
			string uid;
			int hash;
			this.GetPlayerInfo( player, out uid, out hash );

			return !mymod.Session.PlayerHashes.ContainsKey( uid ) || mymod.Session.PlayerHashes[uid] != hash;
		}
	}
}
