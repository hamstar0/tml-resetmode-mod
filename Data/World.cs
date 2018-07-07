using HamstarHelpers.Components.Errors;
using HamstarHelpers.DebugHelpers;
using HamstarHelpers.PlayerHelpers;
using System.Collections.Generic;
using Terraria;


namespace ResetMode.Data {
	public enum ResetModeWorldStatus {
		Normal,
		Active,
		Expired
	}



	public class ResetModeWorldData {
		private ResetModeSessionData Session;

		public ResetModeWorldStatus WorldStatus {
			get {
				if( !this.Session.AllPlayedWorlds.Contains(this.Session.CurrentWorld) ) {
					return ResetModeWorldStatus.Normal;
				}
				if( this.Session.CurrentWorld != "" ) {
					return ResetModeWorldStatus.Active;
				}
				return ResetModeWorldStatus.Expired;
			}
		}

		public ISet<string> WorldPlayers {
			get {
				return this.Session.PlayersValidated;
			}
		}

		public bool IsExiting = false;


		////////////////

		internal ResetModeWorldData( ResetModeSessionData session ) {
			this.Session = session;
		}


		////////////////

		internal bool IsPlaying( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );
			if( !has_uid ) {
				throw new HamstarException( "ResetMode - ResetModeWorldData.IsPlaying - Player has no uid." );
			}

			return this.WorldPlayers.Contains( uid );
		}


		internal void AddPlayer( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );
			if( !has_uid ) {
				throw new HamstarException( "ResetMode - ResetModeWorldData.AddPlayer - Player has no uid." );
			}

			this.WorldPlayers.Add( uid );
		}
	}
}
