namespace App.Achievements
{
    public enum TaskType
    {
        NONE = 0,
        
        //Eternal
        BATTLES_PLAYED =1, //
        BATTLES_WIN=2,
        CARDS_OPENED=3, //
        CARDS_MERGED=4, //
        BOSS_WIN=5,
        SMALL_CHEST_OPENED=6,
        MEDIUM_SCEST_OPENED=7,
        MEGA_CHEST_OPENED=8,
        GIGA_CHEST_OPENED=9,
        CARDS_1_RANK_TAKEN=10, //
        CARDS_2_RANK_TAKEN=11, //
        CARDS_3_RANK_TAKEN=12, //
        CARDS_4_RANK_TAKEN=13, //
        CARDS_5_RANK_TAKEN=14, // 
        CARDS_6_RANK_TAKEN=15, //
        CARDS_7_RANK_TAKEN=16, //
        CARDS_8_RANK_TAKEN=17, //
        CARDS_9_RANK_TAKEN=18, //
        CARDS_10_RANK_TAKEN=19, //
        
        
        //Daily
        WIN_1_BATTLE=20, 
        OPEN_1_CHEST=21,
        MERGE_2_CARDS=22, //
        PLAY_3_BATTLE=23, //
        TAKE_1_CHEST=24,
        WIN_3_BATTLE=25
    }
    
    public enum TaskGroup
    {
        NONE =0,
        BATTLE_END =1,
        CHEST_OPENED=2,
        CHEST_TAKEN=3,
        CARD_TAKEN=5,
    }
}