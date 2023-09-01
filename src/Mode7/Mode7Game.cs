using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Mode7.CameraEngine
{
    public class Mode7Game : Game
    {
        const float CameraSpeed = 500f;
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        Mode7Camera _camera;
        Texture2D _map;

        public Mode7Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _map = Content.Load<Texture2D>("map");

            // Position the camera at the start line and zoom in plenty
            _camera = new Mode7Camera(
                viewport: GraphicsDevice.Viewport,
                position: new Vector2(952, 654),
                rotation: MathHelper.ToRadians(MathHelper.PiOver4), 
                zoom: 12f);
        }

        protected override void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Add some basic controls to zoom/rotate/move the camera
            var rotationDelta = 0f;
            var zoomDelta = 0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Subtract)) zoomDelta += delta * 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Add)) zoomDelta -= delta * 1;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) rotationDelta += 0.01f * CameraSpeed * delta;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) rotationDelta -= 0.01f * CameraSpeed * delta;
            _camera.Zoom += zoomDelta;
            _camera.Rotation += rotationDelta;

            // Use the rotation to calculate the direction we want to move in with sin and cos
            var sin = MathF.Sin(-_camera.Rotation);
            var cos = MathF.Cos(-_camera.Rotation);
            var positionDelta = Vector2.Zero;

            // Move the camera in the direction of the rotation
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                positionDelta.X += sin * CameraSpeed * delta;
                positionDelta.Y += -cos * CameraSpeed * delta;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                positionDelta.X += -sin * CameraSpeed * delta;
                positionDelta.Y += cos * CameraSpeed * delta;
            }

            _camera.Position += positionDelta;

            // Exit the Matrix
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var title = $"Mode7 Camera Engine - FPS: {1 / gameTime.ElapsedGameTime.TotalSeconds:0}";
            title += " - Zoom: " + _camera.Zoom.ToString("0.00");
            Window.Title = title;

            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Use the camera to transform the sprite batch
            _spriteBatch.Begin(transformMatrix: _camera.WorldToScreen());

            _spriteBatch.Draw(_map, Vector2.Zero, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}