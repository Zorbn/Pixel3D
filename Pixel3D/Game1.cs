using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pixel3D;

public struct Hit
{
    public float Distance;
    public int Tile;
    public bool XSide;
}

public class Game1 : Game
{
    private const int ScreenWidth = 320;
    private const int ScreenHeight = 180;

    private GraphicsDeviceManager graphics;
    private Color[] screenPixels;
    private int[] pixelDepth;
    private Texture2D screenTexture;
    private SpriteBatch spriteBatch;

    private static readonly int[] Map =
    {
        2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,
        2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
    };
    
    private static readonly Color[] texture =
    {
	    Color.Black, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.Red,
	    Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.White, Color.Black
    };

    private static readonly Color[] TileColors = { Color.White, Color.Orange, Color.Purple };

    private const int MapSize = 24;

    private float playerX = 0f;
    private float playerY = 0f;
    private float playerDir = 0f;

    private float playerSpeed = 1f;
    private float playerRotSpeed = 1f;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        screenTexture = new Texture2D(GraphicsDevice, ScreenWidth, ScreenHeight);
        screenPixels = new Color[ScreenWidth * ScreenHeight];
        pixelDepth = new int[ScreenWidth * ScreenHeight];

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyState = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            keyState.IsKeyDown(Keys.Escape))
            Exit();

        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // float moveFwd = 0f;
        //
        // if (keyState.IsKeyDown(Keys.Up))
        // {
        //     moveFwd += 1.0f;
        // }
        //
        // if (keyState.IsKeyDown(Keys.Down))
        // {
        //     moveFwd -= 1.0f;
        // }
        //
        // if (keyState.IsKeyDown(Keys.Left))
        // {
        //     playerDir -= playerRotSpeed * deltaTime;
        // }
        //
        // if (keyState.IsKeyDown(Keys.Right))
        // {
        //     playerDir += playerRotSpeed * deltaTime;
        // }
        //
        // playerDir %= MathF.PI * 2;
        //
        // float playerForwardX = MathF.Cos(playerDir);
        // float playerForwardY = MathF.Sin(playerDir);
        //
        // playerX += playerForwardX * moveFwd * deltaTime * playerSpeed;
        // playerY += playerForwardY * moveFwd * deltaTime * playerSpeed;
        
        float playerForwardX = MathF.Sin(playerDir);
        float playerForwardY = MathF.Cos(playerDir);
        float playerRightX = MathF.Sin(playerDir + MathF.PI * 0.5f);
	    float playerRightY = MathF.Cos(playerDir + MathF.PI * 0.5f);
        
        if (keyState.IsKeyDown(Keys.Up))
        {
	        playerX += playerForwardX * deltaTime * playerSpeed;
	        playerY += playerForwardY * deltaTime * playerSpeed;
        }
        
        if (keyState.IsKeyDown(Keys.Down))
        {
	        playerX -= playerForwardX * deltaTime * playerSpeed;
	        playerY -= playerForwardY * deltaTime * playerSpeed;
        }
        
        if (keyState.IsKeyDown(Keys.Left))
        {
	        playerX -= playerRightX * deltaTime * playerSpeed;
	        playerY -= playerRightY * deltaTime * playerSpeed;
        }
        
        if (keyState.IsKeyDown(Keys.Right))
        {
	        playerX += playerRightX * deltaTime * playerSpeed;
	        playerY += playerRightY * deltaTime * playerSpeed;
        }

        if (keyState.IsKeyDown(Keys.A))
        {
	        playerDir -= deltaTime * playerRotSpeed;
        }
        
        if (keyState.IsKeyDown(Keys.D))
        {
	        playerDir += deltaTime * playerRotSpeed;
        }

        // int screenCenterY = ScreenHeight / 2;
        // for (var x = 0; x < ScreenWidth; x++)
        // {
        //     float xInterp = x / (float)ScreenWidth;
        //     Hit hit = Cast(playerX, playerY, playerDir - MathF.PI / 8 + MathF.PI / 4 * xInterp);
        //     int wallHalfHeight = (int)(ScreenHeight / hit.Distance * 0.5f);
        //     int wallTop = screenCenterY - wallHalfHeight;
        //     int wallBottom = screenCenterY + wallHalfHeight;
        //     
        //     for (var y = 0; y < wallTop; y++)
        //     {
        //         SetPixel(x, y, Color.White);
        //     }
        //     
        //     for (var y = wallTop; y < wallBottom; y++)
        //     {
        //         Color color = TileColors[hit.Tile];
        //         if (!hit.XSide)
        //         {
        //             color = new Color(color.R / 2, color.G / 2, color.B / 2);
        //         }
        //         SetPixel(x, y, color);
        //     }
        //     
        //     for (var y = wallBottom; y < ScreenHeight; y++)
        //     {
        //         SetPixel(x, y, Color.White);
        //     }
        // }
        //
        // for (var y = 0; y < MapSize; y++)
        // {
        //     for (var x = 0; x < MapSize; x++)
        //     {
        //         int tile = GetMapTile(x, y);
        //         if (tile == 0) continue;
        //         
        //         SetPixel(x, y, TileColors[tile]);
        //     }
        // }
        //
        // for (int i = 0; i < 5; i++)
        // {
        //     float x = playerX + playerForwardX * i;
        //     float y = playerY + playerForwardY * i;
        //     
        //     SetPixel((int)x, (int)y, Color.Red);
        // }

        for (int x = 0; x < ScreenWidth; x++)
        {
	        for (int y = 0; y < ScreenHeight; y++)
	        {
		        SetPixel(x, y, Color.White);
	        }
        }

        // renderWall(4, 4, 5, 4, 0, Color.Purple, 0, 1);

        DrawWall(-1, -1, 1, 1, 1, 1, Color.Purple);
        DrawWall(1, -1, 1, 1, 1, 3, Color.Purple);
        DrawFloor(-1, 1, 1, 1, 1, 3, Color.Aqua);
        
        for (var y = 0; y < MapSize; y++)
        {
	        for (var x = 0; x < MapSize; x++)
	        {
		        int tile = GetMapTile(x, y);
		        if (tile == 0) continue;
                
		        SetPixel(x + 1, y + 1, TileColors[tile]);
	        }
        }
        
        for (int i = 0; i < 5; i++)
        {
	        float x = playerX + playerForwardX * i;
	        float y = playerY + playerForwardY * i;
            
	        SetPixel((int)x, (int)y, Color.Red);
        }
        
        SetPixel((int)playerX, (int)playerY, Color.Black);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        screenTexture.SetData(screenPixels);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(screenTexture, Vector2.Zero, null, Color.White, 0f, Vector2.One,
            2f, SpriteEffects.None, 0f);
        spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
    {
	    x1 = Math.Clamp(x1, 0, ScreenWidth);
	    x2 = Math.Clamp(x2, 0, ScreenWidth);
	    x3 = Math.Clamp(x3, 0, ScreenWidth);
	    y1 = Math.Clamp(y1, 0, ScreenHeight);
	    y2 = Math.Clamp(y2, 0, ScreenHeight);
	    y3 = Math.Clamp(y3, 0, ScreenHeight);
	    
	    int topX, topY;
	    int midX, midY;
	    int btmX, btmY;

	    if (y1 < y2)
	    {
		    if (y1 < y3)
		    {
			    if (y2 < y3)
			    {
				    topX = x1;
				    topY = y1;
				    midX = x2;
				    midY = y2;
				    btmX = x3;
				    btmY = y3;
			    }
			    else
			    {
				    topX = x1;
				    topY = y1;
				    midX = x3;
				    midY = y3;
				    btmX = x2;
				    btmY = y2;
			    }
		    }
		    else
		    {
			    topX = x3;
			    topY = y3;
			    midX = x1;
			    midY = y1;
			    btmX = x2;
			    btmY = y2;
		    }
	    }
	    else
	    {
		    if (y2 < y3)
		    {
			    if (y1 < y3)
			    {
				    topX = x2;
				    topY = y2;
				    midX = x1;
				    midY = y1;
				    btmX = x3;
				    btmY = y3;
			    }
			    else
			    {
				    topX = x2;
				    topY = y2;
				    midX = x3;
				    midY = y3;
				    btmX = x1;
				    btmY = y1;
			    }
		    }
		    else
		    {
			    topX = x3;
			    topY = y3;
			    midX = x2;
			    midY = y2;
			    btmX = x1;
			    btmY = y1;
		    }
	    }

	    if (topY == midY)
	    {
		    DrawFlatTopTriangle(topX, topY, midX, midY, btmX, btmY, color);
	    }
	    else if (midY == btmY)
	    {
		    DrawFlatBottomTriangle(topX, topY, midX, midY, btmX, btmY, color);
	    }
	    else
	    {
		    int extraY = midY;
		    int extraX = (int)MathF.Floor((float)(midY - topY) / (btmY - topY) * (btmX - topX) + topX);
		    
		    DrawFlatBottomTriangle(topX, topY, midX, midY, extraX, extraY, color);
		    DrawFlatTopTriangle(midX, midY, extraX, extraY, btmX, btmY, color);
	    }
    }

    private void DrawFlatBottomTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
    {
	    int minY = y1;
	    int maxY = y2;

	    float leftSlope = (MathF.Min(x2, x3) - x1) / (maxY - minY);
	    float rightSlope = (MathF.Max(x2, x3) - x1) / (maxY - minY);

	    for (int y = minY; y < maxY; y++)
	    {
		    int dy = y - minY;
		    int minX = (int)MathF.Floor(dy * leftSlope) + x1;
		    int maxX = (int)MathF.Ceiling(dy * rightSlope) + x1;

		    for (int x = minX; x < maxX; x++)
		    {
			    SetPixel(x, y, color);
		    }
	    }
    }
    
    private void DrawFlatTopTriangle(int x1, int y1, int x2, int y2, int x3, int y3, Color color)
    {
	    int minY = y1;
	    int maxY = y3;
	    int minX = Math.Min(x1, x2);
	    int maxX = Math.Max(x1, x2);

	    float leftSlope = (x3 - minX) / (float)(maxY - minY);
	    float rightSlope = (x3 - maxX) / (float)(maxY - minY);

	    for (int y = minY; y < maxY; y++)
	    {
		    int dy = y - minY;
		    int minLineX = (int)MathF.Floor(dy * leftSlope) + minX;
		    int maxLineX = (int)MathF.Ceiling(dy * rightSlope) + maxX;

		    for (int x = minLineX; x < maxLineX; x++)
		    {
			    SetPixel(x, y, color);
		    }
	    }
    }

    private Hit Cast(float x, float y, float dir)
    {
        float rayDirX = MathF.Cos(dir);
        float rayDirY = MathF.Sin(dir);
        int rayTileDirX = MathF.Sign(rayDirX);
        int rayTileDirY = MathF.Sign(rayDirY);
        
        // TODO: Fix divide by zero here.
        // c^2 = 1^2 + (rayDirY / rayDirX)^2
        float stepX = MathF.Abs(MathF.Sqrt(1 + (rayDirY * rayDirY) / (rayDirX * rayDirX)));
        float stepY = MathF.Abs(MathF.Sqrt(1 + (rayDirX * rayDirX) / (rayDirY * rayDirY)));
        
        float initialStepX;
        float initialStepY;

        if (rayDirX > 0)
        {
            initialStepX = stepX * (MathF.Ceiling(x) - x);
        }
        else
        {
            initialStepX = stepX * (x - MathF.Floor(x));
        }
        
        if (rayDirY > 0)
        {
            initialStepY = stepY * (MathF.Ceiling(y) - y);
        }
        else
        {
            initialStepY = stepY * (y - MathF.Floor(y));
        }

        float distanceToNextX = initialStepX;
        float distanceToNextY = initialStepY;
        int tileX = (int)MathF.Floor(x);
        int tileY = (int)MathF.Floor(y);

        float lastDistanceToNext = distanceToNextX;
        float lastStep = stepX;
        bool lastSideWasX = true;

        int hitTile = 0;
        while (hitTile == 0)
        {
            if (distanceToNextX < distanceToNextY)
            {
                distanceToNextX += stepX;
                tileX += rayTileDirX;
                
                lastDistanceToNext = distanceToNextX;
                lastStep = stepX;
                lastSideWasX = true;
            }
            else
            {
                distanceToNextY += stepY;
                tileY += rayTileDirY;
                
                lastDistanceToNext = distanceToNextY;
                lastStep = stepY;
                lastSideWasX = false;
            }

            hitTile = GetMapTile(tileX, tileY);
        }

        float hitDistFromStart = lastDistanceToNext - lastStep;
        float cameraPlaneDir = playerDir + MathF.PI / 2;
        // float hitDistToCameraPlane = MathF.
        float hitX = x + hitDistFromStart * rayDirX;
        float hitY = y + hitDistFromStart * rayDirY;
        float distance = MathF.Abs(MathF.Cos(cameraPlaneDir) * (playerY - hitY) -
                                   MathF.Sin(cameraPlaneDir) * (playerX - hitX));
        
        return new Hit
        {
            Distance = distance,
            Tile = hitTile,
            XSide = lastSideWasX
        };
    }
    
    private void DrawFloor(float x0, float y0, float z0, float x1, float y1, float z1, Color color)
    {
	    float hFovFactor = ScreenHeight / MathF.Tan(MathF.PI / 4);
	    float vFovFactor = hFovFactor;
	    float zClip = 0.01f;

	    x0 -= playerX;
	    x1 -= playerX;
	    z0 -= playerY;
	    z1 -= playerY;

	    float oldX0 = x0;
	    float oldX1 = x1;
	    float oldZ0 = z0;
	    float oldZ1 = z1;

	    x0 = oldX0 * MathF.Cos(playerDir) - oldZ0 * MathF.Sin(playerDir);
	    z0 = oldX0 * MathF.Sin(playerDir) + oldZ0 * MathF.Cos(playerDir);
	    
	    x1 = oldX1 * MathF.Cos(playerDir) - oldZ1 * MathF.Sin(playerDir);
	    z1 = oldX1 * MathF.Sin(playerDir) + oldZ1 * MathF.Cos(playerDir);
	    
	    float x2 = oldX0 * MathF.Cos(playerDir) - oldZ1 * MathF.Sin(playerDir);
	    float z2 = oldX0 * MathF.Sin(playerDir) + oldZ1 * MathF.Cos(playerDir);
	    
	    float x3 = oldX1 * MathF.Cos(playerDir) - oldZ0 * MathF.Sin(playerDir);
	    float z3 = oldX1 * MathF.Sin(playerDir) + oldZ0 * MathF.Cos(playerDir);

	    if (z0 < zClip || z1 < zClip || z2 < zClip || z3 < zClip) return;

	    var screenCenter = new Point(ScreenWidth / 2, ScreenHeight / 2);
	    var bl = new Point((int)(x0 * vFovFactor / z0), (int)(y0 * hFovFactor / z0)) + screenCenter;
	    var tl = new Point((int)(x2 * vFovFactor / z2), (int)(y1 * hFovFactor / z2)) + screenCenter;
	    var br = new Point((int)(x3 * vFovFactor / z3), (int)(y0 * hFovFactor / z3)) + screenCenter;
	    var tr = new Point((int)(x1 * vFovFactor / z1), (int)(y1 * hFovFactor / z1)) + screenCenter;
	    
	    DrawTriangle(bl.X, bl.Y, br.X, br.Y, tr.X, tr.Y, color);
	    DrawTriangle(bl.X, bl.Y, tl.X, tl.Y, tr.X, tr.Y, color);
    }
    
    private void DrawWall(float x0, float y0, float z0, float x1, float y1, float z1, Color color)
    {
	    float hFovFactor = ScreenHeight / MathF.Tan(MathF.PI / 4);
	    float vFovFactor = hFovFactor;
	    float zClip = 0.01f;
	    
	    x0 -= playerX;
	    x1 -= playerX;
	    z0 -= playerY;
	    z1 -= playerY;
	    
	    float oldX0 = x0;
	    x0 = x0 * MathF.Cos(playerDir) - z0 * MathF.Sin(playerDir);
	    z0 = oldX0 * MathF.Sin(playerDir) + z0 * MathF.Cos(playerDir);
	    
	    float oldX1 = x1;
	    x1 = x1 * MathF.Cos(playerDir) - z1 * MathF.Sin(playerDir);
	    z1 = oldX1 * MathF.Sin(playerDir) + z1 * MathF.Cos(playerDir);

	    if (z0 < zClip || z1 < zClip)
	    {
		    return;
	    }
	    
	    z0 = MathF.Max(z0, zClip);
	    z1 = MathF.Max(z1, zClip);
	    
	    var bl = new Point((int)(x0 * vFovFactor / z0), (int)(y1 * hFovFactor / z0));
	    var tl = new Point((int)(x0 * vFovFactor / z0), (int)(y0 * hFovFactor / z0));
	    var br = new Point((int)(x1 * vFovFactor / z1), (int)(y1 * hFovFactor / z1));
	    var tr = new Point((int)(x1 * vFovFactor / z1), (int)(y0 * hFovFactor / z1));

	    float tSlope = (tr.Y - tl.Y) / (float)(tr.X - tl.X);
	    float bSlope = (br.Y - bl.Y) / (float)(br.X - bl.X);

	    int minX = Math.Min(bl.X, br.X);
	    int maxX = Math.Max(bl.X, br.X);

	    for (int x = minX; x < maxX; x++)
	    {
		    int dx = x - bl.X;
		    var ty = (int)(tl.Y + tSlope * dx);
		    var by = (int)(bl.Y + bSlope * dx);
		    int minY = Math.Min(ty, by);
		    int maxY = Math.Max(ty, by);

		    for (int y = minY; y < maxY; y++)
		    {
			    int cx = x + ScreenWidth / 2;
			    int cy = y + ScreenHeight / 2;

			    float u = (x - minX) / (float)(maxX - minX) * texture.Length;
			    Color texColor = texture[(int)u % 16];

			    if (cx >= 0 && cy >= 0 && cx < ScreenWidth && cy < ScreenHeight)
			    {
				    // screenPixels[cx + cy * ScreenWidth] = color;
				    screenPixels[cx + cy * ScreenWidth] = texColor;
			    }
		    }
	    }
    }

    private void SetPixel(int x, int y, Color color)
    {
        if (x < 0 || y < 0 || x >= ScreenWidth || y >= ScreenHeight) return;
        
        screenPixels[x + y * ScreenWidth] = color;
    }

    private int GetMapTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= MapSize || y >= MapSize) return 2;

        return Map[x + y * MapSize];
    }
}