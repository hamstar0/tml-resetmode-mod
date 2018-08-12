using HamstarHelpers.Components.Errors;
using HamstarHelpers.Helpers.DebugHelpers;
using HamstarHelpers.Helpers.PlayerHelpers;
using Terraria;


namespace ResetMode.Logic {
	partial class SessionLogic {
		internal bool IsPlaying( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );
			if( !has_uid ) {
				throw new HamstarException( "!ResetMode.SessionLogic.IsPlaying - Player has no uid." );
			}

			return this.Data.PlayersValidated.Contains( uid );
		}


		internal void AddPlayer( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );
			if( !has_uid ) {
				throw new HamstarException( "!ResetMode.SessionLogic.AddPlayer - Player has no uid." );
			}

			this.Data.PlayersValidated.Add( uid );
			if( Main.netMode != 1 ) {
				this.Save( mymod );
			}
		}
	}
}
