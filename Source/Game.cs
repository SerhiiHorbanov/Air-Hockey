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
    
    private const int PuckRadius = 50;
    private const int DiscsRadius = 50;

    private Vector2f _playerDiscWishedPosition;

    private static readonly Vector2f RelativeToPuckResettingDiscsPosition = new (0, 350);
    
    private readonly Rectangle _rightEdge;
    private readonly Rectangle _leftEdge;

    private const float EdgeThickness = 1000;

    private int _firstPlayerScore;
    private int _secondPlayerScore;

    private static readonly Color FirstPlayerColor = Color.Red;
    private static readonly Color SecondPlayerColor = Color.Blue;
    
    private Text _firstPlayerScoreText;
    private Text _secondPlayerScoreText;

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

        _puck = new(PuckRadius, new(), new(), Color.Black);
        
        _firstPlayerDisc = new(DiscsRadius, new(), new(), FirstPlayerColor);
        _secondPlayerDisc = new(DiscsRadius, new(), new(), SecondPlayerColor);
        
        _firstPlayerScoreText = new();
        _secondPlayerScoreText = new();
        
        _window = new(new (900, 900), "air hockey");
        
        ResetPuckAndDiscsPositionsAndVelocities();
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
        InitializeScoreTexts();

        _window.Closed += WindowClosed;
    }

    private void InitializeScoreTexts()
    {
        SetupScoreText(ref _firstPlayerScoreText, FirstPlayerColor);
        SetupScoreText(ref _secondPlayerScoreText, SecondPlayerColor, -FontSize);
    }

    private void SetupScoreText(ref Text scoreText, Color color, float y = 0)
    {
        scoreText.Position = _tablePosition - new Vector2f(_halfTableSize.X, y);
        scoreText.DisplayedString = "0";
        scoreText.FillColor = color;
        scoreText.CharacterSize = FontSize;
        
        scoreText.Font = Arial;
    }

    static void WindowClosed(object sender, EventArgs e)
    {
        RenderWindow w = (RenderWindow)sender;
        w.Close();
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
        _firstPlayerScoreText.Draw(_window, RenderStates.Default);
        _secondPlayerScoreText.Draw(_window, RenderStates.Default);
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
            _secondPlayerScore++;
            _secondPlayerScoreText.DisplayedString = _secondPlayerScore.ToString();
            ResetPuckAndDiscsPositionsAndVelocities();
            return;
        }
        
        if (_puck.Position.Y > _tablePosition.Y - _halfTableSize.Y) 
            return;
        
        _firstPlayerScore++;
        _firstPlayerScoreText.DisplayedString = _firstPlayerScore.ToString();
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
    
    private bool GameContinues()
    {
        return _window.IsOpen;
    }
}