using System;

namespace Either.Root
{
    public struct RootEither<TLeft, TRight>
    {
        public bool IsLeftPresent { get; }
        public bool IsRightPresent { get; }

        private readonly TLeft _left;
        private readonly TRight _right;

        public TLeft Left
        {
            get
            {
                if (IsLeftPresent && !IsRightPresent)
                {
                    return _left;
                }

                throw new FieldAccessException("Value is not present");
            }
        }

        public TRight Right
        {
            get
            {
                if (IsRightPresent && !IsLeftPresent)
                {
                    return _right;
                }

                throw new FieldAccessException("Value is not present");
            }
        }

        public Type LeftType => typeof(TLeft);
        public Type RightType => typeof(TRight);

        public RootEither(TLeft left)
        {
            this._left = left;
            _right = default;

            IsLeftPresent = true;
            IsRightPresent = false;
        }

        public RootEither(TRight right)
        {
            this._right = right;
            _left = default;

            IsLeftPresent = false;
            IsRightPresent = true;
        }

        public RootEither<TLeft, TRight> Of(TLeft left) => new RootEither<TLeft, TRight>(left);
        public RootEither<TLeft, TRight> Of(TRight right) => new RootEither<TLeft, TRight>(right);
        

        // assignment operators

        public static implicit operator RootEither<TLeft, TRight>(TLeft left) => new RootEither<TLeft, TRight>(left);
        public static implicit operator RootEither<TLeft, TRight>(TRight right) => new RootEither<TLeft, TRight>(right);


        public static explicit operator TLeft(RootEither<TLeft, TRight> value) => value.Left;
        public static explicit operator TRight(RootEither<TLeft, TRight> value) => value.Right;
    }
}
