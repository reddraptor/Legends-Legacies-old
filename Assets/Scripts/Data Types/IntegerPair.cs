public struct IntegerPair
{
    public int x;
    public int y;

    public int X
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
        }
    }
    public int Y
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
        }
    }
    public int I
    {
        get
        {
            return x;
        }
        set
        {
            x = value;
        }
    }
    public int J
    {
        get
        {
            return y;
        }
        set
        {
            y = value;
        }
    }

    public int Horizontal
    {
        get { return x; }
        set { x = value; }
    }

    public int Vertical
    {
        get { return y; }
        set { y = value; }
    }

    public IntegerPair(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return "(" + X + ", " + Y + ")";
    }
}
