﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Jump button for Mario
    /// </summary>
    public sealed class JumpButton : ActionButton
    {
        public JumpButton() : base("Jump")
        {
            ButtonImage = AssetManager.Instance.LoadAsset<Texture2D>("UI/Battle/JumpButton");
        }

        public override void OnSelected()
        {
            BattleUIManager.Instance.PushMenu(new JumpSubMenu());
        }

        public override void Draw()
        {
            SpriteRenderer.Instance.Draw(ButtonImage, Camera.Instance.SpriteToUIPos(new Vector2(-170, 50)), Color.White, false, .4f, true);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, Name, new Vector2(230, 320), Color.White, .45f);
        }
    }
}
