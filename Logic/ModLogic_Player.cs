using HamstarHelpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using ResetMode.Data;
using Rewards;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;


namespace ResetMode.Logic {
	partial class ModLogic {
		private static bool GetPlayerInfo( Player player, out string uid, out int hash ) {
			bool loaded;
			uid = PlayerIdentityHelpers.GetUniqueId( player, out loaded );
			hash = PlayerLogic.GetHash( player );
			return loaded;
		}

		private static bool ComparePP( ResetModeMod mymod, string uid, int pp ) {
			return mymod.Session.PlayerPP.ContainsKey( uid ) ?
				pp == mymod.Session.PlayerPP[uid] :
				pp == 0;
		}



		////////////////

		internal IDictionary<int, bool> HasPPChanged = new Dictionary<int, bool>();


		////////////////

		public void LoadPlayerData( ResetModeMod mymod, Player player ) {
			bool loaded;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out loaded );

			int pp = (int)RewardsAPI.GetPoints( player );

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.LoadPlayerData player: " + player.whoAmI + ", loaded:"+loaded+", uid: " + uid + ", pp: " + pp );
			}
			if( !loaded ) { throw new Exception( "Not loaded" ); }

			this.HasPPChanged[player.whoAmI] = !ModLogic.ComparePP( mymod, uid, pp );

			mymod.Session.PlayerPP[uid] = pp;

			if( mymod.Logic.NetMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}
		}

		////////////////

		public void NetSendPlayerData( ResetModeMod mymod, Player player, BinaryWriter writer ) {
			int pp = (int)RewardsAPI.GetPoints( player );

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.NetSendPlayerData player: " + player.whoAmI+", pp: "+ pp );
			}

			writer.Write( pp );
		}

		public void NetReceivePlayerData( ResetModeMod mymod, Player player, BinaryReader reader ) {
			bool loaded;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out loaded );

			int pp = reader.ReadInt32();

			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.NetReceivePlayerData player: " + player.whoAmI + ", loaded:" + loaded + ", uid: " + uid + ", pp: " + pp );
			}
			if( !loaded ) { throw new Exception( "Not loaded" ); }

			this.HasPPChanged[player.whoAmI] = !ModLogic.ComparePP( mymod, uid, pp );

			mymod.Session.PlayerPP[uid] = pp;

			if( mymod.Logic.NetMode != 1 ) {
				mymod.SessionJson.SaveFile();
			}
		}


		////////////////
		
		public bool IsPlayerSessionSynced( ResetModeMod mymod, Player player, out bool has_pp_changed ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.IsPlayerSessionSynced player: " + player.whoAmI );
			}

			has_pp_changed = false;
			string uid;
			int hash;

			if( !ModLogic.GetPlayerInfo( player, out uid, out hash ) ) {
				return true;
			}

			this.HasPPChanged.TryGetValue( player.whoAmI, out has_pp_changed );

			bool no_saves = mymod.Session.PlayerHashes.ContainsKey( uid );
			if( !no_saves ) { return true; }
			
			return mymod.Session.PlayerHashes[uid] == hash;
		}


		public void RegisterPlayerDataWithSession( ResetModeMod mymod, Player player ) {
			if( mymod.Config.DebugModeInfo ) {
				LogHelpers.Log( "ModLogic.RegisterPlayerDataWithSession player: " + player.whoAmI );
			}

			var myplayer = player.GetModPlayer<ResetModePlayer>();
			string uid;
			int hash;

			if( !ModLogic.GetPlayerInfo( player, out uid, out hash ) ) {
				return;
			}

			mymod.Session.PlayerHashes[ uid ] = hash;

			if( mymod.Logic.NetMode == 0 ) {
				int pp = (int)RewardsAPI.GetPoints( player );

				this.HasPPChanged[ player.whoAmI ] = !ModLogic.ComparePP( mymod, uid, pp );

				mymod.Session.PlayerPP[ uid ] = pp;
			}

			mymod.SessionJson.SaveFile();
		}
	}
}
