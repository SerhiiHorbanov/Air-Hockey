class Game
{
    void Run()
    {
        while (GameContinues())
        {
            Render();
            Input();
            Update();
        }
    }

    void Render()
    {
        
    }

    void Input()
    {
        
    }
    
    void Update()
    {
        
    }
    
    bool GameContinues()
    {
        return true;
    }
}