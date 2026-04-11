using UnityEngine.Rendering;
    public class TrackingHandler
    {
        public static void OnLevelStart(string level_mode, int level, int attempt_num, int level_score)
        {
            // TrackingManager.SendEvent("level_start",
            //         "level_mode", level_mode,
            //         TrackingConstants.kLevel, level,
            //         "attempt_num", attempt_num,
            //         "level_score", level_score
            //     );
        }
        public static void OnClassicStart(int attempt_num, int level_score)
        {
            // TrackingManager.SendEvent("level_start",
            //         "level_mode", PlayMode.Classic.ToString(),
            //         "attempt_num", attempt_num,
            //         "level_score", level_score);
        }
        public static void OnAdventureStart(int level, int attempt_num, int level_score)
        {
            // TrackingManager.SendEvent("level_start",
            //         "level_mode", PlayMode.Adventure.ToString(),
            //         TrackingConstants.kLevel, level,
            //         "attempt_num", attempt_num,
            //         "level_score", level_score);
        }
        public static void OnClassicContinue(int score, int attempt_num)
        {
            // TrackingManager.SendEvent(TrackingConstants.kLevelContinue,
            //     TrackingConstants.kLevelMode, PlayMode.Classic.ToString(),
            //     TrackingConstants.kLevelScore, score,
            //     TrackingConstants.kAttemptNum, attempt_num,
            //     TrackingConstants.kPhase, "",
            //     TrackingConstants.kItemUsed, "");
        }
        public static void OnAdventureContinue(int score, int attempt_num)
        {
            // TrackingManager.SendEvent(TrackingConstants.kLevelContinue,
            //     TrackingConstants.kLevelMode, PlayMode.Adventure.ToString(),
            //     TrackingConstants.kLevelScore, score,
            //     TrackingConstants.kAttemptNum, attempt_num,
            //     TrackingConstants.kPhase, "",
            //     TrackingConstants.kItemUsed, "");
        }
        public static void OnClassicEnd(int attempt_num, int level_score, int play_time, int continue_num, bool win = false)
        {
            // TrackingManager.SendEvent("level_end",
            //         "level_mode", PlayMode.Classic.ToString(),
            //         "attempt_num", attempt_num,
            //         "level_score", level_score,
            //         "use_item_num", 0,
            //         "action_num", 0,
            //         "continue_num", continue_num,
            //         "lose_cause", "",
            //         "win", win);

        }
        public static void OnAdventureEnd(int attempt_num, int level_score, int play_time, int continue_num, bool win)
        {
            // TrackingManager.SendEvent("level_end",
            //         "level_mode", PlayMode.Adventure.ToString(),
            //         "attempt_num", attempt_num,
            //         "level_score", level_score,
            //         "use_item_num", 0,
            //         "action_num", 0,
            //         "continue_num", continue_num,
            //         "lose_cause", "",
            //         "win", win);

        }
        public static void DefaultOnMatchStart(int level, string play_mode, int start_score)
        {
            // TrackingManager.OnLevelStart(level, play_mode, 1, start_score);
        }
        public static void DefaultOnMatchContinue(int level, string play_mode, int start_score)
        {
            // TrackingManager.OnLevelContinue(level, play_mode, 1, "", "", start_score);
        }
        public static void DefaultOnMatchEnd(int level, string play_mode, int start_score, bool isWin, int play_time, int action_num)
        {
            // TrackingManager.OnLevelEnd(level, play_mode, 1, start_score, isWin, play_time, action_num, 0, 0, "", -1, -1, -1, -1);
        }
        public static void OnTutorialBegin(string tutorial_id, string view, int step)
        {
            // TrackingManager.SendEvent("tutorial" + tutorial_id + "_begin",
            //     "view", view,
            //     "step", step);
        }
        public static void OnTutorialCompleted(string tutorial_id, string view, int step)
        {
            // TrackingManager.SendEvent("tutorial" + tutorial_id + "_complete",
            //     "placement", view,
            //     "step", step);
        }

    }
