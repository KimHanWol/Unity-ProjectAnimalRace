using System.Collections;

public interface FeverInterface
{
    public void FeverInitialize();
    public void FeverReadyForStart(float FirstDelay, float GrowDuration, float DelayAfterGrown, float TurnDelay, string EmojiKey, float EmojiDuration, float LastDuration);
    public void FeverStart();
    public void FeverReadyForFinish(float FinishFirstDelay, float ShrinkDuration, float DelayAfterShrink, string FinishEmojiKey, float FinishEmojiDuration, float FinishLastDuration);
    public void FeverFinished();
}