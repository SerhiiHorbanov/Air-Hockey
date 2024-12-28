using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Game
{
    private RenderWindow _window;
    private static readonly Color BackGroundColor = Color.White;
    
    private readonly Vector2f _tablePosition;
    private readonly Vector2f _halfTableSize;
    
    private Circle _puck;
    private Circle _firstPlayerDisc;
    private Circle _secondPlayerDisc;

    private Vector2f _playerDiscWishedPosition;

    private static readonly Vector2f RelativeToPuckResettingDiscsPosition = new (0, 350);
    
    private readonly Rectangle _rightEdge;
    private readonly Rectangle _leftEdge;

    private const float EdgeThickness = 1000;
    private const float HalfEdgeThickness = EdgeThickness / 2;

    private int _firstPlayerScore;
    private int _secondPlayerScore;

    private static readonly Color FirstPlayerColor = Color.Red;
    private static readonly Color SecondPlayerColor = Color.Blue;
    
    private Text _scoresText;

    private const int FontSize = 36;
    private static readonly Font Arial = new("C:/Windows/Fonts/arial.ttf");

    public Game() : this(new Vector2f(800, 900), new Vector2f(450, 450))
    { }

    private Game(Vector2f tableSize, Vector2f tablePosition)
    {
        _tablePosition = tablePosition;
        _halfTableSize = tableSize * 0.5f;

        Vector2f edgeSize = new(EdgeThickness, tableSize.Y + EdgeThickness);

        Vector2f tableLeftTop = _tablePosition - _halfTableSize;
        tableLeftTop.X -= EdgeThickness;
        _leftEdge = new (tableLeftTop, edgeSize, Color.Black);

        Vector2f tableRightTop = _tablePosition + _halfTableSize;
        tableRightTop.Y -= tableSize.Y;
        _rightEdge = new (tableRightTop, tableSize, Color.Black);

        _puck = new(50, new(115, 100), new(0, 1f), Color.Black);
        
        _firstPlayerDisc = new(50, _tablePosition, new(0, 0), FirstPlayerColor);
        _secondPlayerDisc = new(50, _tablePosition, new(0, 0), SecondPlayerColor);
        
        _scoresText = new();
        
        _window = new(new (900, 900), "window");
        
        ResetPuckAndDiscsPositionsAndVelocities();
    }

    private void ResetPuckAndDiscsPositionsAndVelocities()
    {
        _puck.Position = _tablePosition;
        _firstPlayerDisc.Position = _puck.Position + RelativeToPuckResettingDiscsPosition;
        _secondPlayerDisc.Position = _puck.Position - RelativeToPuckResettingDiscsPosition;
        
        _puck.Velocity = new(0, 0);
        _firstPlayerDisc.Velocity = new(0, 0);
        _secondPlayerDisc.Velocity = new(0, 0);
    }
    
    public void Run()
    {
        Initialization();
        while (GameContinues())
        {
            Render();
            Input();
            Update();
            Thread.Sleep(16);
        }
    }

    private void Initialization()
    {
        _scoresText.Position = _tablePosition - new Vector2f(_halfTableSize.X, 0);
        _scoresText.DisplayedString = "lorem ipsum";
        _scoresText.FillColor = Color.Black;
        _scoresText.CharacterSize = FontSize;
        
        _scoresText.Font = Arial;
    }

    private void Render()
    {
        _window.Clear(BackGroundColor);
        
        DrawObjects();
        DrawScore();

        _window.Display();
    }
    
    private void DrawObjects()
    {
        _firstPlayerDisc.Draw(_window);
        _secondPlayerDisc.Draw(_window);
        
        _puck.Draw(_window);
        
        _rightEdge.Draw(_window);
        _leftEdge.Draw(_window);
    }
    
    private void DrawScore()
    {
        _scoresText.DisplayedString = _firstPlayerScore + "/" + _secondPlayerScore;
        _scoresText.Draw(_window, RenderStates.Default);
    }

    private void Input()
    {
        _window.DispatchEvents();
        
        _playerDiscWishedPosition = (Vector2f)(Mouse.GetPosition() - _window.Position);
    }
    
    private void Update()
    {
        UpdateDiscsVelocities();
        UpdatePhysics();

        CheckAndResolveWinCondition();
    }

    private void UpdateDiscsVelocities()
    {
        AccelerateDiscToPoint(ref _firstPlayerDisc, _playerDiscWishedPosition, 0.5f);
        _firstPlayerDisc.Velocity = _firstPlayerDisc.Velocity.Lerp(_playerDiscWishedPosition - _firstPlayerDisc.Position, 0.5f);

        bool isPuckInSecondPlayersHalf = _puck.Position.Y < _tablePosition.Y;
        
        Vector2f secondDiscWishedPosition = isPuckInSecondPlayersHalf ? _puck.Position : new(_puck.Position.X, 100);
        float speed = isPuckInSecondPlayersHalf ? 0.05f : 0.1f;
        
        AccelerateDiscToPoint(ref _secondPlayerDisc, secondDiscWishedPosition, speed);
    }

    private void AccelerateDiscToPoint(ref Circle acceleratingDisc, Vector2f acceleratingToPosition, float interpolation)
    {
        acceleratingDisc.Velocity = acceleratingDisc.Velocity.Lerp(acceleratingToPosition - acceleratingDisc.Position, interpolation);
    }
    
    private void UpdatePhysics()
    {
        UpdateDiscPhysics(_firstPlayerDisc);
        UpdateDiscPhysics(_secondPlayerDisc);
        
        _puck.UpdateVelocity();
        _puck.CheckAndResolveCollision(_firstPlayerDisc);
        _puck.CheckAndResolveCollision(_secondPlayerDisc);
        _puck.CheckAndResolveCollision(_rightEdge);
        _puck.CheckAndResolveCollision(_leftEdge);
    }

    private void UpdateDiscPhysics(Circle disc)
    {
        disc.UpdateVelocity();
        disc.CheckAndResolveCollision(_rightEdge);
        disc.CheckAndResolveCollision(_leftEdge);
    }

    private void CheckAndResolveWinCondition()
    {
        if (_puck.Position.Y > _tablePosition.Y + _halfTableSize.Y)
        {
            ResetPuckAndDiscsPositionsAndVelocities();
            _secondPlayerScore++;
        }
        else if (_puck.Position.Y < _tablePosition.Y - _halfTableSize.Y)
        {
            ResetPuckAndDiscsPositionsAndVelocities();
            _firstPlayerScore++;
        }
    }
    
    private bool GameContinues()
    {
        return _window.IsOpen;
    }
}