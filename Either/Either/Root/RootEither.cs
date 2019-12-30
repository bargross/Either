using System;

namespace Either.Root
{
    public struct RootEither<L, R>
    {
        public bool IsLeftPresent { get; }
        public bool IsRightPresent { get; }

        private readonly L _left;
        private readonly R _right;

        public L Left
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

        public R Right
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

        public Type LeftType => typeof(L);
        public Type RightType => typeof(R);

        public RootEither(L left)
        {
            this._left = left;
            _right = default;

            IsLeftPresent = true;
            IsRightPresent = false;
        }

        public RootEither(R right)
        {
            this._right = right;
            _left = default;

            IsLeftPresent = false;
            IsRightPresent = true;
        }

        public RootEither<L, R> Of(L left)
        {
            return new RootEither<L, R>(left);
        }

        public RootEither<L, R> Of(R right)
        {
            return new RootEither<L, R>(right);
        }

        // assignment operators

        public static implicit operator RootEither<L, R>(L left)
        {
            return new RootEither<L, R>(left);
        }

        public static implicit operator RootEither<L, R>(R right)
        {
            return new RootEither<L, R>(right);
        }

        public static explicit operator L(RootEither<L, R> value)
        {
            return value.Left;
        }

        public static explicit operator R(RootEither<L, R> value)
        {
            return value.Right;
        }
    }
}
