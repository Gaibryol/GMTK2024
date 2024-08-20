public class LevelEvents
{
    public class EndLevel
    {
        public readonly bool Victory;
        public readonly string NextLevel;
        public readonly float TransitionDelay;

        public EndLevel(string nextLevel, bool victory, float transitionDelay=0f)
        {
            Victory = victory;
            NextLevel = nextLevel;
            TransitionDelay = transitionDelay;

        }
    }
}
