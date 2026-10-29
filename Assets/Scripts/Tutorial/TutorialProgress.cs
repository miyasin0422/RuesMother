public static class TutorialProgress
{
    public static bool RueAwakened = false;

    // 説明済みのチュートリアルは、ステージを移動しても再実行しない。
    public static bool JumpComplete = false;
    public static bool DashComplete = false;
    public static bool DropComplete = false;
    public static bool FallComplete = false;
    public static bool BattleComplete = false;
    public static bool GeneComplete = false;
    public static bool JumpBattleComplete = false;
    public static bool HealComplete = false;
    public static bool SaveComplete = false;
    public static bool BossComplete = false;

    // ニューゲーム開始時に呼ぶ。
    public static void ResetAll()
    {
        RueAwakened = false;
        JumpComplete = false;
        DashComplete = false;
        DropComplete = false;
        FallComplete = false;
        BattleComplete = false;
        GeneComplete = false;
        JumpBattleComplete = false;
        HealComplete = false;
        SaveComplete = false;
        BossComplete = false;
    }
}