using HamstarHelpers.Classes.UI.Theme;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;


namespace ResetMode {
	class UIPromptTheme : UITheme {
		public new Color MainBgColor = new Color( 32, 96, 96, 192 );
		public new Color MainEdgeColor = new Color( 160, 255, 255, 192 );

		public new Color ButtonBgColor = new Color( 64, 128, 128, 192 );
		public new Color ButtonEdgeColor = new Color( 128, 160, 160, 192 );
		public new Color ButtonTextColor = new Color( 224, 224, 224, 224 );

		public new Color ButtonBgLitColor = new Color( 96, 160, 160, 192 );
		public new Color ButtonEdgeLitColor = new Color( 255, 255, 255, 255 );
		public new Color ButtonTextLitColor = new Color( 255, 255, 255, 255 );



		////////////////

		public override void ApplyPanel( UIPanel panel ) {
			panel.BackgroundColor = this.MainBgColor;
			panel.BorderColor = this.MainEdgeColor;
		}

		////////////////

		public override void ApplyButton<T>( UITextPanel<T> panel ) {
			panel.BackgroundColor = this.ButtonBgColor;
			panel.BorderColor = this.ButtonEdgeColor;
			panel.TextColor = this.ButtonTextColor;
		}

		public override void ApplyButtonLit<T>( UITextPanel<T> panel ) {
			panel.BackgroundColor = this.ButtonBgLitColor;
			panel.BorderColor = this.ButtonEdgeLitColor;
			panel.TextColor = this.ButtonTextLitColor;
		}
	}
}
