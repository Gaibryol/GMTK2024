public class LevelEvents
{
    public class EndLevel
    {
        public readonly bool Victory;
        public readonly string NextLevel;

        public EndLevel(string nextLevel, bool victory)
        {
            Victory = victory;
            NextLevel = nextLevel;
        }
    }
}
