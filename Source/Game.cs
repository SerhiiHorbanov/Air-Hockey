using SFML.Graphics;
using SFML.System;
using SFML.Window;

internal class Game
{
    private RenderWindow _window;
    private static readonly Color BackGroundColor = Color.White;
    
    private readonly Vector2f _tablePosition;
    private readonly Vector2f _tableSize;
    
    private Circle _puck;
    private Circle _firstPlayerDisc;
    private Circle _secondPlayerDisc;
    
    private const int PuckRadius = 50;
    private const int DiscsRadius = 50;

    private Vector2f _playerDiscWishedPosition;

    private static readonly Vector2f RelativeToPuckResettingDiscsPosition = new (0, 350);
    
    private Rectangle _rightEdge;
    private Rectangle _leftEdge;

    private const float EdgeThickness = 1000;

    private int _firstPlayerScore;
    private int _secondPlayerScore;

    private static readonly Color FirstPlayerColor = Color.Red;
    private static readonly Color SecondPlayerColor = Color.Blue;
    
    private Text _firstPlayerScoreText;
    private Text _secondPlayerScoreText;

    private const int FontSize = 36;
    private static readonly Font Arial = new("C:/Windows/Fonts/arial.ttf");
    
    public Game() : this(new Vector2f(450, 450), new Vector2f(800, 900))
    { }

    private Game(Vector2f tablePosition, Vector2f tableSize)
    {
        _tableSize = tableSize;
        _tablePosition = tablePosition;
    }
    
    public void Run()
    {
        Initialization();
        while (GameContinues())
        {
            Render();
            Input();
            Update();
            FrameTiming.Timing();
        }
    }

    private bool GameContinues()
    {
        return _window.IsOpen;
    }
    
    private void Initialization()
    {
        InitializeTable();
        InitializeUI();
        
        FrameTiming.UpdateLastTimingTick();

        _window = new(new (900, 900), "air hockey");
        _window.Closed += WindowClosed;
    }

    private void InitializeTable()
    {
        Vector2f halfTableSize = _tableSize * 0.5f;
        Vector2f edgeSize = new(EdgeThickness, _tableSize.Y + EdgeThickness);

        Vector2f tableLeftTop = _tablePosition - halfTableSize;
        tableLeftTop.X -= EdgeThickness;
        _leftEdge = new (tableLeftTop, edgeSize, Color.Black);

        Vector2f tableRightTop = _tablePosition + halfTableSize;
        tableRightTop.Y -= _tableSize.Y + EdgeThickness / 2;
        _rightEdge = new (tableRightTop, edgeSize, Color.Black);

        _puck = new(PuckRadius, new(), new(), Color.Black);
        
        _firstPlayerDisc = new(DiscsRadius, FirstPlayerColor);
        _secondPlayerDisc = new(DiscsRadius, SecondPlayerColor);
        
        ResetPuckAndDiscsPositionsAndVelocities();
    }

    private void InitializeUI()
    {
        _firstPlayerScoreText = new();
        _secondPlayerScoreText = new();
        
        SetupScoreText(ref _firstPlayerScoreText, FirstPlayerColor);
        SetupScoreText(ref _secondPlayerScoreText, SecondPlayerColor, -FontSize);
    }

    private void SetupScoreText(ref Text scoreText, Color color, float y = 0)
    {
        scoreText.Position = _tablePosition - new Vector2f(_tableSize.X * 0.5f, y);
        scoreText.DisplayedString = "0";
        scoreText.FillColor = color;
        scoreText.CharacterSize = FontSize;
        
        scoreText.Font = Arial;
    }

    private void WindowClosed(object sender, EventArgs e)
    {
        RenderWindow window = (RenderWindow)sender;
        window.Close();
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
        
        AccelerateDiscToPoint(ref _secondPlayerDisc, secondDiscWishedPosition, 0.4f);
    }

    private void AccelerateDiscToPoint(ref Circle acceleratingDisc, Vector2f acceleratingToPosition, float interpolation)
    {
        float resultInterpolation = interpolation * FrameTiming.DeltaSeconds / FrameTiming.TargetDeltaSeconds;
        
        acceleratingDisc.Velocity = acceleratingDisc.Velocity.Lerp(acceleratingToPosition - acceleratingDisc.Position, resultInterpolation);
    }
    
    private void UpdatePhysics()
    {
        UpdateDiscPhysics(_firstPlayerDisc);
        UpdateDiscPhysics(_secondPlayerDisc);
        
        _puck.UpdateVelocity(FrameTiming.DeltaSeconds);
        _puck.CheckAndResolveCollision(_firstPlayerDisc);
        _puck.CheckAndResolveCollision(_secondPlayerDisc);
        _puck.CheckAndResolveCollision(_rightEdge);
        _puck.CheckAndResolveCollision(_leftEdge);
    }

    private void UpdateDiscPhysics(Circle disc)
    {
        disc.UpdateVelocity(FrameTiming.DeltaSeconds);
        disc.CheckAndResolveCollision(_rightEdge);
        disc.CheckAndResolveCollision(_leftEdge);
    }

    private void CheckAndResolveWinCondition()
    {
        if (_puck.Position.Y > _tablePosition.Y + _tableSize.Y * 0.5f)
        {
            _secondPlayerScore++;
            _secondPlayerScoreText.DisplayedString = _secondPlayerScore.ToString();
            ResetPuckAndDiscsPositionsAndVelocities();
            return;
        }
        
        if (_puck.Position.Y > _tablePosition.Y - _tableSize.Y * 0.5f) 
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
}