namespace MRGS.Imp
{
    public interface MRGSServicesDelegate
    {
        void Login();
        void Logout();
        void OpenAchievements();
        void OpenLeaderboard();
        void SendScoreLeaderboard(ulong LeaderboardScore, string LeaderboardId);
        void SendAchievement(string AchievementId);
    }
}
