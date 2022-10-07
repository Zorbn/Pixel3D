using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pixel3D;

public struct RaycastHit
{
    public int Tile;
    public float Distance;
    public bool HitXSide;
}

public class Game1 : Game
{
    private const int ScreenWidth = 320;
    private const int ScreenHeight = 180;
    private const float Scale = 2f;
    private static readonly Vector2 ScreenPos = new(0, (ScreenHeight - 1) * Scale);
    private const float PiOver2 = MathF.PI / 2;
    private const int CenterY = ScreenHeight / 2;
    private const int WallHeight = ScreenHeight;

    private GraphicsDeviceManager graphics;
    private Color[] screenPixels;
    private Texture2D screenTexture;
    private SpriteBatch spriteBatch;

    private static readonly Color[] texture =
    {
        Color.Black, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.Red,
        Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.Black
    };

    private static readonly int[] map =
    {
        2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,2,
        2,0,0,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,1,1,0,0,0,2,
        2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
    };

    private const int MapSize = 24;

    private const float PlayerSpeed = 1f;
    private const float PlayerRotSpeed = 1f;
    
    private float playerX = 2.1f;
    private float playerY = 2.1f;
    private float playerDir = MathF.PI / 8;

    public int FrameCount;
    public double ElapsedTime;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        IsFixedTimeStep = false;
        graphics.SynchronizeWithVerticalRetrace = false;
    }

    protected override void Initialize()
    {
        screenTexture = new Texture2D(GraphicsDevice, ScreenHeight, ScreenWidth);
        screenPixels = new Color[ScreenWidth * ScreenHeight];

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            keyboardState.IsKeyDown(Keys.Escape))
            Exit();

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        // Console.WriteLine($"{1f / deltaTime}");

        FrameCount++;
        ElapsedTime = gameTime.TotalGameTime.TotalSeconds;
        
        float playerForwardX = MathF.Cos(playerDir);
        float playerForwardY = MathF.Sin(playerDir);

        if (keyboardState.IsKeyDown(Keys.Up))
        {
            playerX += playerForwardX * deltaTime * PlayerSpeed;
            playerY += playerForwardY * deltaTime * PlayerSpeed;
        }

        if (keyboardState.IsKeyDown(Keys.Down))
        {
            playerX -= playerForwardX * deltaTime * PlayerSpeed;
            playerY -= playerForwardY * deltaTime * PlayerSpeed;
        }

        playerX = Math.Clamp(playerX, 0, MapSize);
        playerY = Math.Clamp(playerY, 0, MapSize);

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            playerDir -= PlayerRotSpeed * deltaTime;
        }

        if (keyboardState.IsKeyDown(Keys.Right))
        {
            playerDir += PlayerRotSpeed * deltaTime;
        }

        playerDir %= MathF.PI * 2;

        // var screenCenter = new Point(ScreenWidth / 2, ScreenHeight / 2);

        // for (var y = 0; y < ScreenHeight; y++)
        // {
        //     float distToCenter = Math.Abs(y - ScreenHeight * 0.5f);
        //     float cameraHeight = 0.5f * ScreenHeight;
        //     float distance = cameraHeight / distToCenter;
        //
        //     for (var x = 0; x < ScreenWidth; x++)
        //     {
        //         Color color = (int)MathF.Floor(distance) % 2 == 0 ? Color.CornflowerBlue : Color.Aqua;
        //         SetPixel(x, y, color);
        //     }
        // }

        int centerY = ScreenHeight / 2;
        int wallHeight = ScreenHeight;
        for (var x = 0; x < ScreenWidth; x++)
        {
            // Color color = GetMapTile(x, y) == 1 ? Color.Blue : Color.Black;
            float rayDir = playerDir - (MathF.PI * 0.25f * 0.5f) + MathF.PI * 0.25f * (x / (float)ScreenWidth);
            RaycastHit hit = CastRay(playerX, playerY, rayDir);
            
            float columnHeight = wallHeight / hit.Distance;
            int startY = centerY - (int)(columnHeight * 0.5f);
            int endY = centerY + (int)(columnHeight * 0.5f);
            
            Color color = hit.Tile == 1 ? Color.Purple : Color.Orange;
            
            if (!hit.HitXSide)
            {
                color = new Color(color.R / 2, color.G / 2, color.B / 2);
            }
            
            // TODO: Also draw floor and ceiling on either side of the wall.
            
            for (var y = startY; y < endY; y++)
            {
                // Color color = (int)MathF.Floor(distance) % 2 == 0 ? Color.CornflowerBlue : Color.Aqua;
                SetPixel(x, y, color);
            }
        }

        // Parallel.For(0, ScreenWidth, ParallelOptions, x =>
        // {
        //     float rayDir = playerDir - (MathF.PI * 0.25f * 0.5f) + MathF.PI * 0.25f * (x / (float)ScreenWidth);
        //     RaycastHit hit = CastRay(playerX, playerY, rayDir);
        //     
        //     float columnHeight = wallHeight / hit.Distance;
        //     int startY = centerY - (int)(columnHeight * 0.5f);
        //     int endY = centerY + (int)(columnHeight * 0.5f);
        //     
        //     Color color = hit.Tile == 1 ? Color.Purple : Color.Orange;
        //     
        //     if (!hit.HitXSide)
        //     {
        //         color = new Color(color.R / 2, color.G / 2, color.B / 2);
        //     }
        //     
        //     for (var y = startY; y < endY; y++)
        //     {
        //         SetPixel(x, y, color);
        //     }
        // });

        for (var x = 0; x < MapSize; x++)
        {
            for (var y = 0; y < MapSize; y++)
            {
                SetPixel(x, y, GetMapTile(x, y) == 1 ? Color.Blue : Color.Black);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            float x = playerX + playerForwardX * i;
            float y = playerY + playerForwardY * i;
            SetPixel((int)x, (int)y, Color.Red);
        }
        
        SetPixel((int)playerX, (int)playerY, Color.White);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        screenTexture.SetData(screenPixels);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(screenTexture, ScreenPos, null, Color.White, -PiOver2, Vector2.One,
            Scale, SpriteEffects.FlipHorizontally, 0f);
        spriteBatch.End();

        base.Draw(gameTime);
    }

    private int GetMapTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= MapSize || y >= MapSize) return 1;

        return map[x + y * MapSize];
    }

    private void SetPixel(int x, int y, Color color)
    {
        if (x < 0 || y < 0 || x >= ScreenWidth || y >= ScreenHeight) return;

        screenPixels[y + x * ScreenHeight] = color;
    }

    private RaycastHit CastRay(float startX, float startY, float dir)
    {
        float rayDirX = MathF.Cos(dir);
        float rayDirY = MathF.Sin(dir);
        int rayTileDirX = MathF.Sign(rayDirX);
        int rayTileDirY = MathF.Sign(rayDirY);

        // StepX is the distance the ray is required to travel along it's direction in order to move by 1 unit of x.
        // To find this distance pythagoras's theorem is used (the distance is a hypotenuse). One of the side lengths is
        // 1^2, aka 1. The other side length is the ray needs to travel on the y axis to travel one unit on the x axis, which
        // is rayDirY / rayDirX. So the formula is stepX^2 = 1^2 + (rayDirY / rayDirX)^2. The same formula roughly applies to stepY.
        // float stepX = MathF.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX));
        // float stepY = MathF.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY));
        // Since only the ratio between rayDirX and rayDirY matters, the following values can be used instead:
        float stepX = MathF.Abs(1 / rayDirX);
        float stepY = MathF.Abs(1 / rayDirY);
        
        // Distance of partial steps along the direction of the ray required to reach the next grid cell from the offset
        // within the starting grid cell.
        float initialStepX;
        float initialStepY;
        
        if (rayTileDirX > 0)
        {
            // Find the distance to the start of the next tile. That is the distance in the x direction.
            // Multiply that distance by stepX to find the distance along the ray. This same method is used for the
            // rest of the initialStep calculations.
            initialStepX = (MathF.Ceiling(startX) - startX) * stepX;
        }
        else
        {
            initialStepX = (startX - MathF.Floor(startX)) * stepX;
        }

        if (rayTileDirY > 0)
        {
            initialStepY = (MathF.Ceiling(startY) - startY) * stepY;
        }
        else
        {
            initialStepY = (startY - MathF.Floor(startY)) * stepY;
        }

        // Every time the ray moves one tile in either the x or y direction, keep track of the distance along it's direction
        // that the ray would need to move from it's origin to reach the next tile in that direction. Of the two directions,
        // always move in the direction that would require the ray to move the smallest distance from it's starting position
        // to get there. Ie: If the distance between the ray's starting point and the next x tile is larger than the distance
        // between the ray's starting point and the next y tile, move along the y direction next. Store the distance the
        // ray needed to move to get to that tile. Now repeat.
        float distanceFromStartToNextX = initialStepX;
        float distanceFromStartToNextY = initialStepY;

        int tileX = (int)MathF.Floor(startX);
        int tileY = (int)MathF.Floor(startY);

        int hitTile = 0;
        bool lastMovedOnX = false;
        while (hitTile == 0)
        {
            if (distanceFromStartToNextX < distanceFromStartToNextY)
            {
                tileX += rayTileDirX;
                distanceFromStartToNextX += stepX;
                lastMovedOnX = true;
            }
            else
            {
                tileY += rayTileDirY;
                distanceFromStartToNextY += stepY;
                lastMovedOnX = false;
            }

            hitTile = GetMapTile(tileX, tileY);
        }

        // Find the distance to current x or y value depending on the side that was hit.
        // The distance is along the direction of the ray. To avoid a fish eye effect where rays on the edges of the
        // screen travel farther from the player than rays in the center (ie: when looking at a flat wall), calculate
        // the distance from the hit to the camera plane (just a line in 2D).
        if (lastMovedOnX)
        {
            float distanceFromStartToCurrentX = distanceFromStartToNextX - stepX;
            float x = startX + rayDirX * distanceFromStartToCurrentX;
            float y = startY + rayDirY * distanceFromStartToCurrentX;
            float screenAngle = playerDir + PiOver2;

            float distToCamera = MathF.Abs(MathF.Cos(screenAngle) * (playerY - y) - MathF.Sin(screenAngle) * (playerX - x));

            return new RaycastHit
            {
                Tile = hitTile,
                Distance = distToCamera,
                HitXSide = true
            };
        }
        else
        {

            float distanceFromStartToCurrentY = distanceFromStartToNextY - stepY;
            float x = startX + rayDirX * distanceFromStartToCurrentY;
            float y = startY + rayDirY * distanceFromStartToCurrentY;
            float screenAngle = playerDir + PiOver2;

            float distToCamera = MathF.Abs(MathF.Cos(screenAngle) * (playerY - y) - MathF.Sin(screenAngle) * (playerX - x));
            
            return new RaycastHit
            {
                Tile = hitTile,
                Distance = distToCamera,
                HitXSide = false
            };
        }
    }
}