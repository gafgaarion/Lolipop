namespace FFACETools
{


    #region Proc strength
    public enum VoidwatchProcStrength : byte
    {
        Yellow,
        Red,
        White
    }
    #endregion

    #region Proc type
    public enum VoidwatchProcType : byte
    {
        FindProcSpell,

        // Magic
        BlackMagic,
        WhiteMagic,
        BlueMagic,
        Ninjutsu,
        Song,
        Summon,

        // JA
        BlackMageJA,
        CorsairJA,
        DancerJA,
        DarkKnightJA,
        DragoonJA,
        MonkJA,
        PaladinJA,
        RangerJA,
        ScholarJA,
        ThiefJA,
        WarriorJA,

        // Weaponskill
        Archery,
        Axe,
        Automaton,
        Club,
        Dagger,
        GreatAxe,
        GreatKatana,
        GreatSword,
        Hand2Hand,
        Katana,
        Marksmanship,
        Polearm,
        Scythe,
        Staff,
        Sword
    }

    #endregion

    #region Elements
    public enum Elements : byte
    {
        Wind,
        Water,
        Fire,
        Lightning,
        Light,
        Dark,
        Earth,
        Ice,
        Neutral
    }

    #endregion


}

