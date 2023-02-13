using FixMath;

namespace Logic
{
    public class BaseRecord
    {
        public int Frame;
    }

    public class UnitRecord : BaseRecord
    {
        public FVector2 Pos;
        public FVector2 MoveDir;
    }
}