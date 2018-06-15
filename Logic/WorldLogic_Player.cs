using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.Utilities.Errors;
using Terraria;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public bool IsPlaying( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );

			if( !has_uid ) { throw new HamstarException( "ResetMode - WorldLogic.AddPlayer - Player has no uid." ); }
			
			return this.WorldPlayers.Contains( uid );
		}


		public void AddPlayer( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );

			if( !has_uid ) { throw new HamstarException( "ResetMode - WorldLogic.AddPlayer - Player has no uid." ); }

			this.WorldPlayers.Add( uid );
		}
	}
}
