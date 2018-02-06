using HamstarHelpers.Helpers.PlayerHelpers;
using HamstarHelpers.Utilities.Errors;
using Terraria;


namespace ResetMode.Logic {
	partial class WorldLogic {
		public void AddPlayer( ResetModeMod mymod, Player player ) {
			bool has_uid;
			string uid = PlayerIdentityHelpers.GetUniqueId( player, out has_uid );

			if( !has_uid ) { throw new HamstarException( "AddPlayer - Player has no uid." ); }

			this.WorldPlayers.Add( uid );
		}
	}
}
