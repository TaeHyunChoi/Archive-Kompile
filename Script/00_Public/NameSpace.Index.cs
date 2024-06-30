namespace Index
{
    using System;

    public static class IDxInput
    {
        private const int SHIFT_BIT_HOLD = 8;

        [Flags]
        public enum EInput
        {
            NONE = 0,

            DOWN = 1 << 0,
            UP = 1 << 1,
            LEFT = 1 << 2,
            RIGHT = 1 << 3,
            ENTER = 1 << 4,
            CANCEL = 1 << 5,
            ESCAPE = 1 << 6,
            ACTION = 1 << 7,

            DOWN_HOLD = 1 << (DOWN + SHIFT_BIT_HOLD),
            UP_HOLD = 1 << (UP + SHIFT_BIT_HOLD),
            LEFT_HOLD = 1 << (LEFT + SHIFT_BIT_HOLD),
            RIGHT_HOLD = 1 << (RIGHT + SHIFT_BIT_HOLD),
            ENTER_HOLD = 1 << (ENTER + SHIFT_BIT_HOLD),
            CANCEL_HOLD = 1 << (CANCEL + SHIFT_BIT_HOLD),
            ESCAPE_HOLD = 1 << (ESCAPE + SHIFT_BIT_HOLD),
            ACTION_HOLD = 1 << (ACTION + SHIFT_BIT_HOLD),
            MASK_HOLD = 0x0F << SHIFT_BIT_HOLD,

            MOVE_ALL = UP | DOWN | LEFT | RIGHT | UP_HOLD | DOWN_HOLD| LEFT_HOLD | RIGHT_HOLD,
            ALL = 0xFF
        }

        public static bool HaveFlag(this EInput input, EInput compare)
        {
            //아무런 입력이 없는 상태를 확인한다.
            if (EInput.NONE == input && EInput.NONE == compare)
            {
                return true;
            }

            return 0 != (input & compare);
        }
        public static bool HaveFlag(this EInput input, params EInput[] compares)
        {
            for (int i = 0; i < compares.Length; ++i)
            {
                if (0 != (input & compares[i]))
                {
                    return true;
                }
            }

            return false;
        }
    }
    public static class IDxTile
    {
        public const float SIZE                = 1f;
        public const float SIZE_INVERSE        = 1f / SIZE;
        public const float SIZE_HALF           = 0.5f * SIZE;
        public const float SIZE_HALF_INVERSE   = 1f / SIZE_HALF;
        public const float SIZE_QUATER         = 0.25f * SIZE;
        public const float SIZE_QUATER_INVERSE = 1f / SIZE_QUATER;

        public const byte SHIFT_KEY_LAYER = 21;  // 3bits, mask
        public const byte SHIFT_KEY_SCALE = 20;  // 1bit,  flag
        public const byte SHIFT_KEY_X     = 12;  // 8bits, mask
        public const byte SHIFT_KEY_Y     =  8;  // 4bits, mask
        public const byte SHIFT_KEY_Z     =  0;  // 8bits, mask

        public const byte SHIFT_TRIGGER_SCALE           = 14;
        public const byte SHIFT_TRIGGER_SCALE_VALUE     = 13;
        public const byte SHIFT_TRIGGER_LAYER           = 12;
        public const byte SHIFT_TRIGGER_LAYER_VALUE     =  8;
        public const byte SHIFT_TRIGGER_INTERACT        =  7;
        public const byte SHIFT_TRIGGER_INTERACT_VALUE  =  0;

        public const byte SHIFT_INFO_SCALE = 7;
    }
    public static class IDxStat
    {
        public const byte BIT_INDEX_HP  =  0;
        public const byte BIT_INDEX_MP  =  9;
        public const byte BIT_INDEX_BP  = 18;
        public const byte BIT_INDEX_STR = 27;
        public const byte BIT_INDEX_CON = 36;
        public const byte BIT_INDEX_INT = 45;
        public const byte BIT_INDEX_WIS = 54;
        public const byte BIT_INDEX_DEX = 63;
        public const byte BIT_INDEX_AGI = 72;
        public const byte BIT_INDEX_LUK = 81;
        public const byte BIT_INDEX_CHA = 90;
    }
}