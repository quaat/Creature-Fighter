using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace CreatureFighter
{
    class MenuWindow
    {
        private struct MenuItem
        {
            public string itemText;
            public MenuWindow itemLink;

            public MenuItem(string itemText, MenuWindow itemLink)
            {
                this.itemText = itemText;
                this.itemLink = itemLink;                
            }
        }

        public enum WindowState { Starting, Active, Ending, Inactive }

        private TimeSpan changeSpan;
        private WindowState windowState;
        private List<MenuItem> itemList;
        private int selectedItem;
        private SpriteFont spriteFont;
        private double changeProgress;
        private string menuTitle;
        private Texture2D backgroundImage;

        public MenuWindow(SpriteFont spriteFont, string menuTitle, Texture2D backgroundImage)
        {
            itemList = new List<MenuItem>();
            changeSpan = TimeSpan.FromMilliseconds(800.0);
            selectedItem = 0;
            changeProgress = 0;
            windowState = WindowState.Inactive;
            this.spriteFont = spriteFont;
            this.menuTitle = menuTitle;
            this.backgroundImage = backgroundImage;
        }

        public void AddMenuItem(string itemText, MenuWindow itemLink)
        {
            MenuItem item = new MenuItem(itemText, itemLink);
            itemList.Add(item);
        }

        public void WakeUp()
        {
            windowState = WindowState.Starting;
        }

        public void Update(double timePassedSinceLastTime)
        {
            if ((windowState == WindowState.Starting) || (windowState == WindowState.Ending))
                changeProgress += timePassedSinceLastTime / changeSpan.TotalMilliseconds;

            if (changeProgress >= 1.0f)
            {
                changeProgress = 0.0f;
                if (windowState == WindowState.Starting)
                    windowState = WindowState.Active;
                else if (windowState == WindowState.Ending)
                    windowState = WindowState.Inactive;
            }
        }

        float bgLayerDepth;
        Color bgColor;
        public void Draw(SpriteBatch spriteBatch)
        {
            if (windowState == WindowState.Inactive)
                return;

            float smoothedProgress = MathHelper.SmoothStep(0, 1, (float)changeProgress);
            int verPosition = 300;
            float horPosition = 300;
            float alphaValue;

            switch (windowState)
            {
                case WindowState.Starting:
                    horPosition -= 200f * (1.0f - (float)smoothedProgress);
                    alphaValue = smoothedProgress;
                    bgLayerDepth = 0.5f;
                    bgColor = new Color(new Vector4(1, 1, 1, alphaValue));
                    break;

                case WindowState.Ending:
                    horPosition += 200f * (float)smoothedProgress;
                    alphaValue = 1.0f * smoothedProgress;
                    bgLayerDepth = 1.0f;
                    bgColor = Color.White;
                    break;

                default:
                    alphaValue = 1.0f;
                    bgLayerDepth = 1;
                    bgColor = Color.White;
                    break;
            }

            
            Color titleColor = new Color(new Vector4(1, 1, 1, alphaValue));
            spriteBatch.Draw(backgroundImage, new Vector2(), null, bgColor, 0, Vector2.Zero, 1, SpriteEffects.None, bgLayerDepth);
            spriteBatch.DrawString(spriteFont, menuTitle, new Vector2(horPosition, 200), titleColor, 0, Vector2.Zero, 1.5f, SpriteEffects.None, 0);

            for (int itemID = 0; itemID < itemList.Count; itemID++)
            {
                Vector2 itemPosition = new Vector2(horPosition, verPosition);
                Color itemColor = Color.White;

                if (itemID == selectedItem)
                    itemColor = new Color(new Vector4(1, 0, 0, alphaValue));
                else
                    itemColor = new Color(new Vector4(1, 1, 1, alphaValue));

                spriteBatch.DrawString(spriteFont, itemList[itemID].itemText, itemPosition, itemColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                verPosition += 30;
            }
            
        }

#if XBOX       
        public MenuWindow ProcessInput(GamePadState lastGamePadState, GamePadState currentGamePadState)
        {
            if (lastGamePadState.IsButtonUp(Buttons.LeftThumbstickDown) && currentGamePadState.IsButtonDown(Buttons.LeftThumbstickDown))
                selectedItem++;
            if (lastGamePadState.IsButtonUp(Buttons.LeftThumbstickUp) && currentGamePadState.IsButtonDown(Buttons.LeftThumbstickUp))
                selectedItem--;
            if (selectedItem < 0) selectedItem = 0;
            if (selectedItem >= itemList.Count) selectedItem = itemList.Count - 1;
            if (lastGamePadState.IsButtonUp(Buttons.A) && currentGamePadState.IsButtonDown(Buttons.A))
            {
                windowState = WindowState.Ending;
                return itemList[selectedItem].itemLink;
            }
            else if (lastGamePadState.IsButtonDown(Buttons.Back)) return null;
            else return this;
        }
#endif
#if WINDOWS
        public MenuWindow ProcessInput(KeyboardState lastKbdState, KeyboardState currentKbdState)
        {
            if (lastKbdState.IsKeyUp(Keys.Down) && currentKbdState.IsKeyDown(Keys.Down))
                selectedItem++;
            if (lastKbdState.IsKeyUp(Keys.Up) && currentKbdState.IsKeyDown(Keys.Up))
                selectedItem--;
            if (selectedItem < 0) selectedItem = 0;
            if (selectedItem >= itemList.Count) selectedItem = itemList.Count - 1;
            if (lastKbdState.IsKeyUp(Keys.Enter) && currentKbdState.IsKeyDown(Keys.Enter))
            {
                windowState = WindowState.Ending;
                return itemList[selectedItem].itemLink;
            }
            else if (lastKbdState.IsKeyDown(Keys.Escape)) return null;
            else return this;
        }
#endif        
       
 
    }
}
